using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to wrap an EventStore channel
    /// </summary>
    public interface IEventChannel
        : IDisposable
    {

        /// <summary>
        /// Initializes the <see cref="IEventChannel"/>
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task InitializeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Subscribes to the <see cref="IEventChannel"/>
        /// </summary>
        /// <param name="subscriptionOptions">The <see cref="SubscriptionOptionsDto"/> used to configure the subscription to create</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task SubscribeAsync(SubscriptionOptionsDto subscriptionOptions, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unsubscribes from the <see cref="IEventChannel"/>
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to delete</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes the specified <see cref="CloudEvent"/> to the <see cref="IEventChannel"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to publish</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default);

    }

}
