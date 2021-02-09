namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Configuration
{

    /// <summary>
    /// Represents the options used to configure Kafka
    /// </summary>
    public class KafkaOptions
    {

        /// <summary>
        /// Initializes a new <see cref="KafkaOptions"/>
        /// </summary>
        public KafkaOptions()
        {
            this.Servers = "kafka-broker:9092";
            this.DefaultConsumerGroupId = "cloudevent-channels";
        }

        /// <summary>
        /// Gets/sets the default Kafka consumer group id
        /// </summary>
        public string DefaultConsumerGroupId { get; set; }

        /// <summary>
        /// Gets/sets a CSV list of the Kafka brokers to connect to
        /// </summary>
        public string Servers { get; set; }

    }

}
