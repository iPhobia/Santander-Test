using Application.HackerNews.Interfaces;
using Infrastructure.HackerNews;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IHackerNewsHttpGateway, HackerNewsHttpGateway>(c =>
        {
            var baseUrl = configuration.GetValue<string>("HackerNewsGateway:ApiV0BaseUrl");

            if (string.IsNullOrEmpty(baseUrl))
                throw new ApplicationException("HackerNews base url is not configured");
            
            c.BaseAddress = new Uri(baseUrl);
        });
    }
}