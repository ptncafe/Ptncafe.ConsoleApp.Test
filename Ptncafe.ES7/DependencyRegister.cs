using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Ptncafe.ES7.Model;
using System;

namespace Ptncafe.ES7
{
    public static class DependencyRegister
    {
        public static void ElasticSearchDependencyRegister(
            this IServiceCollection services, IConfiguration configuration)
        {
            var kafkaConfiguration = configuration.GetSection(nameof(ElasticSearchConfiguration))
                  .Get<ElasticSearchConfiguration>();


            var settings = new ConnectionSettings(new Uri(kafkaConfiguration.Url))
                .DefaultIndex(kafkaConfiguration.DefaultIndex);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
