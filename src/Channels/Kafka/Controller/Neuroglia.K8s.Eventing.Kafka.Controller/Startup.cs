using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.K8s.Eventing.Kafka.Controller.Services;
using Neuroglia.K8s.Eventing.Kafka.Resources;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Neuroglia.K8s.Eventing.Kafka.Controller
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
        /// <param name="environment">The current <see cref="IHostEnvironment"/></param>
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;
        }

        /// <summary>
        /// Gets the current <see cref="IConfiguration"/>
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the current <see cref="IHostEnvironment"/>
        /// </summary>
        protected IHostEnvironment Environment { get; }

        /// <summary>
        /// Configures the application's services
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddKubernetesClient();
            services.AddSingleton(provider =>
            {
                return new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
            });
            services.AddCustomResourceWatcher<V1KafkaChannelResource>();
            services.AddHostedService<KafkaChannelController>();
        }

    }

}
