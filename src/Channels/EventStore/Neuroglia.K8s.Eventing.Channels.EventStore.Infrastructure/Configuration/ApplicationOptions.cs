using CloudNative.CloudEvents;
using EventStore.ClientAPI;
using System;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.Configuration
{

    /// <summary>
    /// Represents the options used to configure the application
    /// </summary>
    public class ApplicationOptions
    {

        /// <summary>
        /// Initializes a new <see cref="ApplicationOptions"/>
        /// </summary>
        public ApplicationOptions()
        {
            this.EventStore = new EventStoreOptions();
        }

        /// <summary>
        /// Gets/sets the <see cref="Uri"/> of the sink <see cref="CloudEvent"/> should be dispatched to
        /// </summary>
        public Uri Sink { get; set; }

        /// <summary>
        /// Gets/sets the options used to configure the <see cref="IEventStoreConnection"/>
        /// </summary>
        public EventStoreOptions EventStore { get; set; }

    }

}
