using k8s;
using k8s.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Neuroglia.K8s.Eventing.Kafka.Resources;
using Neuroglia.K8s.Eventing.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Neuroglia.K8s.Eventing.Kafka.Controller.Services
{

    /// <summary>
    /// Represents the service used to manage <see cref="V1KafkaChannelResource"/>s
    /// </summary>
    public class KafkaChannelController
        : Controller<V1KafkaChannelResource>, IHostedService
    {

        /// <summary>
        /// Initializes a new <see cref="KafkaChannelController"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="kubernetes">The service used to interact with the Kubernetes API</param>
        /// <param name="resources">The service used to watch <see cref="V1KafkaChannelResource"/>s</param>
        /// <param name="yamlDeserializer">The service used to deserialize YAML</param>
        public KafkaChannelController(ILoggerFactory loggerFactory, IKubernetes kubernetes, ICustomResourceWatcher<V1KafkaChannelResource> resources, IDeserializer yamlDeserializer) 
            : base(loggerFactory, kubernetes, resources)
        {
            this.YamlDeserializer = yamlDeserializer;
        }

        /// <summary>
        /// Gets the service used to deserialize YAML
        /// </summary>
        protected IDeserializer YamlDeserializer { get; }

        /// <inheritdoc/>
        protected override void OnEvent(IResourceEvent<V1KafkaChannelResource> e)
        {
            base.OnEvent(e);
            switch (e.Type)
            {
                case WatchEventType.Added:
                    _ = this.CreateChannelAsync(e.Resource);
                    break;
                case WatchEventType.Deleted:
                    _ = this.DeleteChannelAsync(e.Resource);
                    break;
            }
        }

        /// <summary>
        /// Creates a new channel based on the specified <see cref="V1KafkaChannelResource"/>
        /// </summary>
        /// <param name="channelResource">The <see cref="V1KafkaChannelResource"/> based on which to create a new channel</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task CreateChannelAsync(V1KafkaChannelResource channelResource)
        {
            if (channelResource == null)
                throw new ArgumentNullException(nameof(channelResource));
            await this.CreateChannelDeploymentAsync(channelResource);
            await this.CreateChannelServiceAsync(channelResource);
            await this.UpdateChannelStatusAsync(channelResource, status =>
            {
                status.Operational = true;
                status.Address = new Uri($"http://{channelResource.Metadata.Name}.{channelResource.Metadata.Namespace()}");
                status.Endpoints = new V1ChannelEndpoints()
                {
                    Pub = "/events/pub",
                    Sub = "/events/sub"
                };
            });
        }

        /// <summary>
        /// Creates a new deployment for the specified <see cref="V1KafkaChannelResource"/>
        /// </summary>
        /// <param name="channelResource">The <see cref="V1KafkaChannelResource"/> to create a new deployment for</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task CreateChannelDeploymentAsync(V1KafkaChannelResource channelResource)
        {
            if (channelResource == null)
                throw new ArgumentNullException(nameof(channelResource));
            try
            {
                this.Logger.LogInformation("Creating a new deployment for channel with name '{channel}'...", channelResource.Metadata.Name);
                V1PodSpec podSpec = new V1PodSpec();
                //Retrieves the container spec
                V1Container container = await this.LoadContainerAsync();
                if (container.Env == null)
                    container.Env = new List<V1EnvVar>();
                //Configures the container's Kafka client configuration
                container.Env.Add(new V1EnvVar("KAFKA__SERVERS", string.Join(",", channelResource.Spec.Servers)));
                //Adds the container spec
                podSpec.Containers = new List<V1Container>() { container };
                //Sets annotations
                V1ObjectMeta podMetadata = new V1ObjectMeta();
                podMetadata.Annotations = new Dictionary<string, string>() { { "sidecar.istio.io/inject", "true" } };
                //Sets labels
                V1PodTemplateSpec podTemplateSpec = new V1PodTemplateSpec(podMetadata, podSpec);
                podTemplateSpec.Metadata.Labels = new Dictionary<string, string>()
                {
                    { "app", channelResource.Name() },
                    { "version", "1.0" }
                };
                //Sets selectors
                V1DeploymentSpec deploymentSpec = new V1DeploymentSpec(new V1LabelSelector(), podTemplateSpec);
                deploymentSpec.Selector.MatchLabels = new Dictionary<string, string>()
                {
                    { "app", channelResource.Name() },
                    { "version", "1.0" }
                };
                //Set deployment metadata
                V1ObjectMeta deploymentMetadata = new V1ObjectMeta(namespaceProperty: channelResource.Namespace(), name: channelResource.Name());
                deploymentMetadata.Name = channelResource.Name();
                deploymentMetadata.NamespaceProperty = channelResource.Namespace();
                //Configures deployment
                V1Deployment deployment = new V1Deployment(KubernetesDefaults.ApiVersions.AppsV1, KubernetesDefaults.Kinds.Deployment, deploymentMetadata, deploymentSpec);
                await this.Kubernetes.CreateNamespacedDeploymentAsync(deployment, channelResource.Namespace());
                this.Logger.LogInformation("The deployment for channel with name '{channel}' has been successfully created.", channelResource.Metadata.Name);
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while creating the deployment for the channel with name '{{channel}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channelResource.Name(), ex.Response.StatusCode, ex.Response.Content);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"An error occured while creating the deployment for the channel with name '{{channel}}'.{Environment.NewLine}Details: {{ex}}", channelResource.Name(), ex.Message);
            }
        }

        /// <summary>
        /// Creates a new service for the specified <see cref="V1KafkaChannelResource"/>
        /// </summary>
        /// <param name="channelResource">The <see cref="V1KafkaChannelResource"/> to create a new service for</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task CreateChannelServiceAsync(V1KafkaChannelResource channelResource)
        {
            if (channelResource == null)
                throw new ArgumentNullException(nameof(channelResource));
            try
            {
                this.Logger.LogInformation("Creating a new service for the channel with name '{resourceName}'...", channelResource.Name());
                V1ObjectMeta serviceMetadata = new V1ObjectMeta();
                serviceMetadata.Name = channelResource.Name();
                serviceMetadata.NamespaceProperty = channelResource.Namespace();
                serviceMetadata.Labels = new Dictionary<string, string>()
                {
                    { "app", channelResource.Name() },
                    { "type", "channel" }
                };
                V1ServiceSpec serviceSpec = new V1ServiceSpec();
                serviceSpec.Ports = new List<V1ServicePort>() { new V1ServicePort(80, name: "http") };
                serviceSpec.Selector = new Dictionary<string, string>() { { "app", channelResource.Name() } };
                V1Service service = new V1Service(KubernetesDefaults.ApiVersions.V1, KubernetesDefaults.Kinds.Service, serviceMetadata, serviceSpec);
                await this.Kubernetes.CreateNamespacedServiceAsync(service, channelResource.Namespace());
                this.Logger.LogInformation("A new service for the channel with name '{resourceName}' has been successfully created.", channelResource.Name());
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while creating the service for the channel with name '{{channel}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channelResource.Name(), ex.Response.StatusCode, ex.Response.Content);
                throw;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"An error occured while creating the service for the channel with name '{{channel}}'.{Environment.NewLine}Details: {{ex}}", channelResource.Name(), ex.Message);
            }
        }

        /// <summary>
        /// Sets the status of the specified <see cref="V1KafkaChannelResource"/>
        /// </summary>
        /// <param name="channelResource">The <see cref="V1KafkaChannelResource"/> to set the status for</param>
        /// <param name="updateAction">The <see cref="Action{T}"/> used to update the specified <see cref="V1KafkaChannelResource"/>'s <see cref="V1ChannelStatus"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task UpdateChannelStatusAsync(V1KafkaChannelResource channelResource, Action<V1ChannelStatus> updateAction)
        {
            try
            {
                this.Logger.LogInformation("Updating the status of the channel with name '{channel}'...", channelResource.Metadata.Name);
                if (channelResource.Status == null)
                    channelResource.Status = new V1ChannelStatus();
                updateAction(channelResource.Status);
                await this.Kubernetes.ReplaceNamespacedCustomObjectStatusAsync(channelResource, channelResource.Definition.Group, channelResource.Definition.Version, channelResource.Metadata.Namespace(), channelResource.Definition.Plural, channelResource.Metadata.Name);
                this.Logger.LogInformation("The status of the channel with name '{channel}' has been successfully updated.", channelResource.Metadata.Name);
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while updating the status of the channel with name '{{channel}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channelResource.Name(), ex.Response.StatusCode, ex.Response.Content);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"An error occured while updating the status of the channel with name '{{channel}}'.{Environment.NewLine}Details: {{ex}}", channelResource.Name(), ex.Message);
            }
        }

        /// <summary>
        /// Deletes the channel described by the <see cref="V1KafkaChannelResource"/>
        /// </summary>
        /// <param name="channelResource">The <see cref="V1KafkaChannelResource"/> that describes the channel to delete</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeleteChannelAsync(V1KafkaChannelResource channelResource)
        {
            if (channelResource == null)
                throw new ArgumentNullException(nameof(channelResource));
            await this.DeleteChannelServiceAsync(channelResource);
            await this.DeleteChannelDeploymentAsync(channelResource);
        }

        /// <summary>
        /// Deletes the deployment of the specified <see cref="V1KafkaChannelResource"/>'s service
        /// </summary>
        /// <param name="channelResource">The <see cref="V1KafkaChannelResource"/> to delete the service of</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeleteChannelServiceAsync(V1KafkaChannelResource channelResource)
        {
            if (channelResource == null)
                throw new ArgumentNullException(nameof(channelResource));
            try
            {
                this.Logger.LogInformation("Deleting the service for the channel with name '{resourceName}'...", channelResource.Name());
                await this.Kubernetes.DeleteNamespacedServiceAsync(channelResource.Name(), channelResource.Namespace());
                this.Logger.LogInformation("The service for the channel with name '{resourceName}' has been successfully deleted.", channelResource.Name());
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while deleting the service for the channel with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channelResource.Name(), ex.Response.StatusCode, ex.Response.Content);
                throw;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"An error occured while deleting the service for the channel with name '{{resourceName}}'.{Environment.NewLine}Details: {{ex}}", channelResource.Name(), ex.Message);
            }
        }

        /// <summary>
        /// Deletes the deployment of the specified <see cref="V1KafkaChannelResource"/>'s deployment
        /// </summary>
        /// <param name="channelResource">The <see cref="V1KafkaChannelResource"/> to delete the deployment of</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DeleteChannelDeploymentAsync(V1KafkaChannelResource channelResource)
        {
            if (channelResource == null)
                throw new ArgumentNullException(nameof(channelResource));
            try
            {
                this.Logger.LogInformation("Deleting the deployment for the channel with name '{resourceName}'...", channelResource.Name());
                await this.Kubernetes.DeleteNamespacedDeploymentAsync(channelResource.Name(), channelResource.Namespace());
                this.Logger.LogInformation("The deployment for the channel with name '{resourceName}' has been successfully deleted.", channelResource.Name());
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while deleting the deployment for the channel with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channelResource.Name(), ex.Response.StatusCode, ex.Response.Content);
                throw;
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"An error occured while deleting the deployment for the channel with name '{{resourceName}}'.{Environment.NewLine}Details: {{ex}}", channelResource.Name(), ex.Message);
            }
        }

        /// <summary>
        /// Loads the <see cref="V1Container"/> to use when creating new channel deployments
        /// </summary>
        /// <returns>A <see cref="V1Container"/> to use when creating new channel deployments</returns>
        protected virtual async Task<V1Container> LoadContainerAsync()
        {
            return await Task.FromResult(this.YamlDeserializer.Deserialize<V1Container>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Resources", "container.yaml"))));

        }

        /// <inheritdoc/>
        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            await this.Resources.StopAsync(cancellationToken);
        }

    }

}
