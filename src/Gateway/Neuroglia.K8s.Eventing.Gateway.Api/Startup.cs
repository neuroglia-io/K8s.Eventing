using CloudNative.CloudEvents;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Neuroglia.AspNetCore.Http;
using Neuroglia.K8s.Eventing.Gateway.Application.Commands;
using Neuroglia.K8s.Eventing.Gateway.Application.StartupTasks;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure.Configuration;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services;
using Neuroglia.StartupTasks;

namespace Neuroglia.K8s.Eventing.Gateway.Api
{

    /// <summary>
    /// Represents the class used to configure the application
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// Initializes a new <see cref="Startup"/>
        /// </summary>
        /// <param name="configuration">The current <see cref="IConfiguration"/></param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.ApplicationOptions = new ApplicationOptions();
            this.Configuration.Bind(this.ApplicationOptions);
        }

        /// <summary>
        /// Gets the current <see cref="IConfiguration"/>
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the current <see cref="Infrastructure.Configuration.ApplicationOptions"/>
        /// </summary>
        protected ApplicationOptions ApplicationOptions { get; }

        /// <summary>
        /// Configures the application's services
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Options.Create(this.ApplicationOptions));
            services.AddKubernetesClient();
            services.AddMediatR(typeof(PublishCloudEventToChannelCommand).Assembly);
            services.AddSingleton<ISubscriptionManager, SubscriptionManager>();
            services.AddSingleton<IChannelManager, ChannelManager>();
            services.AddSingleton<IResourceController, ResourceController>();
            services.AddStartupTask<ResourceControllerInitializationTask>();
            services.AddHttpClient(typeof(Channel).Name);
            services.AddHttpClient<ICloudEventDispatcher, CloudEventDispatcher>();
            services.AddHealthChecks()
                .AddCheck<StartupTasksHealthCheck>("Startup Tasks");
            services.AddIstioHeadersPropagation();
            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, new CloudEventJsonInputFormatter());
            });
        }

        /// <summary>
        /// Configures the application's request pipeline
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to configure</param>
        /// <param name="env">The current <see cref="IWebHostEnvironment"/></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            app.UseHealthChecks("/healthz");
            app.UseStartupTasks();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }

}
