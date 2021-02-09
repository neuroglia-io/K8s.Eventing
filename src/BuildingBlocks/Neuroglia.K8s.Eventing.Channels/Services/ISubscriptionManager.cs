using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Resources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to manage subscriptions
    /// </summary>
    public interface ISubscriptionManager
        : IDisposable
    {

        /// <summary>
        /// Correlates all the <see cref="ISubscription"/>s that apply to the specified <see cref="CloudEvent"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to correlate the <see cref="ISubscription"/>s to</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="ISubscription"/>s that correlate to the specified <see cref="CloudEvent"/></returns>
        Task<IEnumerable<ISubscription>> CorrelateSubscriptionsToAsync(CloudEvent e, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new <see cref="ISubscription"/>
        /// </summary>
        /// <param name="name">The name of the <see cref="ISubscription"/> to create</param>
        /// <param name="spec">The <see cref="V1SubscriptionSpec"/> used to create a new <see cref="ISubscription"/></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns></returns>
        Task<string> SubscribeAsync(string name, V1SubscriptionSpec spec, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new <see cref="ISubscription"/>
        /// </summary>
        /// <param name="configuration">The <see cref="Action{T}"/> used to configure the <see cref="ISubscription"/> to create</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns></returns>
        Task<string> SubscribeAsync(Action<ISubscriptionSpecBuilder> configuration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an active <see cref="ISubscription"/>
        /// </summary>
        /// <param name="subscriptionId">The id of the <see cref="ISubscription"/> to delete</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default);

    }

}
