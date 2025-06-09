using Application.HackerNews.Dtos;

namespace Application.HackerNews.Interfaces;

public interface IHackerNewsService
{
    Task<IEnumerable<NewsDto>> GetBestNews(int top, CancellationToken ct = default);
}