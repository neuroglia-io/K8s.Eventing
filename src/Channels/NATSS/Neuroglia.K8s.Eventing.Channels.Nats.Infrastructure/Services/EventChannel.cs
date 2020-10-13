using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using Neuroglia.CloudEvents;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STAN.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Nats.Infrastructure.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IEventChannel"/> interface
    /// </summary>
    public class EventChannel
        : IEventChannel
    {

        private object _Lock = new object();

        /// <summary>
        /// Initializes a new <see cref="EventChannel"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="stanConnection">The underlying NATS Streaming connection</param>
        /// <param name="httpClientFactory">The service used to create new <see cref="System.Net.Http.HttpClient"/> instances</param>
        public EventChannel(ILogger<EventChannel> logger, IStanConnection stanConnection, IHttpClientFactory httpClientFactory)
        {
            this.Logger = logger;
            this.StanConnection = stanConnection;
            this.EventFormatter = new JsonEventFormatter();
            this.HttpClient = httpClientFactory.CreateClient(typeof(EventChannel).Name);
            this.SubscriptionRegistry = new HashFile();
            this.SubscriptionRegistry.Initialize(Path.Combine(AppContext.BaseDirectory, "sub"), 128, (ushort)JsonConvert.SerializeObject(new SubscriptionOptionsDto() { Id = StringExtensions.GenerateRandomString(128), Subject = StringExtensions.GenerateRandomString(128), DurableName = StringExtensions.GenerateRandomString(128), StreamPosition = 999999 }).Length);
            this.Subscriptions = new ConcurrentDictionary<string, IStanSubscription>();
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the underlying NATS Streaming connection
        /// </summary>
        protected IStanConnection StanConnection { get; }

        /// <summary>
        /// Gets the service used to format <see cref="CloudEvent"/>s
        /// </summary>
        protected JsonEventFormatter EventFormatter { get; }

        /// <summary>
        /// Gets the <see cref="System.Net.Http.HttpClient"/> used to dispatch <see cref="CloudEvent"/>s to the gateway
        /// </summary>
        protected HttpClient HttpClient { get; }

        /// <summary>
        /// Gets the <see cref="HashFile"/> used to store the subscription registry
        /// </summary>
        protected HashFile SubscriptionRegistry { get; }

        /// <summary>
        /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing in-memory subscriptions
        /// </summary>
        protected ConcurrentDictionary<string, IStanSubscription> Subscriptions { get; }

        /// <inheritdoc/>
        public virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            foreach (KeyValuePair<string, string> entry in this.SubscriptionRegistry)
            {
                SubscriptionOptionsDto subscriptionOptions = JsonConvert.DeserializeObject<SubscriptionOptionsDto>(entry.Value);
                await this.SubscribeAsync(subscriptionOptions, false, cancellationToken);
            }
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task SubscribeAsync(SubscriptionOptionsDto subscriptionOptions, CancellationToken cancellationToken = default)
        {
            await this.SubscribeAsync(subscriptionOptions, true, cancellationToken);
        }

        /// <summary>
        /// Creates a new subscription
        /// </summary>
        /// <param name="subscriptionOptions">The object used to configure the subscription to create</param>
        /// <param name="persist">A boolean indicating whether or not to persist the subscription to the <see cref="SubscriptionRegistry"/></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task SubscribeAsync(SubscriptionOptionsDto subscriptionOptions, bool persist, CancellationToken cancellationToken = default)
        {
            string subject = this.GetStanSubjectFor(subscriptionOptions.Subject);
            this.Logger.LogInformation("NATSS subject: {subject}", subject);
            IStanSubscription subscription = this.StanConnection.Subscribe(subject, subscriptionOptions.ToStanSubscriptionOptions(), this.CreateEventHandlerFor(subscriptionOptions.Id));
            this.Subscriptions.TryAdd(subscriptionOptions.Id, subscription);
            if (persist)
            {
                lock (this._Lock)
                {
                    this.SubscriptionRegistry.InsertKey(subscriptionOptions.Id, JsonConvert.SerializeObject(subscriptionOptions), false);
                }
            }
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default)
        {
            if (this.Subscriptions.TryRemove(subscriptionId, out IStanSubscription subscription))
                subscription.Dispose();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default)
        {
            string subject = this.GetStanSubjectFor(e.Subject);
            this.Logger.LogInformation("NATSS subject: {subject}", subject);
            await this.StanConnection.PublishAsync(subject, this.EventFormatter.EncodeStructuredEvent(e, out ContentType contentType));
        }

        /// <summary>
        /// Gets the NATS Streaming subject for the specified <see cref="CloudEvent"/> subject
        /// </summary>
        /// <param name="cloudEventSubject">The <see cref="CloudEvent"/> subject to get the NATS Streaming subject for</param>
        /// <returns>The NATS Streaming subject for the specified <see cref="CloudEvent"/> subject</returns>
        protected virtual string GetStanSubjectFor(string cloudEventSubject)
        {
            return $"neuroglia.eventing.{cloudEventSubject}";
        }

        /// <summary>
        /// Creates a new handler for handling the messages of the specified subscription
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to handle the messages for</param>
        /// <returns>A new handler for handling the messages of the specified subscription</returns>
        protected virtual EventHandler<StanMsgHandlerArgs> CreateEventHandlerFor(string subscriptionId)
        {
            return async (sender, e) =>
            {
                this.Logger.LogInformation("An event with subject '{subject}' has been received from the underlying NATS Streaming sink.", e.Message.Subject);
                this.Logger.LogInformation("Dispatching the resulting cloud event with subject '{subject}' to the gateway...", e.Message.Subject);
                CloudEvent cloudEvent = CloudEventBuilder.FromEvent(this.EventFormatter.DecodeJObject(JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(e.Message.Data))))
                    .WithSubscriptionId(subscriptionId)
                    .WithSequence(e.Message.Sequence)
                    .HasBeenRedelivered(e.Message.Redelivered)
                    .Build();
                CloudEventContent cloudEventContent = new CloudEventContent(cloudEvent, ContentMode.Structured, new JsonEventFormatter());
                using(HttpResponseMessage response = await this.HttpClient.PostAsync(string.Empty, cloudEventContent))
                {
                    string content = await response.Content?.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        this.Logger.LogError($"An error occured while dispatching a cloud event of type '{{eventType}}' to the eventing controller: the remote server response with a '{{statusCode}}' status code.{Environment.NewLine}Details: {{responseContent}}", cloudEvent.Type, response.StatusCode, content);
                        response.EnsureSuccessStatusCode();
                    }
                }
                this.Logger.LogInformation("The cloud event with subject '{subject}' has been successfully dispatched to the gateway.", e.Message.Subject);
                e.Message.Ack();
            };
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="EventChannel"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="EventChannel"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    foreach(KeyValuePair<string, IStanSubscription> entry in this.Subscriptions)
                    {
                        entry.Value.Dispose();
                    }
                    this.Subscriptions.Clear();
                }
                this._Disposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

}
