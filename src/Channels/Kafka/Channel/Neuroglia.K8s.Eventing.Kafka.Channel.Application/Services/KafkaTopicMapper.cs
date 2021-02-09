using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Resources;
using System;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IKafkaTopicMapper"/> interface
    /// </summary>
    public class KafkaTopicMapper
        : IKafkaTopicMapper
    {

        /// <summary>
        /// Gets the prefix for all mapped Kafka topics
        /// </summary>
        public const string TopicPrefix = "cloudevent-";

        /// <inheritdoc/>
        public virtual string Map(CloudEvent e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            return this.Map(this.Sanitize(e.Type));
        }

        /// <inheritdoc/>
        public virtual string Map(V1SubscriptionSpec spec)
        {
            string topic;
            if (spec.Filter != null
                && spec.Filter.Attributes != null
                && spec.Filter.Attributes.TryGetValue(CloudEventAttributes.TypeAttributeName(), out string type))
            {
                type = this.Sanitize(type);
                if (type.Contains('*'))
                    topic = $"^{TopicPrefix}{type}";
                else
                    topic = this.Map(type);
            }  
            else
            {
                topic = $"^{TopicPrefix}*";
            }
            return topic;
        }

        /// <summary>
        /// Maps the specified cloud event type to a Kafka topic
        /// </summary>
        /// <param name="type">The cloud event type to map</param>
        /// <returns>The mapped Kafka topic</returns>
        protected virtual string Map(string type)
        {
            if (string.IsNullOrWhiteSpace(nameof(type)))
                throw new ArgumentNullException(nameof(type));
            return $"{TopicPrefix}{type}";
        }

        /// <summary>
        /// Sanitizes the specified string to make it Kafka-topic compatible
        /// </summary>
        /// <param name="value">The value to sanitize</param>
        /// <returns>The sanitized value</returns>
        protected string Sanitize(string value)
        {
            if (value.Contains('/'))
                value = value.Replace('/', '-');
            return value;
        }

    }

}
