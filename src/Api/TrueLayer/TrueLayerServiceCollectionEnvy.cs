using System;
using Api.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.TrueLayer
{
    public static class TrueLayerServiceCollectionEnvy
    {
        public static IServiceCollection AddTrueLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TrueLayerSettings>(configuration.GetSection("TrueLayer"));
            services.AddHttpClient<ITrueLayerDataApiClient, TrueLayerDataApiHttpClient>((provider, client) =>
            {
                client.BaseAddress = new Uri("https://api.truelayer.com/data/v1");
            });
            return services;
        }
    }
}