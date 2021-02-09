using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Channels.Services;
using Neuroglia.K8s.Eventing.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IEventDispatcher"/> interface
    /// </summary>
    public class EventDispatcher
        : IEventDispatcher
    {

        /// <summary>
        /// Initializes a new <see cref="EventDispatcher"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="subscriptionManager">The service used to watch <see cref="V1SubscriptionResource"/>s</param>
        /// <param name="httpClient">The <see cref="System.Net.Http.HttpClient"/> used to dispatch <see cref="CloudEvent"/>s over HTTP</param>
        /// <param name="eventFormatter">The service used to format <see cref="CloudEvent"/>s</param>
        public EventDispatcher(ILogger<EventDispatcher> logger, ISubscriptionManager subscriptionManager, HttpClient httpClient, ICloudEventFormatter eventFormatter)
        {
            this.Logger = logger;
            this.SubscriptionManager = subscriptionManager;
            this.HttpClient = httpClient;
            this.EventFormatter = eventFormatter;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the servuce used to manage active <see cref="ISubscription"/>s
        /// </summary>
        protected ISubscriptionManager SubscriptionManager { get; }

        /// <summary>
        /// Gets the service used to format <see cref="CloudEvent"/>s
        /// </summary>
        protected ICloudEventFormatter EventFormatter { get; }

        /// <summary>
        /// Gets the <see cref="System.Net.Http.HttpClient"/> used to dispatch <see cref="CloudEvent"/>s over HTTP
        /// </summary>
        protected HttpClient HttpClient { get; }

        /// <inheritdoc/>
        public virtual async Task DispatchAsync(CloudEvent e, CancellationToken cancellationToken = default)
        {
            this.Logger.LogInformation("Dispatching cloud event with id '{eventId}'...", e.Id);
            List<ISubscription> subscriptions = (await this.SubscriptionManager.CorrelateSubscriptionsToAsync(e, cancellationToken)).ToList();
            this.Logger.LogInformation("{count} matching subscriptions have been found.", subscriptions.Count);
            List<Task> tasks = new List<Task>(subscriptions.Count);
            ContentMode contentMode = ContentMode.Binary;
            if (string.IsNullOrWhiteSpace(e.DataContentType?.MediaType)
                || e.DataContentType?.MediaType == MediaTypeNames.Application.Json)
                contentMode = ContentMode.Structured;
            CloudEventContent cloudEventContent = new CloudEventContent(e, contentMode, this.EventFormatter);
            foreach(ISubscription subscription in subscriptions)
            {
                tasks.Add(this.DispatchAsync(cloudEventContent, subscription, cancellationToken));
            }
            await Task.WhenAll(tasks);
            this.Logger.LogInformation("The cloud event with id '{eventId}' has been successfully dispatched", e.Id);
        }

        /// <summary>
        /// Dispatches a <see cref="CloudEventContent"/> to all <see cref="ISubscription"/>s of the specified <see cref="V1SubscriptionResource"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEventContent"/> to dispatch</param>
        /// <param name="subscription">The <see cref="ISubscription"/> for which to dispatch the <see cref="CloudEvent"/></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DispatchAsync(CloudEventContent e, ISubscription subscription, CancellationToken cancellationToken = default)
        {
            this.Logger.LogInformation("Dispatching cloud event to subscription '{subscription}'...", subscription.Name);
            List<Task> tasks = new List<Task>();
            foreach (V1SubscriberSpec subscriber in subscription.Spec.Subscribers)
            {
                switch (subscriber.DeliveryMode)
                {
                    case EventDeliveryMode.Unicast:
                        tasks.Add(this.DispatchAsync(e, subscription, subscriber.Url, cancellationToken));
                        break;
                    case EventDeliveryMode.Multicast:
                        foreach (IPAddress ipAddress in await Dns.GetHostAddressesAsync(subscriber.Url.Host))
                        {
                            Uri subscriberAddress = new Uri($"{subscriber.Url.Scheme}://{ipAddress}:{subscriber.Url.Port}{subscriber.Url.PathAndQuery}");
                            tasks.Add(this.DispatchAsync(e, subscription, subscriberAddress, cancellationToken));
                        }
                        break;
                    default:
                        throw new NotSupportedException($"The specified {nameof(EventDeliveryMode)} '{subscriber.DeliveryMode}' is not supported");
                }
            }
            await Task.WhenAll(tasks);
            this.Logger.LogInformation("Dispatched cloud event to all subscribers of '{subscription}'.", subscription.Name);
        }

        /// <summary>
        /// Dispatches a <see cref="CloudEventContent"/> to the specified <see cref="ISubscription"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEventContent"/> to dispatch</param>
        /// <param name="subscription">The address of the <see cref="ISubscription"/> to dispatch the <see cref="CloudEvent"/> to</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DispatchAsync(CloudEventContent e, ISubscription subscription, Uri subscriberAddress, CancellationToken cancellationToken = default)
        {
            using(HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, subscriberAddress) { Content = e })
            {
                using (HttpResponseMessage response = await this.HttpClient.SendAsync(request, cancellationToken))
                {

                }
            }
        }

    }

}
