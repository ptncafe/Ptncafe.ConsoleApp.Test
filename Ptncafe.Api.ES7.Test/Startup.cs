using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ptncafe.ES7;
using Ptncafe.ES7.Model;

namespace Ptncafe.Api.ES7.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var kafkaConfiguration = Configuration.GetSection(nameof(ElasticSearchConfiguration))
                  .Get<ElasticSearchConfiguration>();
            var settings = new Nest.ConnectionSettings(new Uri(kafkaConfiguration.Url))
             .DefaultIndex(kafkaConfiguration.DefaultIndex);

            var esClient = services.ElasticSearchDependencyRegister(settings);
            esClient.Indices.Create("testindex", c => c.Map<Model.TestModel>(m => m.AutoMap()));


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

    }
}
