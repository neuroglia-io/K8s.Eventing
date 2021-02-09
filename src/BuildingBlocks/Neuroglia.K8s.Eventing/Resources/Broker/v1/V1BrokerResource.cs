using k8s.Models;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents an instance of a <see cref="V1BrokerDefinition"/>
    /// </summary>
    public class V1BrokerResource
        : CustomResource<V1BrokerSpec, V1BrokerStatus>
    {

        /// <summary>
        /// Initializes a new <see cref="V1BrokerResource"/>
        /// </summary>
        public V1BrokerResource() 
            : base(EventingDefaults.Resources.Broker)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="V1BrokerResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="V1BrokerResource"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="V1BrokerResource"/>'s <see cref="V1BrokerSpec"/></param>
        /// <param name="status">The <see cref="V1BrokerResource"/>'s <see cref="V1BrokerStatus"/></param>
        public V1BrokerResource(V1ObjectMeta metadata, V1BrokerSpec spec, V1BrokerStatus status)
            : this()
        {
            this.Metadata = metadata;
            this.Spec = spec;
            this.Status = status;
        }

        /// <summary>
        /// Initializes a new <see cref="V1BrokerResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="V1BrokerResource"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="V1BrokerResource"/>'s <see cref="V1BrokerSpec"/></param>
        public V1BrokerResource(V1ObjectMeta metadata, V1BrokerSpec spec)
            : this(metadata, spec, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="V1BrokerResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="V1BrokerResource"/>'s <see cref="V1ObjectMeta"/></param>
        public V1BrokerResource(V1ObjectMeta metadata)
            : this(metadata, null, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="V1BrokerResource"/>
        /// </summary>
        /// <param name="spec">The <see cref="V1BrokerResource"/>'s <see cref="V1BrokerSpec"/></param>
        public V1BrokerResource(V1BrokerSpec spec)
            : this(null, spec, null)
        {

        }

    }

}
