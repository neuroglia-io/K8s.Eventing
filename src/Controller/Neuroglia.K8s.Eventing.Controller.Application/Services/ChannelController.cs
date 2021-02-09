using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Neuroglia.K8s.Eventing.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Controller.Application.Services
{

    /// <summary>
    /// Represents the service used to control <see cref="ChannelResource"/>s
    /// </summary>
    public class ChannelController
        : Controller<V1ChannelResource>
    {

        /// <summary>
        /// Initializes a new <see cref="ChannelController"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="kubernetes">The service used to interact with the Kubernetes API</param>
        /// <param name="resources">The service used to watch <see cref="ChannelResource"/>s</param>
        public ChannelController(ILoggerFactory loggerFactory, IKubernetes kubernetes, ICustomResourceWatcher<V1ChannelResource> resources)
            : base(loggerFactory, kubernetes, resources)
        {

        }

        /// <inheritdoc/>
        protected override void OnEvent(IResourceEvent<V1ChannelResource> e)
        {
            base.OnEvent(e);
            switch (e.Type)
            {
                case WatchEventType.Added:
                    _ = this.CreateChannelDeploymentAsync(e.Resource);
                    _ = this.CreateChannelServiceAsync(e.Resource);
                    break;
                case WatchEventType.Deleted:

                    break;
            }
        }

        /// <summary>
        /// Creates a new <see cref="V1Deployment"/> for the specified <see cref="ChannelResource"/>
        /// </summary>
        /// <param name="channel">The <see cref="ChannelResource"/> to deploy</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task CreateChannelDeploymentAsync(V1ChannelResource channel)
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
                await this.Kubernetes.CreateNamespacedDeploymentAsync(deployment, channel.Namespace());
                this.Logger.LogInformation("A new deployment for the channel with name '{resourceName}' has been successfully created.", channel.Name());
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while creating the deployment for the channel with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channel.Name(), ex.Response.StatusCode, ex.Response.Content);
                throw;
            }
        }

        /// <summary>
        /// Creates a new <see cref="V1Service"/> for the specified <see cref="ChannelResource"/>
        /// </summary>
        /// <param name="channel">The <see cref="ChannelResource"/> to deploy</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task CreateChannelServiceAsync(V1ChannelResource channel)
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
                await this.Kubernetes.CreateNamespacedServiceAsync(service, channel.Namespace());
                this.Logger.LogInformation("A new service for the channel with name '{resourceName}' has been successfully created.", channel.Name());
            }
            catch (HttpOperationException ex)
            {
                this.Logger.LogError($"An error occured while creating the service for the channel with name '{{resourceName}}': the server responded with a non-success status code '{{statusCode}}'.{Environment.NewLine}Details: {{responseContent}}", channel.Name(), ex.Response.StatusCode, ex.Response.Content);
                throw;
            }
        }

    }

}
