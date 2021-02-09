namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the <see cref="CustomResourceDefinition"/> used to configure an event type 
    /// </summary>
    public class EventTypeDefinition
        : CustomResourceDefinition
    {

        /// <summary>
        /// Gets the <see cref="EventTypeDefinition"/>'s kind
        /// </summary>
        public const string KIND = "EventType";
        /// <summary>
        /// Gets the <see cref="EventTypeDefinition"/>'s plural
        /// </summary>
        public const string PLURAL = "eventtypes";

        /// <summary>
        /// Initializes a new <see cref="V1SubscriptionDefinition"/>
        /// </summary>
        public EventTypeDefinition()
            : base(EventingDefaults.Resources.ApiVersion, KIND, PLURAL)
        {

        }

    }

}
