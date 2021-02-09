namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents a <see cref="CustomResourceDefinition"/> used to configure an event broker virtual service
    /// </summary>
    public class V1BrokerDefinition
        : CustomResourceDefinition
    {

        /// <summary>
        /// Gets the <see cref="V1BrokerDefinition"/>'s kind
        /// </summary>
        public const string KIND = "Broker";
        /// <summary>
        /// Gets the <see cref="V1BrokerDefinition"/>'s plural
        /// </summary>
        public const string PLURAL = "brokers";

        /// <summary>
        /// Initializes a new <see cref="V1BrokerDefinition"/>
        /// </summary>
        public V1BrokerDefinition() 
            : base(EventingDefaults.Resources.ApiVersion, KIND, PLURAL)
        {

        }

    }

}
