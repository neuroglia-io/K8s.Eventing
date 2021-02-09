namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the <see cref="CustomResourceDefinition"/> used to configure an event subscription 
    /// </summary>
    public class V1SubscriptionDefinition
        : CustomResourceDefinition
    {

        /// <summary>
        /// Gets the <see cref="V1SubscriptionDefinition"/>'s kind
        /// </summary>
        public const string KIND = "Subscription";
        /// <summary>
        /// Gets the <see cref="V1SubscriptionDefinition"/>'s plural
        /// </summary>
        public const string PLURAL = "subscriptions";

        /// <summary>
        /// Initializes a new <see cref="V1SubscriptionDefinition"/>
        /// </summary>
        public V1SubscriptionDefinition()
            : base(EventingDefaults.Resources.ApiVersion, KIND, PLURAL)
        {

        }

    }

}
