using CloudNative.CloudEvents;
using System;
using System.Collections.Generic;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to manage <see cref="CloudEvent"/> subscriptions
    /// </summary>
    public interface ISubscriptionManager
    {

        /// <summary>
        /// Creates a new <see cref="ISubscription"/>
        /// </summary>
        /// <param name="id">The id of the <see cref="ISubscription"/> to create</param>
        /// <param name="subject">The <see cref="CloudEvent"/> subject the <see cref="Subscription"/> applies to</param>
        /// <param name="type">The <see cref="CloudEvent"/> type the <see cref="Subscription"/> applies to</param>
        /// <param name="source">The <see cref="CloudEvent"/> source the <see cref="Subscription"/> applies to</param>
        /// <param name="channelName">The name of the channel the <see cref="Subscription"/> is bound to</param>
        /// <param name="subscribers">An <see cref="IEnumerable{T}"/> containing the <see cref="Uri"/>s of all the subscribers</param>
        /// <returns>The newly created <see cref="ISubscription"/></returns>
        ISubscription RegisterSubscription(string id, string subject, string type, Uri source, string channelName, IEnumerable<Uri> subscribers);

        /// <summary>
        /// Deletes an existing <see cref="ISubscription"/>
        /// </summary>
        /// <param name="id">The id of the <see cref="ISubscription"/> to delete</param>
        void UnregisterSubscription(string id);

        /// <summary>
        /// Gets the <see cref="ISubscription"/> with the specified id
        /// </summary>
        /// <param name="id">The id of the subscription to get</param>
        /// <returns>The <see cref="ISubscription"/> with the specified id</returns>
        ISubscription GetSubscriptionById(string id);

        /// <summary>
        /// Gets all the <see cref="ISubscription"/>s to the specified subjet
        /// </summary>
        /// <param name="subject">The subject for which to get the <see cref="ISubscription"/>s</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the <see cref="ISubscription"/>s to the specified subject</returns>
        IEnumerable<ISubscription> GetSubscriptionsBySubject(string subject);

        /// <summary>
        /// Gets all the <see cref="ISubscription"/>s to the specified event type
        /// </summary>
        /// <param name="eventType">The event type for which to get the <see cref="ISubscription"/>s</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the <see cref="ISubscription"/>s to the specified event type</returns>
        IEnumerable<ISubscription> GetSubscriptionsByEventType(string eventType);

        /// <summary>
        /// Gets all the <see cref="ISubscription"/>s to the specified event source
        /// </summary>
        /// <param name="eventSource">The event source for which to get the <see cref="ISubscription"/>s</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the <see cref="ISubscription"/>s to the specified event source</returns>
        IEnumerable<ISubscription> GetSubscriptionsByEventSource(Uri eventSource);

        /// <summary>
        /// Gets all the <see cref="ISubscription"/>s to the specified channel name
        /// </summary>
        /// <param name="channelName">The name of the channel for which to get the <see cref="ISubscription"/>s</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all the <see cref="ISubscription"/>s to the specified channel name</returns>
        IEnumerable<ISubscription> GetSubscriptionsByChannel(string channelName);

    }

}
