using k8s.Models;
using Neuroglia.K8s.Eventing.Resources;

namespace Neuroglia.K8s.Eventing.Kafka.Resources
{

    /// <summary>
    /// Represents an instance of a <see cref="V1KafkaChannelDefinition"/>
    /// </summary>
    public class V1KafkaChannelResource
        : V1ChannelResource<V1KafkaChannelSpec, V1ChannelStatus>
    {

        /// <summary>
        /// Initializes a new <see cref="V1KafkaChannelResource"/>
        /// </summary>
        public V1KafkaChannelResource()
            : base(new V1KafkaChannelDefinition())
        {

        }

        /// <summary>
        /// Initializes a new <see cref="V1KafkaChannelResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="V1KafkaChannelResource"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="V1KafkaChannelResource"/>'s <see cref="V1KafkaChannelSpec"/></param>
        public V1KafkaChannelResource(V1ObjectMeta metadata, V1KafkaChannelSpec spec)
            : this()
        {
            this.Metadata = metadata;
            this.Spec = spec;
        }

        /// <summary>
        /// Initializes a new <see cref="V1KafkaChannelResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="V1KafkaChannelResource"/>'s <see cref="V1ObjectMeta"/></param>
        public V1KafkaChannelResource(V1ObjectMeta metadata)
            : this(metadata, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="V1KafkaChannelResource"/>
        /// </summary>
        /// <param name="spec">The <see cref="V1KafkaChannelResource"/>'s <see cref="V1KafkaChannelSpec"/></param>
        public V1KafkaChannelResource(V1KafkaChannelSpec spec)
            : this(null, spec)
        {

        }

    }

}
