namespace Neuroglia.K8s.Eventing.Kafka.Resources
{
    /// <summary>
    /// Represents a <see cref="CustomResourceDefinition"/> used to configure a Kafka cloud event channel
    /// </summary>
    public class V1KafkaChannelDefinition
        : CustomResourceDefinition
    {

        /// <summary>
        /// Gets the <see cref="V1KafkaChannelDefinition"/>'s kind
        /// </summary>
        public const string KIND = "KafkaChannel";
        /// <summary>
        /// Gets the <see cref="V1KafkaChannelDefinition"/>'s plural
        /// </summary>
        public const string PLURAL = "kafkachannels";

        /// <summary>
        /// Initializes a new <see cref="V1KafkaChannelDefinition"/>
        /// </summary>
        public V1KafkaChannelDefinition()
            : base(KafkaDefaults.Resources.ApiVersion, KIND, PLURAL)
        {

        }

    }

}
