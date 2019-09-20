using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Ptncafe.ES7
{
    public static class DependencyRegister
    {
        public static ElasticClient ElasticSearchDependencyRegister(
            this IServiceCollection services
            , ConnectionSettings connectionSettings

            )
        {
            //var kafkaConfiguration = configuration.GetSection(nameof(ElasticSearchConfiguration))
            //      .Get<ElasticSearchConfiguration>();

            var settings = connectionSettings;

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
            return client;
        }
    }
}