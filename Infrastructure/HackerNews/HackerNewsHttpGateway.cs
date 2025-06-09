using System.Net.Http.Json;
using Application.HackerNews.Dtos;
using Application.HackerNews.Interfaces;

namespace Infrastructure.HackerNews;

public class HackerNewsHttpGateway(HttpClient httpClient) 
    : IHackerNewsHttpGateway
{
    public async Task<NewsApiResponse?> GetNewsDetails(int id, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<NewsApiResponse>($"item/{id}.json", ct);
    }

    public async Task<CommentApiResponse?> GetCommentDetails(int id, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<CommentApiResponse>($"item/{id}.json", ct);
    }

    public IAsyncEnumerable<int> GetBestNewsIds(int take, CancellationToken ct = default)
    {
        return httpClient
            .GetFromJsonAsAsyncEnumerable<int>("beststories.json", ct)
            .Take(take);
    }
}