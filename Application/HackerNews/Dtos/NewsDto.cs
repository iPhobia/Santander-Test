namespace Application.HackerNews.Dtos;

public record NewsDto(string Title, Uri Uri, string PostedBy, DateTimeOffset Time, int Score, int CommentCount)
{
    public static NewsDto MapFrom(NewsApiResponse obj, int commentsCount)
    {
        return new NewsDto(obj.Title, obj.Url, obj.By, DateTimeOffset.FromUnixTimeSeconds(obj.Time), obj.Score, commentsCount);
    }
}