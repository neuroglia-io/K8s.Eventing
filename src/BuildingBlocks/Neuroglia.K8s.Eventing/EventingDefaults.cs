using Neuroglia.K8s.Eventing.Resources;

namespace Neuroglia.K8s.Eventing
{

    /// <summary>
    /// Exposes constants about Neuroglia Kubernetes Eventing
    /// </summary>
    public static class EventingDefaults
    {

        /// <summary>
        /// Exposes constants about Neuroglia Kubernetes CRDs
        /// </summary>
        public static class Resources
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
            /// Gets the 'Broker' CRD
            /// </summary>
            public static V1BrokerDefinition Broker = new V1BrokerDefinition();
            /// <summary>
            /// Gets the 'EventType' CRD
            /// </summary>
            public static EventTypeDefinition EventType = new EventTypeDefinition();
            /// <summary>
            /// Gets the 'Subscription' CRD
            /// </summary>
            public static V1SubscriptionDefinition Subscription = new V1SubscriptionDefinition();

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

    }

}
