using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.K8s.Eventing.Channels.Controllers;
using Neuroglia.K8s.Eventing.Kafka.Channel.Application;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Api
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
        /// <param name="environment">The current <see cref="IWebHostEnvironment"/></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;
        }

        /// <summary>
        /// Gets the current <see cref="IConfiguration"/>
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the current <see cref="IWebHostEnvironment"/>
        /// </summary>
        protected IWebHostEnvironment Environment { get; }

        /// <summary>
        /// Configures the application's services
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, new CloudEventJsonInputFormatter());
            })
                .ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(new AssemblyPart(typeof(EventsController).Assembly)));
            services.AddApplicationServices(this.Configuration);
        }

        /// <summary>
        /// Configures the application
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to configure</param>
        public void Configure(IApplicationBuilder app)
        {
            if (this.Environment.IsDevelopment())
                app.UseDeveloperExceptionPage();
            app.UseHealthChecks("/healthz");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
