using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Application.HackerNews.Dtos;
using Application.HackerNews.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.HackerNews.Services;

public class HackerNewsService : IHackerNewsService
{
    private readonly IHackerNewsHttpGateway _hackerNewsHttpGateway;
    private readonly ILogger<HackerNewsService> _logger;
    
    private const int DefaultDegreeOfParallelism = 5;
    private static int _maxDegreeOfParallelism;

    public HackerNewsService(
        IHackerNewsHttpGateway hackerNewsHttpGateway, 
        IConfiguration configuration, 
        ILogger<HackerNewsService> logger)
    {
        _hackerNewsHttpGateway = hackerNewsHttpGateway;
        _logger = logger;
        _maxDegreeOfParallelism = configuration.GetValue<int?>("HackerNewsGateway:ConcurrencyThreshold") ?? DefaultDegreeOfParallelism;
    }
    
    public async Task<IEnumerable<NewsDto>> GetBestNews(int top, CancellationToken ct = default)
    {
        var bestNewsIds = _hackerNewsHttpGateway.GetBestNewsIds(top, ct);

        return await GetBestNewsInParallel(bestNewsIds, ct)
            .OrderByDescending(n => n.Score)
            .ToListAsync(ct);
    }

    private async IAsyncEnumerable<NewsDto> GetBestNewsInParallel(IAsyncEnumerable<int> bestNewsIds,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var channel = Channel.CreateUnbounded<NewsDto>();
        
        await Task.Run(async () =>
        {
            await Parallel.ForEachAsync(bestNewsIds, new ParallelOptions
                {
                    MaxDegreeOfParallelism = _maxDegreeOfParallelism,
                    CancellationToken = ct
                }, 
                async (id, _) =>
            {
                try
                {
                    var item = await _hackerNewsHttpGateway.GetNewsDetails(id, ct);
                    if (item != null)
                    {
                        var commentCount = await CountComments(item.Kids, ct);
                        var dto = NewsDto.MapFrom(item, commentCount);
                        await channel.Writer
                            .WriteAsync(dto, ct);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error fetching item {Id}: {ErrorMessage}", id, ex.Message);
                }
            });

            channel.Writer.Complete();
        }, ct);
        
        await foreach (var item in channel.Reader.ReadAllAsync(ct))
        {
            yield return item;
        }
    }
    
    private async ValueTask<int> CountComments(int[] commentIds, CancellationToken ct)
    {
        if (commentIds.Length == 0)
            return 0;

        var channel = Channel.CreateUnbounded<int>();
        var totalCommentsCount = 0;
        var commentsToProcessCount = 0;
        
        foreach (var id in commentIds)
        {
            await channel.Writer
                .WriteAsync(id, ct);
            Interlocked.Increment(ref commentsToProcessCount);
        }
        
        await Task.Run(async () =>
        {
            await Parallel.ForEachAsync(channel.Reader.ReadAllAsync(ct), new ParallelOptions
            {
                MaxDegreeOfParallelism = _maxDegreeOfParallelism,
                CancellationToken = ct
            }, async (commentId, _) =>
            {
                try
                {
                    var comment = await _hackerNewsHttpGateway.GetCommentDetails(commentId, ct);
                    Interlocked.Increment(ref totalCommentsCount);

                    if (comment?.Kids is { Length: > 0 })
                    {
                        foreach (var kid in comment.Kids)
                        {
                            await channel.Writer
                                .WriteAsync(kid, ct);
                            Interlocked.Increment(ref commentsToProcessCount);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to fetch comment {Id}: {Error}", commentId, ex.Message);
                }
                finally
                {
                    if (Interlocked.Decrement(ref commentsToProcessCount) == 0)
                    {
                        channel.Writer.Complete();
                    }
                }
            });
        }, ct);

        return totalCommentsCount;
    }
}