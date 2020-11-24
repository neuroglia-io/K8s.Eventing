using EventStore.ClientAPI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure
{
    /// <summary>
    /// Represents a wrapper for an <see cref="EventStorePersistentSubscriptionBase"/>
    /// </summary>
    public class PersistentSubscription
       : Subscription
    {

        /// <summary>
        /// Initializes a new <see cref="PersistentSubscription"/>
        /// </summary>
        /// <param name="id">The id of the <see cref="PersistentSubscription"/> to create</param>
        /// <param name="streamId">The id of the subscribed stream</param>
        /// <param name="durableName">The <see cref="PersistentSubscription"/>'s durable name</param>
        /// <param name="subscriptionSource">The wrapped <see cref="EventStorePersistentSubscriptionBase"/></param>
        public PersistentSubscription(string id, string streamId, string durableName, EventStorePersistentSubscriptionBase subscriptionSource)
            : base(id, streamId, durableName, subscriptionSource)
        {

        }

        /// <summary>
        /// Gets the wrapped EventStore subscription
        /// </summary>
        protected new EventStorePersistentSubscriptionBase Source
        {
            get
            {
                return base.Source as EventStorePersistentSubscriptionBase;
            }
        }

        /// <inheritdoc/>
        public override async Task UnsubscribeAsync(IEventStoreConnection connection, CancellationToken cancellationToken = default)
        {
            await connection.DeletePersistentSubscriptionAsync(this.StreamId, this.DurableName);
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
