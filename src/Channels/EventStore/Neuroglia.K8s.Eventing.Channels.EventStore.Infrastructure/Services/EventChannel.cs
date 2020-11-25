using CloudNative.CloudEvents;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.CloudEvents;
using Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.Configuration;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IEventChannel"/> interface
    /// </summary>
    public class EventChannel
        : IEventChannel
    {

        private object _Lock = new object();

        private const string CloudEventStreamName = "cloudevents-stream";

        /// <summary>
        /// Initializes a new <see cref="EventChannel"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="applicationOptions">The service used to access the current <see cref="ApplicationOptions"/></param>
        /// <param name="eventStoreConnection">The underlying EventStore connection</param>
        /// <param name="httpClientFactory">The service used to create new <see cref="System.Net.Http.HttpClient"/> instances</param>
        public EventChannel(ILogger<EventChannel> logger, IOptions<ApplicationOptions> applicationOptions, IEventStoreConnection eventStoreConnection, IHttpClientFactory httpClientFactory)
        {
            this.Logger = logger;
            this.ApplicationOptions = applicationOptions.Value;
            this.EventStoreConnection = eventStoreConnection;
            this.EventFormatter = new JsonEventFormatter();
            this.HttpClient = httpClientFactory.CreateClient(typeof(EventChannel).Name);
            this.SubscriptionRegistry = new HashFile();
            this.SubscriptionRegistry.Initialize(Path.Combine(AppContext.BaseDirectory, "sub"), 128, (ushort)JsonConvert.SerializeObject(new SubscriptionOptionsDto() { Id = StringExtensions.GenerateRandomString(128), Subject = StringExtensions.GenerateRandomString(128), DurableName = StringExtensions.GenerateRandomString(128), StreamPosition = 999999 }).Length);
            this.Subscriptions = new ConcurrentDictionary<string, ISubscription>();
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected Microsoft.Extensions.Logging.ILogger Logger { get; }

        /// <summary>
        /// Gets the current <see cref="ApplicationOptions"/>
        /// </summary>
        protected ApplicationOptions ApplicationOptions { get; }

        /// <summary>
        /// Gets the connection to the remote EventStore server
        /// </summary>
        protected IEventStoreConnection EventStoreConnection { get; }

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
        protected ConcurrentDictionary<string, ISubscription> Subscriptions { get; }

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
        public virtual async Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(e.Id, out Guid eventId))
                eventId = Guid.NewGuid();
            byte[] data = this.EventFormatter.EncodeStructuredEvent(e, out ContentType contentType);
            byte[] metadata = new byte[0];
            await this.EventStoreConnection.AppendToStreamAsync(CloudEventStreamName, ExpectedVersion.Any, new EventData(eventId, e.Type, true, data, metadata));
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
            string streamId = this.GetEventStreamIdFor(subscriptionOptions);
            object subscriptionSource;
            if (string.IsNullOrWhiteSpace(subscriptionOptions.DurableName))
            {
                if (subscriptionOptions.StreamPosition.HasValue)
                {
                    CatchUpSubscriptionSettings settings = subscriptionOptions.ToCatchUpSubscriptionSettings();
                    subscriptionSource = this.EventStoreConnection.SubscribeToStreamFrom(streamId, subscriptionOptions.StreamPosition.Value, settings,
                        this.CreateCatchUpSubscriptionHandler(subscriptionOptions.Id), subscriptionDropped: this.CreateCatchUpSubscriptionDropHandler(subscriptionOptions.Id, streamId, subscriptionOptions.StreamPosition, settings));
                }
                else
                {
                    subscriptionSource = await this.EventStoreConnection.SubscribeToStreamAsync(streamId, true, this.CreateStandardSubscriptionHandler(subscriptionOptions.Id), 
                        subscriptionDropped: this.CreateStandardSubscriptionDropHandler(subscriptionOptions.Id, streamId));
                }
            }
            else
            {
                try
                {
                    await this.EventStoreConnection.CreatePersistentSubscriptionAsync(streamId, subscriptionOptions.DurableName, subscriptionOptions.ToPersistentSubscriptionSettings(), 
                        new UserCredentials(this.ApplicationOptions.EventStore.Username, this.ApplicationOptions.EventStore.Password));
                }
                catch { }
                subscriptionSource = await this.EventStoreConnection.ConnectToPersistentSubscriptionAsync(streamId, subscriptionOptions.DurableName, this.CreatePersistentSubscriptionHandler(subscriptionOptions.Id), 
                    this.CreatePersistentSubscriptionDropHandler(subscriptionOptions.Id, streamId, subscriptionOptions.DurableName), autoAck: false);
            }
            this.AddOrUpdateSubscription(subscriptionOptions.Id, streamId, subscriptionOptions.DurableName, subscriptionSource);
            if (persist)
            {
                lock (this._Lock)
                {
                    this.SubscriptionRegistry.InsertKey(subscriptionOptions.Id, JsonConvert.SerializeObject(subscriptionOptions), false);
                }
            }
            this.Logger.LogInformation("Created a new EventStore subscription to stream '{streamId}'", streamId);
        }

        /// <inheritdoc/>
        public virtual async Task UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default)
        {
            if (!this.Subscriptions.TryRemove(subscriptionId, out ISubscription subscription))
                return;
            await subscription.UnsubscribeAsync(this.EventStoreConnection, cancellationToken);
        }

        /// <summary>
        /// Gets the id of the stream the specified <see cref="SubscriptionOptionsDto"/> applies to
        /// </summary>
        /// <param name="subscriptionOptions">The <see cref="SubscriptionOptionsDto"/> to get the stream id for</param>
        /// <returns>The id of the stream the specified <see cref="SubscriptionOptionsDto"/> applies to</returns>
        protected virtual string GetEventStreamIdFor(SubscriptionOptionsDto subscriptionOptions)
        {
            if (!string.IsNullOrWhiteSpace(subscriptionOptions.Subject))
                return $"$cloudevent-subject-{subscriptionOptions.Subject}";
            else if (!string.IsNullOrWhiteSpace(subscriptionOptions.Type))
                return $"$et-{subscriptionOptions.Type}";
            else if (!string.IsNullOrWhiteSpace(subscriptionOptions.Source))
                return $"$cloudevent-source-{subscriptionOptions.Source}";
            else
                throw new NotSupportedException();
        }

        /// <summary>
        /// Adds or updates a new <see cref="ISubscription"/>
        /// </summary>
        /// <param name="subscriptionId">The <see cref="ISubscription"/>'s id</param>
        /// <param name="streamId">The id of the subscribed stream</param>
        /// <param name="durableName">The <see cref="ISubscription"/>'s durable name</param>
        /// <param name="subscriptionSource">The <see cref="IEventStoreSubscription"/>'s source</param>
        protected virtual void AddOrUpdateSubscription(string subscriptionId, string streamId, string durableName, object subscriptionSource)
        {
            if (this.Subscriptions.TryGetValue(subscriptionId, out ISubscription subscription))
            {
                subscription.SetSource(subscriptionSource);
            }
            else
            {
                subscription = Subscription.CreateFor(subscriptionId, streamId, durableName, subscriptionSource);
                subscription.Disposed += this.OnSubscriptionDisposed;
                this.Subscriptions.AddOrUpdate(subscription.Id, subscription, (id, sub) => sub);
            }
        }

        /// <summary>
        /// Creates a new persistent subscription handler
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to create the handler for</param>
        /// <returns>A new <see cref="Func{T1, T2, TResult}"/> used to handle the persistent subscription</returns>
        protected virtual Func<EventStorePersistentSubscriptionBase, ResolvedEvent, Task> CreatePersistentSubscriptionHandler(string subscriptionId)
        {
            return async (subscription, e) =>
            {
                try
                {
                    await this.DispatchEventAsync(subscriptionId, e);
                    subscription.Acknowledge(e.Event.EventId);
                }
                catch(Exception ex)
                {
                    subscription.Fail(e, PersistentSubscriptionNakEventAction.Park, ex.Message);
                }
            };
        }

        /// <summary>
        /// Creates a new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription that has been dropped</param>
        /// <param name="streamId">The id of the subscribed stream</param>
        /// <param name="subscriptionName">The persistent subscription's name</param>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <param name="eventAckMode">The way subscribed events should be acknowledged</param>
        /// <returns>A new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops</returns>
        protected virtual Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception> CreatePersistentSubscriptionDropHandler(string subscriptionId, string streamId, string subscriptionName)
        {
            return (sub, reason, ex) =>
            {
                switch (reason)
                {
                    case SubscriptionDropReason.UserInitiated:
                    case SubscriptionDropReason.NotFound:
                    case SubscriptionDropReason.NotAuthenticated:
                    case SubscriptionDropReason.AccessDenied:
                    case SubscriptionDropReason.PersistentSubscriptionDeleted:
                        this.Logger.LogInformation("The persistent subscription to stream with id '{streamId}' and with group name '{subscriptionName}' has been dropped for the following reason: '{dropReason}'.",
                            streamId, subscriptionName, reason);
                        if (this.Subscriptions.TryGetValue(subscriptionId, out ISubscription subscription))
                            subscription.Dispose();
                        break;
                    default:
                        this.Logger.LogInformation("The persistent subscription to stream with id '{streamId}' and with group name '{subscriptionName}' has been dropped for the following reason: '{dropReason}'. Resubscribing...",
                            streamId, subscriptionName, reason);
                        object subscriptionSource = this.EventStoreConnection.ConnectToPersistentSubscriptionAsync(streamId, subscriptionName,
                            this.CreatePersistentSubscriptionHandler(subscriptionId),
                            this.CreatePersistentSubscriptionDropHandler(subscriptionId, streamId, subscriptionName), 
                            autoAck: false);
                        this.AddOrUpdateSubscription(subscriptionId, streamId, subscriptionName, subscriptionSource);
                        break;
                }
            };
        }

        /// <summary>
        /// Creates a new catch-up subscription handler
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to create the handler for</param>
        /// <returns>A new <see cref="Func{T1, T2, TResult}"/> used to handle the catch-up subscription</returns>
        protected virtual Func<EventStoreCatchUpSubscription, ResolvedEvent, Task> CreateCatchUpSubscriptionHandler(string subscriptionId)
        {
            return async (subscription, e) =>
            {
                await this.DispatchEventAsync(subscriptionId, e);
            };
        }

        /// <summary>
        /// Creates a new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription that has been dropped</param>
        /// <param name="streamId">The id of the catch-up stream</param>
        /// <param name="startFrom">The event number from which to start</param>
        /// <param name="settings">The <see cref="CatchUpSubscriptionSettings"/> to use</param>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <returns>A new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops</returns>
        protected virtual Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> CreateCatchUpSubscriptionDropHandler(string subscriptionId, string streamId, long? startFrom, CatchUpSubscriptionSettings settings)
        {
            return (sub, reason, ex) =>
            {
                switch (reason)
                {
                    case SubscriptionDropReason.UserInitiated:
                    case SubscriptionDropReason.NotFound:
                    case SubscriptionDropReason.NotAuthenticated:
                    case SubscriptionDropReason.AccessDenied:
                        this.Logger.LogInformation("A catch-up subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'.", streamId, reason);
                        if (this.Subscriptions.TryGetValue(subscriptionId, out ISubscription subscription))
                            subscription.Dispose();
                        break;
                    default:
                        this.Logger.LogInformation("A catch-up subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'. Resubscribing...", streamId, reason);
                        object subscriptionSource = this.EventStoreConnection.SubscribeToStreamFrom(streamId, startFrom, settings,
                            this.CreateCatchUpSubscriptionHandler(subscriptionId),
                            subscriptionDropped: this.CreateCatchUpSubscriptionDropHandler(subscriptionId, streamId, startFrom, settings));
                        this.AddOrUpdateSubscription(subscriptionId, streamId, null, subscriptionSource);
                        break;
                }
            };
        }

        /// <summary>
        /// Creates a new standard subscription handler
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to create the handler for</param>
        /// <returns>A new <see cref="Func{T1, T2, TResult}"/> used to handle the standard subscription</returns>
        protected virtual Func<EventStoreSubscription, ResolvedEvent, Task> CreateStandardSubscriptionHandler(string subscriptionId)
        {
            return async (subscription, e) =>
            {
                await this.DispatchEventAsync(subscriptionId, e);
            };
        }

        /// <summary>
        /// Creates a new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription that has been dropped</param>
        /// <param name="streamId">The id of the catch-up stream</param>
        /// <param name="resolveLinks">A boolean indicating whether or not to resolve event links</param>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <returns>A new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops</returns>
        protected virtual Action<EventStoreSubscription, SubscriptionDropReason, Exception> CreateStandardSubscriptionDropHandler(string subscriptionId, string streamId)
        {
            return (sub, reason, ex) =>
            {
                switch (reason)
                {
                    case SubscriptionDropReason.UserInitiated:
                    case SubscriptionDropReason.NotFound:
                    case SubscriptionDropReason.NotAuthenticated:
                    case SubscriptionDropReason.AccessDenied:
                        this.Logger.LogInformation("A standard subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'.", streamId, reason);
                        if (this.Subscriptions.TryGetValue(subscriptionId, out ISubscription subscription))
                            subscription.Dispose();
                        break;
                    default:
                        this.Logger.LogInformation("A standard subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'. Resubscribing...", streamId, reason);
                        object subscriptionSource = this.EventStoreConnection.SubscribeToStreamAsync(streamId, true,
                            this.CreateStandardSubscriptionHandler(subscriptionId),
                            this.CreateStandardSubscriptionDropHandler(subscriptionId, streamId))
                            .GetAwaiter().GetResult();
                        this.AddOrUpdateSubscription(subscriptionId, streamId, null, subscriptionSource);
                        break;
                }
            };
        }

        /// <summary>
        /// Dispatches the <see cref="ResolvedEvent"/> to the <see cref="CloudEvent"/> gateway
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription the <see cref="CloudEvent"/> originates from</param>
        /// <param name="e">The <see cref="ResolvedEvent"/> to dispatch</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DispatchEventAsync(string subscriptionId, ResolvedEvent e)
        {
            this.Logger.LogInformation("An event has been received from the underlying EventStore sink.");
            this.Logger.LogInformation("Dispatching the resulting cloud event to the gateway...");
            CloudEvent cloudEvent = CloudEventBuilder.FromEvent(this.EventFormatter.DecodeJObject(JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(e.Event.Data))))
                .WithSubscriptionId(subscriptionId)
                .WithSequence((ulong)e.Event.EventNumber)
                .Build();
            CloudEventContent cloudEventContent = new CloudEventContent(cloudEvent, ContentMode.Structured, new JsonEventFormatter());
            using (HttpResponseMessage response = await this.HttpClient.PostAsync("pub", cloudEventContent))
            {
                string content = await response.Content?.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    this.Logger.LogError($"An error occured while dispatching a cloud event of type '{{eventType}}' to the eventing controller: the remote server response with a '{{statusCode}}' status code.{Environment.NewLine}Details: {{responseContent}}", cloudEvent.Type, response.StatusCode, content);
                    response.EnsureSuccessStatusCode();
                }
            }
            this.Logger.LogInformation("The cloud event has been successfully dispatched to the gateway.");
        }

        /// <summary>
        /// Represents the handler fired whenever an <see cref="ISubscription"/> has been disposed
        /// </summary>
        /// <param name="sender">The disposed <see cref="ISubscription"/></param>
        /// <param name="e">The event's arguments</param>
        protected virtual void OnSubscriptionDisposed(object sender, EventArgs e)
        {
            ISubscription subscription = (ISubscription)sender;
            this.Subscriptions.Remove(subscription.Id, out subscription);
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
                    foreach (KeyValuePair<string, ISubscription> entry in this.Subscriptions)
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
