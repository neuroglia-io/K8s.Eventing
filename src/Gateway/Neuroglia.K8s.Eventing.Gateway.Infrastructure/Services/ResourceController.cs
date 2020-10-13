using k8s;
using k8s.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Neuroglia.Istio.Resources;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure.Configuration;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using Neuroglia.K8s.Eventing.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services
{

    /// <summary>
    /// Represents the service used to manage eventing-related  <see cref="CustomResource"/>s
    /// </summary>
    public class ResourceController
        : IResourceController
    {

        private CancellationTokenSource _StoppingTokenSource = new CancellationTokenSource();
        private Task _ExecutingTask;

        /// <summary>
        /// Initializes a new <see cref="ResourceController"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="options">The current <see cref="ApplicationOptions"/></param>
        /// <param name="kubernetesClient">The service used to interact with the Kubernetes API</param>
        /// <param name="channelManager">The service used to manage <see cref="IChannel"/>s</param>
        /// <param name="subscriptionManager">The service used to manage <see cref="ISubscription"/>s</param>
        public ResourceController(IServiceProvider serviceProvider, ILogger<ResourceController> logger, IOptions<ApplicationOptions> options, IKubernetes kubernetesClient, IChannelManager channelManager, ISubscriptionManager subscriptionManager)
        {
            this.ServiceProvider = serviceProvider;
            this.Logger = logger;
            this.Options = options.Value;
            this.KubernetesClient = kubernetesClient;
            this.ChannelManager = channelManager;
            this.SubscriptionManager = subscriptionManager;
            this.EventWatchers = new List<ICustomResourceEventWatcher>();
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

        /// <summary>
        /// Gets the service used to manage <see cref="IChannel"/>s
        /// </summary>
        protected IChannelManager ChannelManager { get; }

        /// <summary>
        /// Gets the service used to manage <see cref="ISubscription"/>s
        /// </summary>
        protected ISubscriptionManager SubscriptionManager { get; }

        /// <summary>
        /// Gets an <see cref="IList{T}"/> containing all registered <see cref="ICustomResourceEventWatcher"/>s
        /// </summary>
        protected IList<ICustomResourceEventWatcher> EventWatchers { get; }

        /// <inheritdoc/>
        public virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.InitializeChannelsAsync(cancellationToken);
                await this.InitializeSubscriptionsAsync(cancellationToken);
                this.EventWatchers.Add(ActivatorUtilities.CreateInstance<CustomResourceEventWatcher<Resources.Channel>>(this.ServiceProvider, EventingDefaults.Resources.Channel, this.Options.Pod.Namespace, new CustomResourceEventDelegate<Resources.Channel>(this.OnChannelEvent)));
                this.EventWatchers.Add(ActivatorUtilities.CreateInstance<CustomResourceEventWatcher<Resources.Subscription>>(this.ServiceProvider, EventingDefaults.Resources.Subscription, string.Empty, new CustomResourceEventDelegate<Resources.Subscription>(this.OnSubscriptionEvent)));
                this.EventWatchers.Add(ActivatorUtilities.CreateInstance<CustomResourceEventWatcher<Broker>>(this.ServiceProvider, EventingDefaults.Resources.Broker, string.Empty, new CustomResourceEventDelegate<Broker>(this.OnBrokerEvent)));
                foreach (ICustomResourceEventWatcher eventWatcher in this.EventWatchers)
                {
                    await eventWatcher.StartAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// Initializes all <see cref="IChannel"/>s
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task InitializeChannelsAsync(CancellationToken cancellationToken = default)
        {
            KubernetesList<Resources.Channel> channels = await this.KubernetesClient.ListNamespacedCustomObjectAsync<Resources.Channel>(EventingDefaults.Resources.Channel.Group, EventingDefaults.Resources.Channel.Version, this.Options.Pod.Namespace, EventingDefaults.Resources.Channel.Plural, cancellationToken: cancellationToken);
            foreach(Resources.Channel channel in channels.Items)
            {
                this.RegisterChannel(channel);
            }
        }

        /// <summary>
        /// Initializes all <see cref="ISubscription"/>s
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task InitializeSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            KubernetesList<Resources.Subscription> subscriptions = await this.KubernetesClient.ListNamespacedCustomObjectAsync<Resources.Subscription>(EventingDefaults.Resources.Subscription.Group, EventingDefaults.Resources.Subscription.Version, this.Options.Pod.Namespace, EventingDefaults.Resources.Subscription.Plural, cancellationToken: cancellationToken);
            foreach (Resources.Subscription subscription in subscriptions.Items)
            {
                this.RegisterSubscription(subscription);
            }
        }

        #region Channels

        /// <summary>
        /// Handles a <see cref="Resources.Channel"/> event
        /// </summary>
        /// <param name="e">The type of event to handle</param>
        /// <param name="channel">The <see cref="Resources.Channel"/> the event to handle applies to</param>
        protected virtual async void OnChannelEvent(WatchEventType e, Resources.Channel channel)
        {
            this.Logger.LogInformation("A new event of type '{watchEventType}' concerning the CRD of kind '{crdKind}' with name '{crdName}' has been received.", e, channel.Kind, channel.Name());
            switch (e)
            {
                case WatchEventType.Added:
                    await this.DeployChannelAsync(channel);
                    break;
                case WatchEventType.Modified:

                    break;
                case WatchEventType.Deleted:
                    await this.DeleteChannelAsync(channel);
                    break;
                case WatchEventType.Bookmark:

                    break;
                case WatchEventType.Error:

                    break;
                default:
                    throw new NotSupportedException($"The specified watch event type '{e}' is not supported");
            }
        }

        /// <summary>
        /// Deploys the specified <see cref="Resources.Channel"/>
        /// </summary>
        /// <param name="channel">The <see cref="Resources.Channel"/> to deploy</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeployChannelAsync(Resources.Channel channel)
        {
            try
            {
                await this.CreateChannelDeploymentAsync(channel);
                await this.CreateChannelServiceAsync(channel);
                this.RegisterChannel(channel);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Creates a new <see cref="V1Deployment"/> for the specified <see cref="Resources.Channel"/>
        /// </summary>
        /// <param name="channel">The <see cref="Resources.Channel"/> to deploy</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task CreateChannelDeploymentAsync(Resources.Channel channel)
        {
            V1Deployment deployment;
            try
            {
                this.Logger.LogInformation("Creating a new deployment for the channel with name '{resourceName}'...", channel.Name());
                V1PodSpec podSpec = new V1PodSpec();
                V1Container container = channel.Spec.Container;
                container.Env.Add(new V1EnvVar("SINK", $"http://gateway.{channel.Namespace()}.svc.cluster.local/events/"));
                podSpec.Containers = new List<V1Container>() { container };
                V1ObjectMeta podMetadata = new V1ObjectMeta();
                podMetadata.Annotations = new Dictionary<string, string>() { { "sidecar.istio.io/inject", "true" } };
                V1PodTemplateSpec podTemplateSpec = new V1PodTemplateSpec(podMetadata, podSpec);
                podTemplateSpec.Metadata.Labels = new Dictionary<string, string>()
                {
                    { "app", channel.Name() },
                    { "version", "1.0" }
                };
                V1DeploymentSpec deploymentSpec = new V1DeploymentSpec(new V1LabelSelector(), podTemplateSpec);
                deploymentSpec.Selector.MatchLabels = new Dictionary<string, string>()
                {
                    { "app", channel.Name() },
                    { "version", "1.0" }
                };
                V1ObjectMeta deploymentMetadata = new V1ObjectMeta(namespaceProperty: channel.Namespace(), name: channel.Name());
                deploymentMetadata.Labels = new Dictionary<string, string>() { { "type", EventingDefaults.Labels.Channel } };
                deploymentMetadata.Name = channel.Name();
                deploymentMetadata.NamespaceProperty = channel.Namespace();
                deployment = new V1Deployment(KubernetesDefaults.ApiVersions.AppsV1, KubernetesDefaults.Kinds.Deployment, deploymentMetadata, deploymentSpec);
                await this.KubernetesClient.CreateNamespacedDeploymentAsync(deployment, channel.Namespace());
                this.Logger.LogInformation("A new deployment for the channel with name '{resourceName}' has been successfully created.", channel.Name());
            }
            catch(HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while creating the deployment for the channel with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channel.Name(), ex.Response.StatusCode, ex.Response.Content);
                throw ex;
            }
        }

        /// <summary>
        /// Creates a new <see cref="V1Service"/> for the specified <see cref="Resources.Channel"/>
        /// </summary>
        /// <param name="channel">The <see cref="Resources.Channel"/> to deploy</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task CreateChannelServiceAsync(Resources.Channel channel)
        {
            V1Service service;
            try
            {
                this.Logger.LogInformation("Creating a new service for the channel with name '{resourceName}'...", channel.Name());
                V1ObjectMeta serviceMetadata = new V1ObjectMeta();
                serviceMetadata.Name = channel.Name();
                serviceMetadata.NamespaceProperty = channel.Namespace();
                serviceMetadata.Labels = new Dictionary<string, string>() 
                { 
                    { "app", channel.Name() },
                    { "type", "channel" }
                };
                V1ServiceSpec serviceSpec = new V1ServiceSpec();
                serviceSpec.Ports = new List<V1ServicePort>() { new V1ServicePort(80, name: "http") };
                serviceSpec.Selector = new Dictionary<string, string>() { { "app", channel.Name() } };
                service = new V1Service(KubernetesDefaults.ApiVersions.V1, KubernetesDefaults.Kinds.Service, serviceMetadata, serviceSpec);
                await this.KubernetesClient.CreateNamespacedServiceAsync(service, channel.Namespace());
                this.Logger.LogInformation("A new service for the channel with name '{resourceName}' has been successfully created.", channel.Name());
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while creating the service for the channel with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channel.Name(), ex.Response.StatusCode, ex.Response.Content);
                throw ex;
            }
        }

        /// <summary>
        /// Registers the specified <see cref="Resources.Channel"/>
        /// </summary>
        /// <param name="channel">The <see cref="Resources.Channel"/> to register</param>
        protected virtual void RegisterChannel(Resources.Channel channel)
        {
            this.ChannelManager.RegisterChannel(channel.Name(), new Uri($"http://{channel.Name()}/events/"));
        }

        /// <summary>
        /// Deletes an existing <see cref="Resources.Channel"/>
        /// </summary>
        /// <param name="channel">The <see cref="Resources.Channel"/> to delete</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeleteChannelAsync(Resources.Channel channel)
        {
            try
            {
                this.UnregisterChannel(channel);
                await this.DeleteChannelServiceAsync(channel);
                await this.DeleteChannelDeploymentAsync(channel);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Unregisters the specified <see cref="Resources.Channel"/>
        /// </summary>
        /// <param name="channel">The <see cref="Resources.Channel"/> to unregister</param>
        protected virtual void UnregisterChannel(Resources.Channel channel)
        {
            this.ChannelManager.UnregisterChannel(channel.Name());
        }

        /// <summary>
        /// Deletes the <see cref="V1Service"/> for the specified <see cref="Resources.Channel"/>
        /// </summary>
        /// <param name="channel">The <see cref="Resources.Channel"/> to delete</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeleteChannelServiceAsync(Resources.Channel channel)
        {
            try
            {
                this.Logger.LogInformation("Deleting the service for the channel with name '{resourceName}'...", channel.Name());
                await this.KubernetesClient.DeleteNamespacedServiceAsync(channel.Name(), channel.Namespace());
                this.Logger.LogInformation("The service for the channel with name '{resourceName}' has been successfully deleted.", channel.Name());
            }
            catch(HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while deleting the service for the channel with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channel.Name(), ex.Response.StatusCode, ex.Response.Content);
                throw ex;
            }
        }

        /// <summary>
        /// Deletes the <see cref="V1Deployment"/> for the specified <see cref="Resources.Channel"/>
        /// </summary>
        /// <param name="channel">The <see cref="Resources.Channel"/> to delete</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeleteChannelDeploymentAsync(Resources.Channel channel)
        {
            try
            {
                this.Logger.LogInformation("Deleting the deployment for the channel with name '{resourceName}'...", channel.Name());
                await this.KubernetesClient.DeleteNamespacedDeploymentAsync(channel.Name(), channel.Namespace());
                this.Logger.LogInformation("The deployment for the channel with name '{resourceName}' has been successfully deleted.", channel.Name());
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while deleting the deployment for the channel with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channel.Name(), ex.Response.StatusCode, ex.Response.Content);
                throw ex;
            }
        }

        #endregion

        #region Subscriptions

        /// <summary>
        /// Handles a <see cref="Resources.Subscription"/> event
        /// </summary>
        /// <param name="e">The type of event to handle</param>
        /// <param name="subscription">The <see cref="Resources.Subscription"/> the event to handle applies to</param>
        protected virtual async void OnSubscriptionEvent(WatchEventType e, Resources.Subscription subscription)
        {
            this.Logger.LogInformation("A new event of type '{watchEventType}' concerning the CRD of kind '{crdKind}' with name '{crdName}' has been received.", e, subscription.Kind, subscription.Name());
            switch (e)
            {
                case WatchEventType.Added:
                    await this.SubscribeToChannelAsync(subscription);
                    break;
                case WatchEventType.Modified:

                    break;
                case WatchEventType.Deleted:
                    await this.UnsubscribeFromChannelAsync(subscription);
                    break;
                case WatchEventType.Bookmark:

                    break;
                case WatchEventType.Error:

                    break;
                default:
                    throw new NotSupportedException($"The specified watch event type '{e}' is not supported");
            }
        }

        /// <summary>
        /// Creates the specified <see cref="Resources.Subscription"/> on the <see cref="Resources.Channel"/> it applies to
        /// </summary>
        /// <param name="subscription">The <see cref="Resources.Subscription"/> to create</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task SubscribeToChannelAsync(Resources.Subscription subscription)
        {
            if (!this.ChannelManager.TryGetChannel(subscription.Spec.Channel, out IChannel channel))
                return;
            SubscriptionOptionsDto subscriptionOptions = new SubscriptionOptionsDto()
            {
                Id = subscription.Spec.Id,
                DurableName = subscription.Spec.IsDurable ? subscription.Spec.Id : null,
                Subject = subscription.Spec.Subject
            };
            await channel.SubscribeAsync(subscriptionOptions);
            this.RegisterSubscription(subscription);
        }

        /// <summary>
        /// Registers the specified <see cref="Resources.Subscription"/>
        /// </summary>
        /// <param name="subscription">The <see cref="Resources.Subscription"/> to register</param>
        protected virtual void RegisterSubscription(Resources.Subscription subscription)
        {
            this.SubscriptionManager.RegisterSubscription(subscription.Spec.Id, subscription.Spec.Subject, subscription.Spec.Type, subscription.Spec.Source, subscription.Spec.Channel, subscription.Spec.Subscriber.Select(s => s.Uri));
        }

        /// <summary>
        /// Deletes the specified <see cref="Resources.Subscription"/> from the <see cref="Resources.Channel"/> it applies to
        /// </summary>
        /// <param name="subscription">The <see cref="Resources.Subscription"/> to delete</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task UnsubscribeFromChannelAsync(Resources.Subscription subscription)
        {
            if (!this.ChannelManager.TryGetChannel(subscription.Spec.Channel, out IChannel channel))
                return;
            await channel.UnsubscribeAsync(subscription.Spec.Id);
        }

        /// <summary>
        /// Unregisters the specified <see cref="Resources.Subscription"/>
        /// </summary>
        /// <param name="subscription">The <see cref="Resources.Subscription"/> to unregister</param>
        protected virtual void UnregisterSubscription(Resources.Subscription subscription)
        {
            this.SubscriptionManager.UnregisterSubscription(subscription.Spec.Id);
        }

        #endregion

        #region Brokers

        /// <summary>
        /// Handles a <see cref="Broker"/> event
        /// </summary>
        /// <param name="e">The type of event to handle</param>
        /// <param name="broker">The <see cref="Broker"/> the event to handle applies to</param>
        protected virtual async void OnBrokerEvent(WatchEventType e, Broker broker)
        {
            this.Logger.LogInformation("A new event of type '{watchEventType}' concerning the CRD of kind '{crdKind}' with name '{crdName}' has been received.", e, broker.Kind, broker.Name());
            switch (e)
            {
                case WatchEventType.Added:
                    await this.DeployBrokerAsync(broker);
                    break;
                case WatchEventType.Modified:

                    break;
                case WatchEventType.Deleted:
                    await this.DeleteBrokerAsync(broker);
                    break;
                case WatchEventType.Bookmark:

                    break;
                case WatchEventType.Error:

                    break;
                default:
                    throw new NotSupportedException($"The specified watch event type '{e}' is not supported");
            }
        }

        /// <summary>
        /// Deploys the specified <see cref="Broker"/>
        /// </summary>
        /// <param name="broker">The <see cref="Broker"/> to deploy</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeployBrokerAsync(Broker broker)
        {
            try
            {
                await this.CreateBrokerExternalNameServiceAsync(broker);
                await this.CreateBrokerVirtualServiceAsync(broker);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Creates an external name <see cref="V1Service"/> for the specified <see cref="Broker"/>
        /// </summary>
        /// <param name="broker">The <see cref="Broker"/> to deploy</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task CreateBrokerExternalNameServiceAsync(Broker broker)
        {
            V1Service service;
            try
            {
                this.Logger.LogInformation("Creating a new external name service for the broker with name '{resourceName}'...", broker.Name());
                V1ObjectMeta metadata = new V1ObjectMeta(name: broker.Name());
                V1ServiceSpec spec = new V1ServiceSpec()
                {
                    Type = KubernetesDefaults.ServiceTypes.ExternalName,
                    ExternalName = $"gateway.{this.Options.Pod.Namespace}.svc.cluster.local",
                    Ports = new List<V1ServicePort>()
                    {
                        new V1ServicePort(80, name: "http")
                    }
                };
                service = new V1Service(KubernetesDefaults.ApiVersions.V1, KubernetesDefaults.Kinds.Service, metadata, spec);
                await this.KubernetesClient.CreateNamespacedServiceAsync(service, broker.Namespace());
                this.Logger.LogInformation("A new external name service for the broker with name '{resourceName}' has been successfully created.", broker.Name());
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while creating the external name service for the broker with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", broker.Name(), ex.Response.StatusCode, ex.Response.Content);
                return;
            }
        }

        /// <summary>
        /// Creates a virtual <see cref="V1Service"/> for the specified <see cref="Broker"/>
        /// </summary>
        /// <param name="broker">The <see cref="Broker"/> to deploy</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task CreateBrokerVirtualServiceAsync(Broker broker)
        {
            VirtualService virtualService;
            try
            {
                this.Logger.LogInformation("Creating a new istio virtual service for the broker with name '{resourceName}'...", broker.Name());
                V1ObjectMeta metadata = new V1ObjectMeta(name: $"{broker.Name()}-vs");
                VirtualServiceSpec spec = new VirtualServiceSpec()
                {
                    Hosts = new List<string>() { broker.Name() },
                    Http = new List<HttpRoute>() 
                    { 
                        new HttpRoute()
                        {
                            Headers = new Headers()
                            {
                                Request = new HeadersOperations()
                                {
                                    Add = new Dictionary<string, string>()
                                    {
                                        { EventingDefaults.Headers.Channel, broker.Spec.Channel }
                                    }
                                }
                            },
                            Route = new List<HttpRouteDestination>()
                            {
                                new HttpRouteDestination()
                                {
                                    Destination = new Destination(broker.Name())
                                    {
                                        Port = new PortSelector(80)
                                    }
                                }
                            }
                        } 
                    }
                };
                virtualService = new VirtualService(metadata, spec);
                await this.KubernetesClient.CreateNamespacedCustomObjectAsync(virtualService, broker.Namespace());
                this.Logger.LogInformation("A new istio virtual service for the broker with name '{resourceName}' has been successfully created.", broker.Name());
            }
            catch(HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while creating the istio virtual service for the broker with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", broker.Name(), ex.Response.StatusCode, ex.Response.Content);
                return;
            }
            try
            {
                this.Logger.LogInformation("Updating the status of the broker with name '{resourceName}'...", broker.Name());
                broker.Status = new BrokerStatus() { Url = $"{virtualService.Name()}.{virtualService.Namespace()}.svc.cluster.{virtualService.Metadata.ClusterName}" };
                await this.KubernetesClient.ReplaceNamespacedCustomObjectStatusAsync(broker, broker.ApiGroup(), broker.ApiGroupVersion(), broker.Namespace(), BrokerDefinition.PLURAL, broker.Name());
                this.Logger.LogInformation("The status of the broker with name '{resourceName}' has been successfully updated", broker.Name());
            }
            catch(HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while updating the status of the CRD '{{resourceKind}}' with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", broker.Kind, broker.Name(), ex.Response.StatusCode, ex.Response.Content);
            }
        }

        /// <summary>
        /// Deletes the specified <see cref="Broker"/>
        /// </summary>
        /// <param name="broker">The <see cref="Broker"/> to delete</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeleteBrokerAsync(Broker broker)
        {
            try
            {
                await this.DeleteBrokerVirtualServiceAsync(broker);
                await this.DeleteBrokerExternalNameServiceAsync(broker);
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// Deletes the external name <see cref="V1Service"/> for the specified <see cref="Broker"/>
        /// </summary>
        /// <param name="broker">The <see cref="Broker"/> to delete</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeleteBrokerVirtualServiceAsync(Broker broker)
        {
            try
            {
                this.Logger.LogInformation("Deleting the virtual service for the broker with name '{resourceName}'...", broker.Name());
                await this.KubernetesClient.DeleteNamespacedServiceAsync(broker.Name(), broker.Namespace());
                this.Logger.LogInformation("The virtual service for the broker with name '{resourceName}' has been successfully deleted.", broker.Name());
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while deleting the virtual service for the broker with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", broker.Name(), ex.Response.StatusCode, ex.Response.Content);
                return;
            }
        }

        /// <summary>
        /// Deletes the virtual <see cref="V1Service"/> for the specified <see cref="Broker"/>
        /// </summary>
        /// <param name="broker">The <see cref="Broker"/> to delete</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeleteBrokerExternalNameServiceAsync(Broker broker)
        {
            try
            {
                this.Logger.LogInformation("Deleting the external name service for the broker with name '{resourceName}'...", broker.Name());
                await this.KubernetesClient.DeleteNamespacedServiceAsync($"{broker.Name()}-vs", broker.Namespace());
                this.Logger.LogInformation("The external name service for the broker with name '{resourceName}' has been successfully deleted.", broker.Name());
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while deleting the external name service for the broker with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", broker.Name(), ex.Response.StatusCode, ex.Response.Content);
                return;
            }
        }

        #endregion

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="ResourceController"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceController"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    foreach (ICustomResourceEventWatcher eventWatcher in this.EventWatchers)
                    {
                        try
                        {
                            eventWatcher.Dispose();
                        }
                        catch { }
                    }
                    this._StoppingTokenSource.Cancel();
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
