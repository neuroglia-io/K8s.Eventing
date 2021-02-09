using k8s.Models;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents an instance of an <see cref="EventTypeSpec"/>
    /// </summary>
    public class EventTypeResource
        : CustomResource<EventTypeSpec>
    {

        /// <summary>
        /// Initializes a new <see cref="EventTypeResource"/>
        /// </summary>
        public EventTypeResource()
            : base(EventingDefaults.Resources.EventType)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="EventTypeResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="EventTypeResource"/>'s <see cref="V1ObjectMeta"/></param>
        /// <param name="spec">The <see cref="EventTypeResource"/>'s <see cref="EventTypeSpec"/></param>
        public EventTypeResource(V1ObjectMeta metadata, EventTypeSpec spec)
            : this()
        {
            this.Metadata = metadata;
            this.Spec = spec;
        }

        /// <summary>
        /// Initializes a new <see cref="EventTypeResource"/>
        /// </summary>
        /// <param name="metadata">The <see cref="EventTypeResource"/>'s <see cref="V1ObjectMeta"/></param>
        public EventTypeResource(V1ObjectMeta metadata)
            : this(metadata, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="EventTypeResource"/>
        /// </summary>
        /// <param name="spec">The <see cref="EventTypeResource"/>'s <see cref="EventTypeSpec"/></param>
        public EventTypeResource(EventTypeSpec spec)
            : this(null, spec)
        {

        }

    }

}
