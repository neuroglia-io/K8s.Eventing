namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Configuration
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
            this.Channel = "kafka";
            this.Kafka = new KafkaOptions();
        }

        /// <summary>
        /// Gets/sets the channel's name
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Gets/sets the name of the application's pod. Set by Kubernetes.
        /// </summary>
        public string Pod { get; set; }

        /// <summary>
        /// Gets/sets the options used to configure Kafka
        /// </summary>
        public KafkaOptions Kafka { get; set; }

    }

}
