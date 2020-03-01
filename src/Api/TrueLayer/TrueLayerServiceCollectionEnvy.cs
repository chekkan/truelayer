using Api.Application;
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
            services.AddHttpClient<ITrueLayerDataApiClient, TrueLayerDataApiHttpClient>();
            services.AddHttpClient<ITransactionReader, TrueLayerDataApiHttpClient>();
            services.AddHttpClient<IAccountReader, TrueLayerDataApiHttpClient>();
            return services;
        }
    }
}