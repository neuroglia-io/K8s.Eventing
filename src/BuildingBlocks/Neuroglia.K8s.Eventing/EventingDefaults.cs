using Neuroglia.K8s.Eventing.Resources;

namespace Neuroglia.K8s.Eventing
{

    /// <summary>
    /// Exposes constants about Neuroglia Kubernetes Eventing
    /// </summary>
    public static class EventingDefaults
    {

        /// <summary>
        /// Gets the default group for all Neuroglia Eventing resources
        /// </summary>
        public const string Group = "eventing.k8s.neuroglia.io";
        /// <summary>
        /// Gets the default version for all Neuroglia Eventing resources
        /// </summary>
        public const string Version = "v1alpha1";

        /// <summary>
        /// Gets the default api version for all Neuroglia Eventing resources
        /// </summary>
        public static string ApiVersion = string.Join("/", Group, Version);

        /// <summary>
        /// Exposes constants about Neuroglia Kubernetes Eventing labels
        /// </summary>
        public static class Labels
        {

            private const string Prefix = "neuroglia-eventing-";

            /// <summary>
            /// Gets the label assigned to all channels
            /// </summary>
            public const string Channel = Prefix + "channel";

        }

        /// <summary>
        /// Exposes constants about Neuroglia Kubernetes Eventing default headers
        /// </summary>
        public static class Headers
        {

            private const string Prefix = "x-eventing-";
            /// <summary>
            /// Gets the http header used to store the channel a cloud event is intended for
            /// </summary>
            public const string Channel = Prefix + "channel";
            /// <summary>
            /// Gets the http header used to indicate the origin of a cloud event
            /// </summary>
            public const string Origin = Prefix + "origin";

        }

        /// <summary>
        /// Exposes constants about default Neuroglia Kubernetes Eventing resources
        /// </summary>
        public static class Resources
        {

            /// <summary>
            /// Gets the 'Broker' CRD
            /// </summary>
            public static BrokerDefinition Broker = new BrokerDefinition();
            /// <summary>
            /// Gets the 'Channel' CRD
            /// </summary>
            public static ChannelDefinition Channel = new ChannelDefinition();
            /// <summary>
            /// Gets the 'Subscription' CRD
            /// </summary>
            public static SubscriptionDefinition Subscription = new SubscriptionDefinition();

        }

    }

}
