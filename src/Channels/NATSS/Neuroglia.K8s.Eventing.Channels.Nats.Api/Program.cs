using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Neuroglia.K8s.Eventing.Channels.Nats.Api
{

    /// <summary>
    /// Represents the application's entry point container class
    /// </summary>
    public class Program
    {

        /// <summary>
        /// Represents the application's entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates a new <see cref="IHostBuilder"/>
        /// </summary>
        /// <param name="args">An array of string representing the arguments passed to the application</param>
        /// <returns>A new <see cref="IHostBuilder"/></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

    }

}
