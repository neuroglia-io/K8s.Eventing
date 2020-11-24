using EventStore.ClientAPI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure
{
    /// <summary>
    /// Represents a wrapper for an <see cref="EventStoreStreamCatchUpSubscription"/>
    /// </summary>
    public class CatchUpSubscription
        : Subscription
    {

        /// <summary>
        /// Initializes a new <see cref="CatchUpSubscription"/>
        /// </summary>
        /// <param name="id">The id of the <see cref="CatchUpSubscription"/> to create</param>
        /// <param name="streamId">The id of the subscribed stream</param>
        /// <param name="durableName">The <see cref="CatchUpSubscription"/>'s durable name</param>
        /// <param name="subscriptionSource">The wrapped <see cref="EventStoreStreamCatchUpSubscription"/></param>
        public CatchUpSubscription(string id, string streamId, string durableName, EventStoreStreamCatchUpSubscription subscriptionSource)
            : base(id, streamId, durableName, subscriptionSource)
        {

        }

        /// <summary>
        /// Gets the wrapped EventStore subscription
        /// </summary>
        protected new EventStoreStreamCatchUpSubscription Source
        {
            get
            {
                return base.Source as EventStoreStreamCatchUpSubscription;
            }
        }

        /// <inheritdoc/>
        public override Task UnsubscribeAsync(IEventStoreConnection connection, CancellationToken cancellationToken = default)
        {
            this.Source.Stop();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                this.Source.Stop(TimeSpan.FromSeconds(3));
            base.Dispose(disposing);
        }

    }

}
