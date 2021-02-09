namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the base class for all <see cref="IV1ChannelResource{TSpec, TStatus}"/> implementations
    /// </summary>
    public abstract class V1ChannelResource<TSpec, TStatus>
        : CustomResource<TSpec, TStatus>, IV1ChannelResource<TSpec, TStatus>
        where TSpec : class, IV1ChannelSpec, new()
        where TStatus : class, IV1ChannelStatus, new()
    {

        /// <summary>
        /// Initializes a new <see cref="V1ChannelResource{TSpec, TStatus}"/>
        /// </summary>
        /// <param name="definition">The <see cref="V1ChannelResource{TSpec, TStatus}"/>'s <see cref="ICustomResourceDefinition"/></param>
        protected V1ChannelResource(ICustomResourceDefinition definition) 
            : base(definition)
        {

        }

    }

    /// <summary>
    /// Represents a generic <see cref="V1ChannelResource{TSpec, TStatus}"/> implementation
    /// </summary>
    public class V1ChannelResource
        : CustomResource<V1ChannelSpec, V1ChannelStatus>
    {

        /// <summary>
        /// Initializes a new <see cref="V1ChannelResource"/>
        /// </summary>
        /// <param name="definition">The <see cref="V1ChannelResource"/>'s <see cref="ICustomResourceDefinition"/></param>
        public V1ChannelResource(ICustomResourceDefinition definition) 
            : base(definition)
        {

        }

    }

}
