using Application.HackerNews.Dtos;
using Application.HackerNews.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/hackernews")]
public class HackerNewsController : Controller
{
    private readonly IHackerNewsService _hackerNewsService;

    public HackerNewsController(IHackerNewsService hackerNewsService) => _hackerNewsService = hackerNewsService;
    
    [HttpGet]
    [SwaggerOperation("Retrieves news from HackerNews API")]
    public async Task<IEnumerable<NewsDto>> GetBestNews(
        [FromQuery, SwaggerParameter("Response items amount is limited by given value. Default value = 10")] 
        int top = 10, 
        CancellationToken ct = default) 
        => await _hackerNewsService.GetBestNews(top, ct);
}