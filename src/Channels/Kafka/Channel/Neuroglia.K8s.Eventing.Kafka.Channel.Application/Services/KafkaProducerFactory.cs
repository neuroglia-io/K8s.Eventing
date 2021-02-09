using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Neuroglia.K8s.Eventing.Kafka.Channel.Application.Configuration;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IKafkaProducerFactory"/>
    /// </summary>
    public class KafkaProducerFactory
        : IKafkaProducerFactory
    {

        /// <summary>
        /// Initializes a new <see cref="KafkaProducerFactory"/>
        /// </summary>
        /// <param name="options">The service used to access the current <see cref="ApplicationOptions"/></param>
        public KafkaProducerFactory(IOptions<ApplicationOptions> options)
        {
            this.Options = options.Value;
        }

        /// <summary>
        /// Gets the current <see cref="ApplicationOptions"/>
        /// </summary>
        protected ApplicationOptions Options { get; }

        /// <inheritdoc/>
        public virtual IProducer<TKey, TValue> Create<TKey, TValue>()
        {
            ProducerConfig config = new ProducerConfig()
            {
                BootstrapServers = this.Options.Kafka.Servers,
                ClientId = this.Options.Pod
            };
            ProducerBuilder<TKey, TValue> builder = new ProducerBuilder<TKey, TValue>(config);
            return builder.Build();
        }

    }
}
