using System.Text.Json.Serialization;

namespace Application.HackerNews.Dtos;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ItemType
{
    Story, Comment
}