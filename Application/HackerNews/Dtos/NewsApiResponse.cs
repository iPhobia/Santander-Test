namespace Application.HackerNews.Dtos;

public record NewsApiResponse(int Id, string By, int Descendants, int[] Kids, int Time, 
    string Title, ItemType Type, Uri Url, int Score);
