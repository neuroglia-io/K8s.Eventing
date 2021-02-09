namespace Neuroglia.K8s.Eventing.Kafka
{

    /// <summary>
    /// Exposes constants about the Kafka channel
    /// </summary>
    public static class KafkaDefaults
    {

        /// <summary>
        /// Defines constants for all Kafka CRDs
        /// </summary>
        public static class Resources
        {

            /// <summary>
            /// Gets the default group for all Kafka Cloud Eventing resources
            /// </summary>
            public const string Group = "kafka." + EventingDefaults.Resources.Group;
            /// <summary>
            /// Gets the default version for all Kafka Cloud Eventing resources
            /// </summary>
            public const string Version = "v1alpha1";
            /// <summary>
            /// Gets the default api version for all Kafka Cloud Eventing resources
            /// </summary>
            public static string ApiVersion = string.Join("/", Group, Version);

        }

        /// <summary>
        /// Gets the Kafka channel type
        /// </summary>
        public const string ChannelType = "Kafka";


    }

}
