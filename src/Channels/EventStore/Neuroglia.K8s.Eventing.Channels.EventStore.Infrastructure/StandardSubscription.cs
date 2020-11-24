using EventStore.ClientAPI;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure
{
    /// <summary>
    /// Represents a wrapper for an <see cref="EventStoreSubscription"/>
    /// </summary>
    public class StandardSubscription
        : Subscription
    {

        /// <summary>
        /// Initializes a new <see cref="StandardSubscription"/>
        /// </summary>
        /// <param name="id">The id of the <see cref="StandardSubscription"/> to create</param>
        /// <param name="streamId">The id of the subscribed stream</param>
        /// <param name="durableName">The <see cref="StandardSubscription"/>'s durable name</param>
        /// <param name="subscriptionSource">The wrapped <see cref="EventStoreSubscription"/></param>
        public StandardSubscription(string id, string streamId, string durableName, EventStoreSubscription subscriptionSource)
            : base(id, streamId, durableName, subscriptionSource)
        {

        }

        /// <summary>
        /// Gets the wrapped EventStore subscription
        /// </summary>
        protected new EventStoreSubscription Source
        {
            get
            {
                return base.Source as EventStoreSubscription;
            }
        }

        /// <inheritdoc/>
        public override Task UnsubscribeAsync(IEventStoreConnection connection, CancellationToken cancellationToken = default)
        {
            this.Source.Close();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                this.Source.Dispose();
            base.Dispose(disposing);
        }

    }

}
