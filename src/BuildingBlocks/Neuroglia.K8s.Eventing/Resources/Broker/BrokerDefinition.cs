namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents a <see cref="CustomResourceDefinition"/> used to configure an event broker virtual service
    /// </summary>
    public class BrokerDefinition
        : CustomResourceDefinition
    {

        /// <summary>
        /// Gets the <see cref="BrokerDefinition"/>'s kind
        /// </summary>
        public const string KIND = "Broker";
        /// <summary>
        /// Gets the <see cref="BrokerDefinition"/>'s plural
        /// </summary>
        public const string PLURAL = "brokers";

        /// <summary>
        /// Initializes a new <see cref="BrokerDefinition"/>
        /// </summary>
        public BrokerDefinition() 
            : base(EventingDefaults.ApiVersion, KIND, PLURAL)
        {

        }

    }

}
