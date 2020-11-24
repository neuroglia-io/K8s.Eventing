using EventStore.ClientAPI;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure
{
    /// <summary>
    /// Defines extensions for <see cref="SubscriptionOptionsDto"/>
    /// </summary>
    public static class SubscriptionOptionsDtoExtensions
    {

        /// <summary>
        /// Transforms the <see cref="SubscriptionOptionsDto"/> into a new <see cref="PersistentSubscriptionSettings"/>
        /// </summary>
        /// <param name="subscriptionOptions">The <see cref="SubscriptionOptionsDto"/> to transform</param>
        /// <returns>A new <see cref="PersistentSubscriptionSettings"/></returns>
        public static PersistentSubscriptionSettings ToPersistentSubscriptionSettings(this SubscriptionOptionsDto subscriptionOptions)
        {
            PersistentSubscriptionSettingsBuilder builder = PersistentSubscriptionSettings.Create();
            if (subscriptionOptions.StreamPosition.HasValue)
            {
                switch (subscriptionOptions.StreamPosition.Value)
                {
                    case StreamPosition.Start:
                        builder.StartFromBeginning();
                        break;
                    case StreamPosition.End:
                        builder.StartFromCurrent();
                        break;
                    default:
                        builder.StartFrom(subscriptionOptions.StreamPosition.Value);
                        break;
                }
            }
            builder.ResolveLinkTos();
            builder.WithMaxSubscriberCountOf(1);
            builder.MinimumCheckPointCountOf(1);
            builder.MaximumCheckPointCountOf(1);
            return builder.Build();
        }

        /// <summary>
        /// Transforms the <see cref="SubscriptionOptionsDto"/> into a new <see cref="CatchUpSubscriptionSettings"/>
        /// </summary>
        /// <param name="subscriptionOptions">The <see cref="SubscriptionOptionsDto"/> to transform</param>
        /// <returns>A new <see cref="CatchUpSubscriptionSettings"/></returns>
        public static CatchUpSubscriptionSettings ToCatchUpSubscriptionSettings(this SubscriptionOptionsDto subscriptionOptions)
        {
            CatchUpSubscriptionSettings defaultSettings = CatchUpSubscriptionSettings.Default;
            return new CatchUpSubscriptionSettings(defaultSettings.MaxLiveQueueSize, defaultSettings.ReadBatchSize, defaultSettings.VerboseLogging, true);
        }

    }

}
