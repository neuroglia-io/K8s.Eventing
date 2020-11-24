using CloudNative.CloudEvents;
using EventStore.ClientAPI;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Neuroglia.AspNetCore.Http;
using Neuroglia.K8s.Eventing.Channels.EventStore.Application.Commands;
using Neuroglia.K8s.Eventing.Channels.EventStore.Application.StartupTasks;
using Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.Configuration;
using Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.Services;
using Neuroglia.Mediation;
using Neuroglia.StartupTasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Api
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
            services.AddMediatR(typeof(SubscribeCommand).Assembly);
            services.AddCommandBehavior(typeof(OperationExceptionHandlingBehavior<,>), typeof(SubscribeCommand).Assembly);
            services.AddHttpClient(typeof(EventChannel).Name, options =>
            {
                options.BaseAddress = this.ApplicationOptions.Sink;
                options.DefaultRequestHeaders.Add(EventingDefaults.Headers.Origin, "natss");
            });
            services.AddSingleton(provider =>
            {
                ConnectionSettingsBuilder settingsBuilder = ConnectionSettings.Create();
                return EventStoreConnection.Create(this.ApplicationOptions.EventStore.ConnectionString, settingsBuilder, "CloudEventsChannel");
            });
            services.AddSingleton<IEventChannel, EventChannel>();
            services.AddStartupTask<EventChannelInitializationTask>();
            services.AddHealthChecks()
                .AddCheck<StartupTasksHealthCheck>("Startup Tasks");
            services.AddIstioHeadersPropagation();
            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, new CloudEventJsonInputFormatter());
            })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
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
