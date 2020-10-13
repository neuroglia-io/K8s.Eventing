using k8s.Models;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents an instance of a <see cref="ChannelDefinition"/>
    /// </summary>
    public class Channel
        : CustomResource<ChannelSpec>
    {

        /// <summary>
        /// Initializes a new <see cref="Channel"/>
        /// </summary>
        public Channel() 
            : base(EventingDefaults.Resources.Channel)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Channel"/>
        /// </summary>
        /// <param name="metadata">The <see cref="Channel"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="Channel"/>'s <see cref="ChannelSpec"/></param>
        public Channel(V1ObjectMeta metadata, ChannelSpec spec)
            : this()
        {
            this.Metadata = metadata;
            this.Spec = spec;
        }

        /// <summary>
        /// Initializes a new <see cref="Channel"/>
        /// </summary>
        /// <param name="metadata">The <see cref="Channel"/>'s <see cref="V1ObjectMeta"/></param>
        public Channel(V1ObjectMeta metadata)
            : this(metadata, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Channel"/>
        /// </summary>
        /// <param name="spec">The <see cref="Channel"/>'s <see cref="ChannelSpec"/></param>
        public Channel(ChannelSpec spec)
            : this(null, spec)
        {

        }

    }

}
