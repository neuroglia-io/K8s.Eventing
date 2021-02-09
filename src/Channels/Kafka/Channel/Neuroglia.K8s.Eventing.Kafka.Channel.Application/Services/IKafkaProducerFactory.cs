using Confluent.Kafka;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services
{
    /// <summary>
    /// Defines the fundamentals of a service used to create <see cref="IProducer{TKey, TValue}"/> instances
    /// </summary>
    public interface IKafkaProducerFactory
    {

        /// <summary>
        /// Creates a new <see cref="IProducer{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="TKey">The type of key of messages to produce</typeparam>
        /// <typeparam name="TValue">The type of value of messages to produce/typeparam>
        /// <returns>A new <see cref="IConsumer{TKey, TValue}"/></returns>
        IProducer<TKey, TValue> Create<TKey, TValue>();

    }

}
