using k8s.Models;
using Neuroglia.K8s.Eventing.Resources;

namespace Neuroglia.K8s.Eventing.EventStore.Resources
{

    /// <summary>
    /// Represents an instance of a <see cref="V1EventStoreChannelDefinition"/>
    /// </summary>
    public class V1EventStoreChannelResource
        : V1ChannelResource<V1EventStoreChannelSpec, V1ChannelStatus>
    {

        /// <summary>
        /// Initializes a new <see cref="V1EventStoreChannelResource"/>
        /// </summary>
        public V1EventStoreChannelResource()
            : base(new V1EventStoreChannelDefinition())
        {

        }

        /// <summary>
        /// Initializes a new <see cref="V1EventStoreChannelResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="V1EventStoreChannelResource"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="V1EventStoreChannelResource"/>'s <see cref="V1EventStoreChannelSpec"/></param>
        public V1EventStoreChannelResource(V1ObjectMeta metadata, V1EventStoreChannelSpec spec)
            : this()
        {
            this.Metadata = metadata;
            this.Spec = spec;
        }

        /// <summary>
        /// Initializes a new <see cref="V1EventStoreChannelResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="V1EventStoreChannelResource"/>'s <see cref="V1ObjectMeta"/></param>
        public V1EventStoreChannelResource(V1ObjectMeta metadata)
            : this(metadata, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="V1EventStoreChannelResource"/>
        /// </summary>
        /// <param name="spec">The <see cref="V1EventStoreChannelResource"/>'s <see cref="V1EventStoreChannelSpec"/></param>
        public V1EventStoreChannelResource(V1EventStoreChannelSpec spec)
            : this(null, spec)
        {

        }

    }

}
