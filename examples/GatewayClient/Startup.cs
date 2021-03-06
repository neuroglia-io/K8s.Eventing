using CloudNative.CloudEvents;
using GatewayClient.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.AspNetCore.Http;
using System;

namespace GatewayClient
{

    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<IEventBus, EventBus>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("BROKER_URI"), UriKind.Absolute);
            });
            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, new CloudEventJsonInputFormatter());
            })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });
            services.AddIstioHeadersPropagation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }

}
