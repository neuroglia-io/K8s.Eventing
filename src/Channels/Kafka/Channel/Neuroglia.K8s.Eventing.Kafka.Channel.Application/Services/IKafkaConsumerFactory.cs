using Confluent.Kafka;
using System;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to create <see cref="IConsumer{TKey, TValue}"/> instances
    /// </summary>
    public interface IKafkaConsumerFactory
    {

        /// <summary>
        /// Creates a new <see cref="IConsumer{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="TKey">The type of key of messages to consume</typeparam>
        /// <typeparam name="TValue">The type of value of messages to consume/typeparam>
        /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IConsumer{TKey, TValue}"/> to create</param>
        /// <returns>A new <see cref="IConsumer{TKey, TValue}"/></returns>
        IConsumer<TKey, TValue> Create<TKey, TValue>(Action<ConsumerConfig> configuration);

    }

}
