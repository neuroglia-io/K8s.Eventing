using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Gateway.Integration.Commands;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using Neuroglia.Mediation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GatewayClient.Services
{

    public class EventBus
        : IEventBus
    {

        public EventBus(ILogger<EventBus> logger, HttpClient httpClient)
        {
            this.Logger = logger;
            this.HttpClient = httpClient;
        }

        protected ILogger Logger { get; }

        protected HttpClient HttpClient { get; }

        public async Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default)
        {
            using(HttpResponseMessage response  = await this.HttpClient.PostAsync("pub", new CloudEventContent(e, ContentMode.Structured, new JsonEventFormatter())))
            {
                response.EnsureSuccessStatusCode();
                this.Logger.LogInformation("A cloud event with type '{type}' and subject '{subject}' has been successfully published", e.Type, e.Subject);
            }
        }

        public async Task<SubscriptionDto> SubscribeToAsync(string subject, CancellationToken cancellationToken = default)
        {
            var channel = Environment.GetEnvironmentVariable("CHANNEL");
            var podName = Environment.GetEnvironmentVariable("POD_NAME");
            var podNamespace = Environment.GetEnvironmentVariable("POD_NAMESPACE");
            var subscriberUri = new Uri($"http://{podName}.{podNamespace}.svc.cluster.local/events");
            var createSubscriptionCommand = new CreateSubscriptionCommandDto
            {
                Subject = subject,
                Channel = channel,
                Subscribers = new List<Uri>() { subscriberUri }
            };
            SubscriptionDto subscription;
            using (HttpResponseMessage response = await this.HttpClient.PostAsJsonAsync("sub", createSubscriptionCommand))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response?.Content.ReadAsStringAsync();
                    throw new OperationException($"The remote server responded with a non-success status code '{response.StatusCode}'.{Environment.NewLine}Details: {content}");
                }
                subscription = JsonConvert.DeserializeObject<SubscriptionDto>(await response?.Content.ReadAsStringAsync());
                this.Logger.LogInformation("A cloud event subscription to subject '{subject}' has been successfully created", subject);
            }
            return subscription;
        }

        public async Task UnsubscribeFromAsync(string subscriptionId, CancellationToken cancellationToken = default)
        {
            using (HttpResponseMessage response = await this.HttpClient.DeleteAsync($"unsub?subscriptionId={subscriptionId}"))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response?.Content.ReadAsStringAsync();
                    throw new OperationException($"The remote server responded with a non-success status code '{response.StatusCode}'.{Environment.NewLine}Details: {content}");
                }
                this.Logger.LogInformation("The cloud event subscription with id '{subscriptionId}' has been successfully deleted", subscriptionId);
            }
        }

    }

}
