using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Neuroglia.K8s.Eventing.Kafka.Channel.Application.Configuration;
using System;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IKafkaConsumerFactory"/>
    /// </summary>
    public class KafkaConsumerFactory
        : IKafkaConsumerFactory
    {

        /// <summary>
        /// Initializes a new <see cref="KafkaConsumerFactory"/>
        /// </summary>
        /// <param name="options">The service used to access the current <see cref="ApplicationOptions"/></param>
        public KafkaConsumerFactory(IOptions<ApplicationOptions> options)
        {
            this.Options = options.Value;
        }

        /// <summary>
        /// Gets the current <see cref="ApplicationOptions"/>
        /// </summary>
        protected ApplicationOptions Options { get; }

        /// <inheritdoc/>
        public virtual IConsumer<TKey, TValue> Create<TKey, TValue>(Action<ConsumerConfig> configuration)
        {
            ConsumerConfig config = new ConsumerConfig()
            {
                BootstrapServers = this.Options.Kafka.Servers,
                ClientId = this.Options.Pod
            };
            configuration(config);
            ConsumerBuilder<TKey, TValue> builder = new ConsumerBuilder<TKey, TValue>(config);
            return builder.Build();
        }

    }

}
