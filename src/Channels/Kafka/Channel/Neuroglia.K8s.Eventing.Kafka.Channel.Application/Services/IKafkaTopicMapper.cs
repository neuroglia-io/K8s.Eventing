using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Resources;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to map Kafka topics
    /// </summary>
    public interface IKafkaTopicMapper
    {

        /// <summary>
        /// Maps the specified <see cref="CloudEvent"/> to a Kafka topic
        /// </summary>
        /// <param name="type">The <see cref="CloudEvent"/> to map</param>
        /// <returns>The mapped Kafka topic</returns>
        string Map(CloudEvent e);

        /// <summary>
        /// Maps the specified <see cref="V1SubscriptionSpec"/> to a Kafka topic
        /// </summary>
        /// <param name="type">The <see cref="V1SubscriptionSpec"/> to map</param>
        /// <returns>The mapped Kafka topic</returns>
        string Map(V1SubscriptionSpec spec);

    }

}
