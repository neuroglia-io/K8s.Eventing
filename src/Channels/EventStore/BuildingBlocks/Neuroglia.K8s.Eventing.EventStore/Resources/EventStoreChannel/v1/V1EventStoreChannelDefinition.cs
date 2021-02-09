namespace Neuroglia.K8s.Eventing.EventStore.Resources
{
    /// <summary>
    /// Represents a <see cref="CustomResourceDefinition"/> used to configure a EventStore cloud event channel
    /// </summary>
    public class V1EventStoreChannelDefinition
        : CustomResourceDefinition
    {

        /// <summary>
        /// Gets the <see cref="V1EventStoreChannelDefinition"/>'s kind
        /// </summary>
        public const string KIND = "EventStoreChannel";
        /// <summary>
        /// Gets the <see cref="V1EventStoreChannelDefinition"/>'s plural
        /// </summary>
        public const string PLURAL = "eventstorechannels";

        /// <summary>
        /// Initializes a new <see cref="V1EventStoreChannelDefinition"/>
        /// </summary>
        public V1EventStoreChannelDefinition()
            : base(EventStoreDefaults.Resources.ApiVersion, KIND, PLURAL)
        {

        }

    }

}
