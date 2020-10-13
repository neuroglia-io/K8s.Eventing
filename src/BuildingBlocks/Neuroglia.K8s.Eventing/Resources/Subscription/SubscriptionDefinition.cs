namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the <see cref="CustomResourceDefinition"/> used to configure an event subscription 
    /// </summary>
    public class SubscriptionDefinition
        : CustomResourceDefinition
    {

        /// <summary>
        /// Gets the <see cref="SubscriptionDefinition"/>'s kind
        /// </summary>
        public const string KIND = "Subscription";
        /// <summary>
        /// Gets the <see cref="SubscriptionDefinition"/>'s plural
        /// </summary>
        public const string PLURAL = "subscriptions";

        /// <summary>
        /// Initializes a new <see cref="SubscriptionDefinition"/>
        /// </summary>
        public SubscriptionDefinition()
            : base(EventingDefaults.ApiVersion, KIND, PLURAL)
        {

        }

    }

}
