using CloudNative.CloudEvents;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.K8s.Eventing.Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ISubscriptionManager"/> interface
    /// </summary>
    public class SubscriptionManager
        : ISubscriptionManager, IHostedService
    {

        /// <summary>
        /// Initializes a new <see cref="SubscriptionManager"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="subscriptionFactory">The service used to create <see cref="ISubscription"/>s</param>
        /// <param name="resources">The service used to watch <see cref="V1SubscriptionResource"/>s</param>
        /// <param name="options">The service used to access the current <see cref="SubscriptionManagerOptions"/></param>
        public SubscriptionManager(ILogger<SubscriptionManager> logger, ISubscriptionFactory subscriptionFactory, ICustomResourceWatcher<V1SubscriptionResource> resources, IOptions<SubscriptionManagerOptions> options)
        {
            this.Logger = logger;
            this.SubscriptionFactory = subscriptionFactory;
            this.Resources = resources;
            this.Options = options.Value;
            this.Subscriptions = new ConcurrentDictionary<string, ISubscription>();
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to create <see cref="ISubscription"/>s
        /// </summary>
        protected ISubscriptionFactory SubscriptionFactory { get; }

        /// <summary>
        /// Gets the service used to watch <see cref="V1SubscriptionResource"/>s
        /// </summary>
        protected ICustomResourceWatcher<V1SubscriptionResource> Resources { get; }

        /// <summary>
        /// Gets the <see cref="IDisposable"/> subscription to <see cref="V1SubscriptionResource"/> events
        /// </summary>
        protected IDisposable ResourceSubscription { get; private set; }

        /// <summary>
        /// Gets the current <see cref="SubscriptionManagerOptions"/>
        /// </summary>
        protected SubscriptionManagerOptions Options { get; }

        /// <summary>
        /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing all active <see cref="ISubscription"/>s
        /// </summary>
        protected ConcurrentDictionary<string, ISubscription> Subscriptions { get; }

        /// <inheritdoc/>
        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            this.ResourceSubscription = this.Resources.Subscribe(this.OnResourceEvent);
            foreach (V1SubscriptionResource resource in this.Resources)
            {
                await this.SubscribeAsync(this.GetSubscriptionNameFor(resource), resource.Spec);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<ISubscription>> CorrelateSubscriptionsToAsync(CloudEvent e, CancellationToken cancellationToken = default)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            return await Task.Run(() => this.Subscriptions.Values
                .ToList()
                .Where(s => s.CorrelatesTo(e)), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<string> SubscribeAsync(Action<ISubscriptionSpecBuilder> configuration, CancellationToken cancellationToken = default)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            string name = Guid.NewGuid().ToString();
            ISubscriptionSpecBuilder specBuilder = new SubscriptionSpecBuilder(this.Options.Channel);
            configuration(specBuilder);
            return await this.SubscribeAsync(name, specBuilder.Build(), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual Task<string> SubscribeAsync(string name, V1SubscriptionSpec spec, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));
            this.Logger.LogInformation("Creating a new subscription with name '{name}'...", name);
            ISubscription subscription = this.SubscriptionFactory.Create(name, spec);
            this.Subscriptions.AddOrUpdate(name, subscription, (key, sub) =>
            {
                sub.Dispose();
                return subscription;
            });
            this.Logger.LogInformation("The subscription with name '{name}' has been successfully created", name);
            return Task.FromResult(name);
        }

        /// <inheritdoc/>
        public virtual async Task UnsubscribeAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (!this.Subscriptions.TryRemove(name, out ISubscription subscription))
                return;
            subscription.Dispose();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Handles changes to <see cref="V1SubscriptionResource"/>s
        /// </summary>
        /// <param name="e">The new <see cref="V1SubscriptionResource"/></param>
        protected virtual void OnResourceEvent(IResourceEvent<V1SubscriptionResource> e)
        {
            switch (e.Type)
            {
                case k8s.WatchEventType.Added:
                case k8s.WatchEventType.Modified:
                    _ = this.SubscribeAsync(this.GetSubscriptionNameFor(e.Resource), e.Resource.Spec);
                    break;
                case k8s.WatchEventType.Deleted:
                    _ = this.UnsubscribeAsync(e.Resource.Metadata.Name);
                    break;
            }
        }

        /// <summary>
        /// Gets the <see cref="ISubscription"/> name for the specified <see cref="V1SubscriptionResource"/>
        /// </summary>
        /// <param name="subscriptionResource">The <see cref="V1SubscriptionResource"/> to get the <see cref="ISubscription"/> name for</param>
        /// <returns>The <see cref="ISubscription"/>'s name</returns>
        protected virtual string GetSubscriptionNameFor(V1SubscriptionResource subscriptionResource)
        {
            return $"{subscriptionResource.Metadata.NamespaceProperty}.{subscriptionResource.Metadata.Name}";
        }

        /// <inheritdoc/>
        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            this.Subscriptions?.ToList().ForEach(r => r.Value.Dispose());
            this.Subscriptions?.Clear();
            this.ResourceSubscription?.Dispose();
            this.ResourceSubscription = null;
            return Task.CompletedTask;
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="SubscriptionManager"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="SubscriptionManager"/> has been disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    this.StopAsync(default).GetAwaiter().GetResult();
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
