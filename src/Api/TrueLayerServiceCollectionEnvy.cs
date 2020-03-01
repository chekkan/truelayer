using System;
using Api.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api
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