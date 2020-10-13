using CloudNative.CloudEvents;
using System;
using System.Collections.Generic;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure
{

    /// <summary>
    /// Defines the fundamentals of an eventing subscription
    /// </summary>
    public interface ISubscription
    {

        /// <summary>
        /// Gets the <see cref="ISubscription"/>'s id
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets/sets <see cref="CloudEvent"/> subject the <see cref="ISubscription"/> applies to
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// Gets/sets <see cref="CloudEvent"/> type the <see cref="ISubscription"/> applies to
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Gets/sets <see cref="CloudEvent"/> source the <see cref="ISubscription"/> applies to
        /// </summary>
        Uri Source { get; set; }

        /// <summary>
        /// Gets/sets the name of the channel the <see cref="ISubscription"/> applies to
        /// </summary>
        string ChannelName { get; set; }

        /// <summary>
        /// Gets a boolean indicating whether or not the <see cref="ISubscription"/> is bound to an <see cref="IChannel"/>, meaning that it has been explicitly created on the latter
        /// </summary>
        bool IsChannelBound { get; }

        /// <summary>
        /// Gets/sets an <see cref="IList{T}"/> containing the addresses of all the subscribers bound to the <see cref="ISubscription"/>
        /// </summary>
        IList<Uri> Subscribers { get; set; }

    }

}
