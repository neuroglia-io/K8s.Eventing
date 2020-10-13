using k8s.Models;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents an instance of a <see cref="BrokerDefinition"/>
    /// </summary>
    public class Broker
        : CustomResource<BrokerSpec, BrokerStatus>
    {

        /// <summary>
        /// Initializes a new <see cref="Broker"/>
        /// </summary>
        public Broker() 
            : base(EventingDefaults.Resources.Broker)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Broker"/>
        /// </summary>
        /// <param name="metadata">The <see cref="Broker"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="Broker"/>'s <see cref="BrokerSpec"/></param>
        /// <param name="status">The <see cref="Broker"/>'s <see cref="BrokerStatus"/></param>
        public Broker(V1ObjectMeta metadata, BrokerSpec spec, BrokerStatus status)
            : this()
        {
            this.Metadata = metadata;
            this.Spec = spec;
            this.Status = status;
        }

        /// <summary>
        /// Initializes a new <see cref="Broker"/>
        /// </summary>
        /// <param name="metadata">The <see cref="Broker"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="Broker"/>'s <see cref="BrokerSpec"/></param>
        public Broker(V1ObjectMeta metadata, BrokerSpec spec)
            : this(metadata, spec, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Broker"/>
        /// </summary>
        /// <param name="metadata">The <see cref="Broker"/>'s <see cref="V1ObjectMeta"/></param>
        public Broker(V1ObjectMeta metadata)
            : this(metadata, null, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Broker"/>
        /// </summary>
        /// <param name="spec">The <see cref="Broker"/>'s <see cref="BrokerSpec"/></param>
        public Broker(BrokerSpec spec)
            : this(null, spec, null)
        {

        }

    }

}
