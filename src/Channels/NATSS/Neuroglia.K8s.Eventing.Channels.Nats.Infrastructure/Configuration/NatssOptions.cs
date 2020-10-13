using STAN.Client;

namespace Neuroglia.K8s.Eventing.Channels.Nats.Infrastructure.Configuration
{

    /// <summary>
    /// Represents the options used to configure the application's <see cref="IStanConnection"/>
    /// </summary>
    public class NatssOptions
    {

        /// <summary>
        /// Initializes a new <see cref="NatssOptions"/>
        /// </summary>
        public NatssOptions()
        {
            this.ClusterId = "test-cluster";
            this.ClientId = "natss-channel";
            this.Host = "nats://localhost:4222";
        }

        /// <summary>
        /// Gets/sets the id of the NATS Streaming Cluster to connect to
        /// </summary>
        public string ClusterId { get; set; }

        /// <summary>
        /// Gets/sets the id of the client used to connect to the NATS Streaming server
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets/sets the address of the NATS Streaming server to connect to
        /// </summary>
        public string Host { get; set; }

    }

}
