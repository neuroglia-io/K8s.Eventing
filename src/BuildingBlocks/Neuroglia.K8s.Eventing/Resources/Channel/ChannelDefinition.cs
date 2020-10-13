namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents a <see cref="CustomResourceDefinition"/> used to configure an event channel
    /// </summary>
    public class ChannelDefinition
        : CustomResourceDefinition
    {

        /// <summary>
        /// Gets the <see cref="ChannelDefinition"/>'s kind
        /// </summary>
        public const string KIND = "Channel";
        /// <summary>
        /// Gets the <see cref="ChannelDefinition"/>'s plural
        /// </summary>
        public const string PLURAL = "channels";

        /// <summary>
        /// Initializes a new <see cref="ChannelDefinition"/>
        /// </summary>
        public ChannelDefinition()
            : base(EventingDefaults.ApiVersion, KIND, PLURAL)
        {

        }

    }

}
