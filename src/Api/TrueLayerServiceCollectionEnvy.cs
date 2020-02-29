using System;
using Api.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public static class TrueLayerServiceCollectionEnvy
    {
        public static IServiceCollection AddTrueLayer(this IServiceCollection services)
        {
            services.AddHttpClient<ITrueLayerDataApiClient, TrueLayerDataApiHttpClient>((provider, client) =>
            {
                client.BaseAddress = new Uri("https://api.truelayer.com/data/v1");
            });
            return services;
        }
    }
}