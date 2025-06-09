using Application.HackerNews.Interfaces;
using Application.HackerNews.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddScoped<IHackerNewsService, HackerNewsService>();
    }
}