using k8s.Models;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents an instance of a <see cref="SubscriptionDefinition"/>
    /// </summary>
    public class Subscription
        : CustomResource<SubscriptionSpec, SubscriptionStatus>
    {

        /// <summary>
        /// Initializes a new <see cref="Subscription"/>
        /// </summary>
        public Subscription() 
            : base(EventingDefaults.Resources.Subscription)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Subscription"/>
        /// </summary>
        /// <param name="metadata">The <see cref="Subscription"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="Subscription"/>'s <see cref="SubscriptionSpec"/></param>
        /// <param name="status">The <see cref="Subscription"/>'s <see cref="BrokerStatus"/></param>
        public Subscription(V1ObjectMeta metadata, SubscriptionSpec spec, SubscriptionStatus status)
            : this()
        {
            this.Metadata = metadata;
            this.Spec = spec;
            this.Status = status;
        }

        /// <summary>
        /// Initializes a new <see cref="Subscription"/>
        /// </summary>
        /// <param name="metadata">The <see cref="Subscription"/>'s <see cref="V1ObjectMeta"/></param>
        public Subscription(V1ObjectMeta metadata)
            : this(metadata, null, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Subscription"/>
        /// </summary>
        /// <param name="spec">The <see cref="Subscription"/>'s <see cref="SubscriptionSpec"/></param>
        public Subscription(SubscriptionSpec spec)
            : this(null, spec, null)
        {

        }

    }

}
