using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IChannelManager"/> interface
    /// </summary>
    public class ChannelManager
        : IChannelManager
    {

        /// <summary>
        /// Initializes a new <see cref="ChannelManager"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="logger">The service used to perform logging</param>
        public ChannelManager(IServiceProvider serviceProvider, ILogger<ChannelManager> logger)
        {
            this.ServiceProvider = serviceProvider;
            this.Logger = logger;
            this.Channels = new ConcurrentDictionary<string, IChannel>();
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing the registered <see cref="IChannel"/>s
        /// </summary>
        protected ConcurrentDictionary<string, IChannel> Channels { get; private set; }

        /// <inheritdoc/>
        public virtual IChannel RegisterChannel(string name, Uri address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            IChannel channel = ActivatorUtilities.CreateInstance<Channel>(this.ServiceProvider, name, address);
            this.Channels.TryAdd(name, channel);
            return channel;
        }

        /// <inheritdoc/>
        public virtual bool TryGetChannel(string name, out IChannel channel)
        {
            return this.Channels.TryGetValue(name, out channel);
        }

        /// <inheritdoc/>
        public virtual void UnregisterChannel(string name)
        {
            if (this.Channels.TryRemove(name, out IChannel channel))
                channel.Dispose();
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="ChannelManager"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="ChannelManager"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    if(this.Channels != null)
                    {
                        foreach(KeyValuePair<string, IChannel> channelEntry in this.Channels)
                        {
                            channelEntry.Value.Dispose();
                        }
                    }
                }
                this._Disposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

}
