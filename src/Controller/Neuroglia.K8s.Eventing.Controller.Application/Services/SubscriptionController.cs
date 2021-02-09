using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Neuroglia.K8s.Eventing.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Controller.Application.Services
{

    /// <summary>
    /// Represents the service used to control <see cref="V1BrokerResource"/>s
    /// </summary>
    public class SubscriptionController
        : Controller<V1SubscriptionResource>
    {

        /// <summary>
        /// Initializes a new <see cref="SubscriptionController"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="kubernetes">The service used to interact with the Kubernetes API</param>
        /// <param name="resources">The service used to watch <see cref="V1SubscriptionResource"/>s</param>
        public SubscriptionController(ILoggerFactory loggerFactory, IKubernetes kubernetes, ICustomResourceWatcher<V1SubscriptionResource> resources)
            : base(loggerFactory, kubernetes, resources)
        {

        }

        /// <inheritdoc/>
        protected override void OnEvent(IResourceEvent<V1SubscriptionResource> e)
        {
            base.OnEvent(e);
            switch (e.Type)
            {
                case WatchEventType.Added:
                    _= this.AttachToChannelAsync(e.Resource);
                    break;
                case WatchEventType.Deleted:

                    break;
            }
        }

        protected virtual async Task AttachToChannelAsync(V1SubscriptionResource subscriptionResource)
        {
            try
            {
                V1CustomResourceDefinitionList definitionList = await this.Kubernetes.ListCustomResourceDefinitionAsync();
                List<Func<CancellationToken, Task<(V1CustomResourceDefinition Definition, object Resource)>>> tasks = new List<Func<CancellationToken, Task<(V1CustomResourceDefinition, object)>>>(definitionList.Items.Count);
                foreach (V1CustomResourceDefinition definition in definitionList.Items)
                {
                    tasks.Add(cancellationToken => Task.Run<(V1CustomResourceDefinition, object)>(async () =>
                    {
                        try
                        {
                            JObject result = await this.Kubernetes.ListClusterCustomObjectAsync(definition.Spec.Group, definition.Spec.Versions.Last().Name, definition.Spec.Names.Plural, fieldSelector: $"metadata.name == {subscriptionResource.Spec.Channel}", cancellationToken: cancellationToken) as JObject;
                            KubernetesList<V1ChannelResource> channels = result.ToObject<KubernetesList<V1ChannelResource>>();
                            return (definition, channels.Items.FirstOrDefault());
                        }
                        catch
                        {
                            return (definition, null);
                        }
                    }, cancellationToken));
                }
                var result = await TaskUtilities.WhenAny(tasks, result => result.Resource != null);
                if (result.Resource == null)
                    return;
                V1ChannelResource channelResource = ((JObject)result.Resource).ToObject<V1ChannelResource>();
                channelResource.Status.Subscriptions.Add(subscriptionResource);
                await this.Kubernetes.ReplaceNamespacedCustomObjectStatusAsync(channelResource, result.Definition.Spec.Group, result.Definition.Spec.Versions.Last().Name, channelResource.Namespace(), result.Definition.Spec.Names.Plural, channelResource.Name());
            }
            catch(HttpOperationException ex)
            {

            }
            catch(Exception ex)
            {

            }
        }

        protected virtual async Task UpdateStatusAsync(V1SubscriptionResource subscriptionResource, Action<V1SubscriptionStatus> updateStatus)
        {
            try
            {
                if (subscriptionResource.Status == null)
                    subscriptionResource.Status = new V1SubscriptionStatus();
                subscriptionResource.Status.Ready = true;
                await this.Kubernetes.ReplaceNamespacedCustomObjectStatusAsync(subscriptionResource, subscriptionResource.Definition.Group, subscriptionResource.Definition.Version, subscriptionResource.Metadata.Namespace(), subscriptionResource.Definition.Plural, subscriptionResource.Metadata.Name);
            }
            catch (HttpOperationException ex)
            {

            }
            catch (Exception ex)
            {

            }
        }

        protected virtual async Task DetachFromChannelAsync(V1SubscriptionResource subscriptionResource)
        {

        }

    }
}
