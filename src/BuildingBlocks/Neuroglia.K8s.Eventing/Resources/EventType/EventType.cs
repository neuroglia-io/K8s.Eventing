using k8s.Models;

namespace Neuroglia.K8s.Eventing.Resources
{
    /// <summary>
    /// Represents an instance of an <see cref="EventTypeSpec"/>
    /// </summary>
    public class EventType
        : CustomResource<EventTypeSpec>
    {

        /// <summary>
        /// Initializes a new <see cref="EventType"/>
        /// </summary>
        public EventType()
            : base(EventingDefaults.Resources.EventType)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="EventType"/>
        /// </summary>
        /// <param name="metadata">The <see cref="EventType"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="EventType"/>'s <see cref="EventTypeSpec"/></param>
        public EventType(V1ObjectMeta metadata, EventTypeSpec spec)
            : this()
        {
            this.Metadata = metadata;
            this.Spec = spec;
        }

        /// <summary>
        /// Initializes a new <see cref="EventType"/>
        /// </summary>
        /// <param name="metadata">The <see cref="EventType"/>'s <see cref="V1ObjectMeta"/></param>
        public EventType(V1ObjectMeta metadata)
            : this(metadata, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="EventType"/>
        /// </summary>
        /// <param name="spec">The <see cref="EventType"/>'s <see cref="EventTypeSpec"/></param>
        public EventType(EventTypeSpec spec)
            : this(null, spec)
        {

        }

    }

}
