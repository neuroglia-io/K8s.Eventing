using CloudNative.CloudEvents;
using k8s;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure.Configuration;
using Neuroglia.K8s.Eventing.Resources;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IEventRegistry"/> interface
    /// </summary>
    public class EventRegistry
        : IEventRegistry
    {

        /// <summary>
        /// Initializes a new <see cref="EventRegistry"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="options">The current <see cref="ApplicationOptions"/></param>
        /// <param name="kubernetesClient">The service used to interact with the Kubernetes API</param>
        /// <param name="channelManager">The service used to manage <see cref="IChannel"/>s</param>
        /// <param name="subscriptionManager">The service used to manage <see cref="ISubscription"/>s</param>
        public EventRegistry(IServiceProvider serviceProvider, ILogger<EventRegistry> logger, IOptions<ApplicationOptions> options, IKubernetes kubernetesClient, IChannelManager channelManager, ISubscriptionManager subscriptionManager)
        {
            this.ServiceProvider = serviceProvider;
            this.Logger = logger;
            this.Options = options.Value;
            this.KubernetesClient = kubernetesClient;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the current <see cref="ApplicationOptions"/>
        /// </summary>
        protected ApplicationOptions Options { get; }

        /// <summary>
        /// Gets the service used to interact with the Kubernetes API
        /// </summary>
        protected IKubernetes KubernetesClient { get; }

        private ConcurrentBag<EventType> _EventTypes;
        /// <summary>
        /// Gets an <see cref="IEnumerable"/> containing all known <see cref="EventType"/>s
        /// </summary>
        public IEnumerable<EventType> EventTypes
        {
            get
            {
                return this._EventTypes;
            }
        }

        /// <inheritdoc/>
        public virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            this.Logger.LogInformation("Initializing event registry...");
            try
            {
                EventTypeDefinition eventTypeDefinition = EventingDefaults.Resources.EventType;
                KubernetesList<EventType> eventTypes = await this.KubernetesClient.ListNamespacedCustomObjectAsync<EventType>(eventTypeDefinition.Group, eventTypeDefinition.Version, this.Options.Pod.Namespace, eventTypeDefinition.Plural, watch: false, cancellationToken: cancellationToken);
                this._EventTypes = new ConcurrentBag<EventType>(eventTypes.Items);
            }
            catch(Exception ex)
            {
                this.Logger.LogError($"An error occured while initializing the event registry{Environment.NewLine}{{ex}}", ex.Message);
                throw;
            }
            this.Logger.LogInformation("Event registry initialized");
        }

        /// <inheritdoc/>
        public virtual async Task AddAsync(CloudEvent e, CancellationToken cancellationToken = default)
        {
            try
            {
                EventType eventType = this.EventTypes.FirstOrDefault(et =>
                   et.Spec?.Type == e.Type
                   && et.Spec?.Subject == e.Subject
                   && et.Spec?.Source == e.Source
                   && et.Spec?.DataSchema == e.DataSchema);
                if (eventType != null)
                    return; 
                eventType = new EventType
                (
                    new k8s.Models.V1ObjectMeta()
                    {
                        Name = $"{e.Type}-{Guid.NewGuid().ToString().Split("-")[1]}",
                        NamespaceProperty = this.Options.Pod.Namespace
                    }, 
                    new EventTypeSpec()
                    {
                        Type = e.Type,
                        Subject = e.Subject,
                        Source = e.Source,
                        DataSchema = e.DataSchema
                    }
                );
                await this.KubernetesClient.CreateNamespacedCustomObjectAsync(eventType, this.Options.Pod.Namespace);
                this._EventTypes.Add(eventType);
            }
            catch(Exception ex)
            {
                this.Logger.LogError($"An error occured while adding a new event to the registry:{Environment.NewLine}{{ex}}", ex.Message);
                throw;
            }
   

        }

    }

}
