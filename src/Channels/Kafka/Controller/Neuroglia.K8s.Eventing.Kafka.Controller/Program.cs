using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Kafka.Controller
{

    /// <summary>
    /// Represents the application's entry point
    /// </summary>
    class Program
    {

        /// <summary>
        /// Starts the application
        /// </summary>
        /// <param name="args">The command lines arguments</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        static async Task Main(string[] args)
        {
            using (IHost host = CreateHost(args))
            {
                await host.RunAsync();
            }
        }

        /// <summary>
        /// Creates a new <see cref="IHost"/>
        /// </summary>
        /// <param name="args">The command lines arguments</param>
        /// <returns>A new <see cref="IHost"/></returns>
        static IHost CreateHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    new Startup(context.Configuration, context.HostingEnvironment).ConfigureServices(services);
                })
                .Build();
        }

    }

}
