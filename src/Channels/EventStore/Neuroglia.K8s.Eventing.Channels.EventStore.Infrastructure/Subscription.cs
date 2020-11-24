using EventStore.ClientAPI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure
{

    /// <summary>
    /// Represents the base class for all EventStore subscription wrappers
    /// </summary>
    public abstract class Subscription
        : ISubscription
    {

        /// <inheritdoc/>
        public event EventHandler Disposed;

        /// <summary>
        /// Initializes a new <see cref="Subscription"/>
        /// </summary>
        /// <param name="id">The id of the <see cref="Subscription"/> to create</param>
        /// <param name="streamId">The id of the subscribed stream</param>
        /// <param name="durableName">The <see cref="Subscription"/>'s durable name</param>
        /// <param name="source">The EventStore subscription wrapped by the <see cref="Subscription"/></param>
        protected Subscription(string id, string streamId, string durableName, object source)
        {
            this.Id = id;
            this.StreamId = streamId;
            this.DurableName = durableName;
            this.Source = source;
        }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public string StreamId { get; }

        /// <inheritdoc/>
        public string DurableName { get; }

        /// <inheritdoc/>
        public object Source { get; protected set; }

        /// <inheritdoc/>
        public virtual void SetSource(object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            this.Source = source;
        }

        /// <inheritdoc/>
        public abstract Task UnsubscribeAsync(IEventStoreConnection connection, CancellationToken cancellationToken = default);

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="Subscription"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="Subscription"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {

                }
                this._Disposed = true;
                this.Disposed?.Invoke(this, new EventArgs());
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a new <see cref="ISubscription"/> for the specified EventStore subscription source
        /// </summary>
        /// <param name="id">The id of the <see cref="ISubscription"/> to create</param>
        /// <param name="streamId">The id of the subscribed stream</param>
        /// <param name="durableName">The <see cref="ISubscription"/>'s durable name</param>
        /// <param name="subscriptionSource">The EventStore subscription source to create the <see cref="ISubscription"/> for</param>
        /// <returns>A new <see cref="ISubscription"/> for the specified event store subscription source</returns>
        public static ISubscription CreateFor(string id, string streamId, string durableName, object subscriptionSource)
        {
            switch (subscriptionSource)
            {
                case EventStoreSubscription standardSubscription:
                    return new StandardSubscription(id, streamId, durableName, standardSubscription);
                case EventStoreStreamCatchUpSubscription catchUpSubscription:
                    return new CatchUpSubscription(id, streamId, durableName, catchUpSubscription);
                case EventStorePersistentSubscriptionBase persistentSubscription:
                    return new PersistentSubscription(id, streamId, durableName, persistentSubscription);
                default:
                    throw new NotSupportedException($"The specified EventSource subscription type '{subscriptionSource.GetType().Name}' is not supported");
            }
        }

    }

}
