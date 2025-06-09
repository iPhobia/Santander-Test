namespace Application.HackerNews.Dtos;

public record CommentApiResponse(int Id, string By, int[] Kids, int Parent, string Text, int Time, ItemType Type);