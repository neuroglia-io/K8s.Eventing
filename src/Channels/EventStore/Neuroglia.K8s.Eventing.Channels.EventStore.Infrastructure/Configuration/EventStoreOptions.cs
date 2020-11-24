using EventStore.ClientAPI;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.Configuration
{
    /// <summary>
    /// Represents the options used to configure the application's <see cref="IEventStoreConnection"/>
    /// </summary>
    public class EventStoreOptions
    {

        /// <summary>
        /// Initializes a new <see cref="EventStore"/>
        /// </summary>
        public EventStoreOptions()
        {
            this.DisableTls = true;
            this.Host = "eventstore";
            this.TcpPort = 1113;
            this.HttpPort = 2113;
            this.Username = "admin";
            this.Password = "changeit";
        }

        /// <summary>
        /// Gets/sets the remote <see cref="IEventStoreConnection"/>'s host. Defaults to 'eventstore'.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets/sets the remote <see cref="IEventStoreConnection"/>'s TCP port. Defaults to '1113'.
        /// </summary>
        public int TcpPort { get; set; }

        /// <summary>
        /// Gets/sets the remote <see cref="IEventStoreConnection"/>'s HTTP port. Defaults to '2113'.
        /// </summary>
        public int HttpPort { get; set; }

        /// <summary>
        /// Gets/sets a boolean indicating whether or not to disable TLS when connecting to the remote <see cref="IEventStoreConnection"/>. Defaults to 'True'.
        /// </summary>
        public bool DisableTls { get; set; }

        /// <summary>
        /// Gets the connection string used to connect to the remote <see cref="IEventStoreConnection"/>.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return $"ConnectTo=tcp://{this.Username}:{this.Password}@{this.Host}:{this.TcpPort}; DefaultUserCredentials={this.Username}:{this.Password};";
            }
        }

        /// <summary>
        /// Gets/sets the default username to use when connecting to the remote <see cref="IEventStoreConnection"/>. Defaults to 'admin'.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets/sets the default password to use when connecting to the remote <see cref="IEventStoreConnection"/>. Defaults to 'changeit'.
        /// </summary>
        public string Password { get; set; }

    }

}
