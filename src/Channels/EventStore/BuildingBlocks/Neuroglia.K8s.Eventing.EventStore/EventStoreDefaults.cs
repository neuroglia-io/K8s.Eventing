namespace Neuroglia.K8s.Eventing.EventStore
{

    /// <summary>
    /// Exposes constants about the EventStore channel
    /// </summary>
    public static class EventStoreDefaults
    {

        /// <summary>
        /// Defines constants for all EventStore CRDs
        /// </summary>
        public static class Resources
        {

            /// <summary>
            /// Gets the default group for all EventStore Cloud Eventing resources
            /// </summary>
            public const string Group = "eventstore." + EventingDefaults.Resources.Group;
            /// <summary>
            /// Gets the default version for all EventStore Cloud Eventing resources
            /// </summary>
            public const string Version = "v1alpha1";
            /// <summary>
            /// Gets the default api version for all EventStore Cloud Eventing resources
            /// </summary>
            public static string ApiVersion = string.Join("/", Group, Version);

        }

        /// <summary>
        /// Gets the EventStore channel type
        /// </summary>
        public const string ChannelType = "EventStore";


    }

}
