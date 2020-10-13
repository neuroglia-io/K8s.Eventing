using CloudNative.CloudEvents;
using System;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to manage <see cref="CloudEvent"/> <see cref="Channel"/>s
    /// </summary>
    public interface IChannelManager
        : IDisposable
    {

        /// <summary>
        /// Registers a new <see cref="IChannel"/>
        /// </summary>
        /// <param name="name">The name of the <see cref="IChannel"/> to register</param>
        /// <param name="address">The <see cref="IChannel"/>'s remote address</param>
        IChannel RegisterChannel(string name, Uri address);

        /// <summary>
        /// Unregisters the specified <see cref="IChannel"/>
        /// </summary>
        /// <param name="name">The name of the <see cref="IChannel"/> to unregister</param>
        void UnregisterChannel(string name);

        /// <summary>
        /// Attempts to get the <see cref="IChannel"/> with the specified name
        /// </summary>
        /// <param name="name">The name of the channel to get</param>
        /// <param name="channel">The <see cref="IChannel"/> with the specified name</param>
        /// <returns>A boolean indicating whether or not the <see cref="IChannel"/> with the specified name exists</returns>
        bool TryGetChannel(string name, out IChannel channel);

    }

}
