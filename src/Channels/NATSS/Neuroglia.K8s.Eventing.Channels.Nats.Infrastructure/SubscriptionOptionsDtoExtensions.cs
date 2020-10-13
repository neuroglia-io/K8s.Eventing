using Neuroglia.K8s.Eventing.Gateway.Integration;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using STAN.Client;
using System;

namespace Neuroglia.K8s.Eventing.Channels.Nats.Infrastructure
{

    /// <summary>
    /// Defines extensions for <see cref="SubscriptionOptionsDto"/>s
    /// </summary>
    public static class SubscriptionOptionsDtoExtensions
    {

        /// <summary>
        /// Converts the <see cref="SubscriptionOptionsDto"/> to a <see cref="StanSubscriptionOptions"/>
        /// </summary>
        /// <param name="subscriptionOptions">The <see cref="SubscriptionOptionsDto"/> to convert</param>
        /// <returns>The resulting <see cref="StanSubscriptionOptions"/></returns>
        public static StanSubscriptionOptions ToStanSubscriptionOptions(this SubscriptionOptionsDto subscriptionOptions)
        {
            StanSubscriptionOptions stanSubscriptionOptions = StanSubscriptionOptions.GetDefaultOptions();
            stanSubscriptionOptions.ManualAcks = true;
            if (!string.IsNullOrWhiteSpace(subscriptionOptions.DurableName))
            {
                stanSubscriptionOptions.DurableName = subscriptionOptions.DurableName;
                stanSubscriptionOptions.LeaveOpen = true;
            }
            if (subscriptionOptions.StreamPosition.HasValue)
            {
                if (subscriptionOptions.StreamPosition.Value == StreamPosition.Start)
                    stanSubscriptionOptions.StartWithLastReceived();
                else if (subscriptionOptions.StreamPosition.Value != StreamPosition.End
                    && subscriptionOptions.StreamPosition.Value < 0)
                    throw new ArgumentOutOfRangeException();
                else
                    stanSubscriptionOptions.StartAt((ulong)subscriptionOptions.StreamPosition.Value);
            }
            return stanSubscriptionOptions;
        }

    }

}
