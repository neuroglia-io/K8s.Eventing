using EventStore.ClientAPI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure
{

    /// <summary>
    /// Defines the fundamentals of an object used to wrap an EventStore subscription
    /// </summary>
    public interface ISubscription
        : IDisposable
    {

        /// <summary>
        /// Represents the event fired whenever the <see cref="ISubscription"/> is disposed of
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Gets the <see cref="ISubscription"/>'s id
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the wrapped EventStore subscription
        /// </summary>
        object Source { get; }

        /// <summary>
        /// Sets the wrapped EventStore subscription
        /// </summary>
        /// <param name="source">The wrapped EventStore subscription</param>
        void SetSource(object source);

        /// <summary>
        /// Unsubscribes the <see cref="ISubscription"/>
        /// </summary>
        /// <param name="connection">The <see cref="IEventStoreConnection"/> to unsubscribe from</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task UnsubscribeAsync(IEventStoreConnection connection, CancellationToken cancellationToken = default);

    }

}
