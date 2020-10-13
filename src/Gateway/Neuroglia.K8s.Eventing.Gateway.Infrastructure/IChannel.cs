using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure
{

    /// <summary>
    /// Defines the fundamentals of an eventing channel
    /// </summary>
    public interface IChannel
        : IDisposable
    {

        /// <summary>
        /// Gets the <see cref="IChannel"/>'s name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the <see cref="IChannel"/>'s remote address
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// Creates a new subscription
        /// </summary>
        /// <param name="subscriptionOptions">The object used to configure the subscription to create</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task SubscribeAsync(SubscriptionOptionsDto subscriptionOptions, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an existing subscription
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to remove</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes the specified <see cref="CloudEvent"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to publish</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default);

    }

}
