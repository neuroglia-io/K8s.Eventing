using System;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Defines the fundamentals of an <see cref="IV1ChannelResource{TSpec, TStatus}"/> status
    /// </summary>
    public interface IV1ChannelStatus
    {

        /// <summary>
        /// Gets/sets a boolean indicating whether or not the channel is operational
        /// </summary>
        bool? Operational { get; set; }

        /// <summary>
        /// Gets/sets the channel's address
        /// </summary>
        Uri Address { get; set; }

        /// <summary>
        /// gets/sets the channel's endpoints
        /// </summary>
        IV1ChannelEndpoints Endpoints { get; }

    }

}
