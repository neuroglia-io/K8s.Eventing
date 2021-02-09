using k8s.Models;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents an instance of a <see cref="V1SubscriptionDefinition"/>
    /// </summary>
    public class V1SubscriptionResource
        : CustomResource<V1SubscriptionSpec, V1SubscriptionStatus>
    {

        /// <summary>
        /// Initializes a new <see cref="V1SubscriptionResource"/>
        /// </summary>
        public V1SubscriptionResource() 
            : base(EventingDefaults.Resources.Subscription)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="V1SubscriptionResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="V1SubscriptionResource"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="V1SubscriptionResource"/>'s <see cref="V1SubscriptionSpec"/></param>
        /// <param name="status">The <see cref="V1SubscriptionResource"/>'s <see cref="V1BrokerStatus"/></param>
        public V1SubscriptionResource(V1ObjectMeta metadata, V1SubscriptionSpec spec, V1SubscriptionStatus status)
            : this()
        {
            this.Metadata = metadata;
            this.Spec = spec;
            this.Status = status;
        }

        /// <summary>
        /// Initializes a new <see cref="V1SubscriptionResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="V1SubscriptionResource"/>'s <see cref="V1ObjectMeta"/></param>
        public V1SubscriptionResource(V1ObjectMeta metadata)
            : this(metadata, null, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="V1SubscriptionResource"/>
        /// </summary>
        /// <param name="spec">The <see cref="V1SubscriptionResource"/>'s <see cref="V1SubscriptionSpec"/></param>
        public V1SubscriptionResource(V1SubscriptionSpec spec)
            : this(null, spec, null)
        {

        }

    }

}
