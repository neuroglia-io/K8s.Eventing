using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Api
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
        static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        /// <summary>
        /// Creates a new <see cref="IHostBuilder"/>
        /// </summary>
        /// <param name="args">The command lines arguments</param>
        /// <returns>A new <see cref="IHostBuilder"/></returns>
        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
            
    }
}
