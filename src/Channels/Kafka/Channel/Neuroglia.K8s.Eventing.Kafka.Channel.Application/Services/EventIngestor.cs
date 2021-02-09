using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Channels.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IEventIngestor"/> interface
    /// </summary>
    public class EventIngestor
        : IEventIngestor
    {

        /// <summary>
        /// Initializes a new <see cref="EventIngestor"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="producerFactory">The service used to create <see cref="IProducer{TKey, TValue}"/> instances</param>
        /// <param name="topicMapper">The service used to map Kafka topics</param>
        /// <param name="eventFormatter">The service used to format <see cref="CloudEvent"/>s</param>
        public EventIngestor(ILogger<EventIngestor> logger, IKafkaProducerFactory producerFactory, IKafkaTopicMapper topicMapper, ICloudEventFormatter eventFormatter)
        {
            this.Logger = logger;
            this.Kafka = producerFactory.Create<string, byte[]>();
            this.TopicMapper = topicMapper;
            this.EventFormatter = eventFormatter;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to produce Kafka events
        /// </summary>
        protected IProducer<string, byte[]> Kafka { get; }

        /// <summary>
        /// Gets the service used to map Kafka topics
        /// </summary>
        protected IKafkaTopicMapper TopicMapper { get; }

        /// <summary>
        /// Gets the service used to format <see cref="CloudEvent"/>s
        /// </summary>
        protected ICloudEventFormatter EventFormatter { get; }

        /// <inheritdoc/>
        public virtual async Task IngestAsync(CloudEvent e, CancellationToken cancellationToken = default)
        {
            Message<string, byte[]> message = new KafkaCloudEventMessage(e, ContentMode.Structured, this.EventFormatter);
            try
            {
                string topic = this.TopicMapper.Map(e);
                DeliveryResult<string, byte[]> deliveryResult = await this.Kafka.ProduceAsync(topic, message);
                this.Logger.LogInformation("Cloud event ingested by Kafka with topic '{topic}'", topic);
            }
            catch(Exception ex)
            {
                this.Logger.LogError($"An error occured while ingesting a cloud event in Kafka.{Environment.NewLine}Details: {ex.Message}");
                throw;
            }
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="EventIngestor"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="EventIngestor"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    this.Kafka?.Dispose();
                }
                this._Disposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

}
