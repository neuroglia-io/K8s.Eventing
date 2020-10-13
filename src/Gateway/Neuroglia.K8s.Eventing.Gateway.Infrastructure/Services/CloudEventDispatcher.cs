using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ICloudEventDispatcher"/> interface
    /// </summary>
    public class CloudEventDispatcher
        : ICloudEventDispatcher
    {

        /// <summary>
        /// Initializes a new <see cref="CloudEventDispatcher"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="subscriptionManager">The service used to manage <see cref="ISubscription"/>s</param>
        /// <param name="httpClient">The <see cref="System.Net.Http.HttpClient"/> used to dispatch <see cref="CloudEvent"/>s to subscribers</param>
        public CloudEventDispatcher(ILogger<CloudEventDispatcher> logger, ISubscriptionManager subscriptionManager, HttpClient httpClient)
        {
            this.Logger = logger;
            this.SubscriptionManager = subscriptionManager;
            this.HttpClient = httpClient;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to manage <see cref="ISubscription"/>s
        /// </summary>
        protected ISubscriptionManager SubscriptionManager { get; }

        /// <summary>
        /// Gets the <see cref="System.Net.Http.HttpClient"/> used to dispatch <see cref="CloudEvent"/>s to subscribers
        /// </summary>
        protected HttpClient HttpClient { get; }

        /// <inheritdoc/>
        public virtual async Task DispatchAsync(CloudEvent cloudEvent, CancellationToken cancellationToken = default)
        {
            this.Logger.LogInformation("Publishing a cloud event to all related subscriptions...");
            List<ISubscription> subscriptions = new List<ISubscription>();
            subscriptions.AddRange(this.SubscriptionManager.GetSubscriptionsBySubject(cloudEvent.Subject));
            subscriptions.AddRange(this.SubscriptionManager.GetSubscriptionsByEventType(cloudEvent.Type));
            subscriptions.AddRange(this.SubscriptionManager.GetSubscriptionsByEventSource(cloudEvent.Source));
            this.Logger.LogInformation("{subscriptionCount} matching subscriptions found.", subscriptions.Count);
            CloudEventContent cloudEventContent = new CloudEventContent(cloudEvent, ContentMode.Structured, new JsonEventFormatter());
            foreach (ISubscription subscription in subscriptions.Distinct())
            {
                this.Logger.LogInformation("{subscriberCount} subscribers found for subscription with id '{subscriptionId}'", subscription.Subscribers.Count, subscription.Id);
                foreach (Uri subscriberAddress in subscription.Subscribers)
                {
                    using (HttpResponseMessage response = await this.HttpClient.PostAsync(subscriberAddress, cloudEventContent, cancellationToken))
                    {
                        if (response.IsSuccessStatusCode)
                            this.Logger.LogInformation("The cloud event has been successfully dispatched to subscriber '{subscriberAddress}'", subscriberAddress);
                        else
                            this.Logger.LogError($"Failed to dispatch the cloud event to '{{subscriberAddress}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", subscriberAddress, response.StatusCode, await response.Content?.ReadAsStringAsync());
                    }
                }
            }
            this.Logger.LogInformation("The cloud event has been published to all related subscriptions.");
        }

    }

}
