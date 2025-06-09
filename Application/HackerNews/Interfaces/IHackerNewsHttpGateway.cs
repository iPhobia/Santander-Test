using Application.HackerNews.Dtos;

namespace Application.HackerNews.Interfaces;

public interface IHackerNewsHttpGateway
{
    Task<NewsApiResponse?> GetNewsDetails(int id, CancellationToken ct = default);
    Task<CommentApiResponse?> GetCommentDetails(int id, CancellationToken ct = default);
    IAsyncEnumerable<int> GetBestNewsIds(int take, CancellationToken ct = default);
}