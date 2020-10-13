using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Gateway.Integration.Commands;
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

        public async Task<string> SubscribeToAsync(string subject, CancellationToken cancellationToken = default)
        {
            var channel = Environment.GetEnvironmentVariable("CHANNEL");
            var podName = Environment.GetEnvironmentVariable("POD_NAME");
            var podNamespace = Environment.GetEnvironmentVariable("POD_NAMESPACE");
            var createSubscriptionCommand = new CreateSubscriptionCommandDto
            {
                Subject = subject,
                Channel = channel,
                Subscribers = new List<Uri>()
                {
                    new Uri($"http://{podName}.{podNamespace}.svc.cluster.local")
                }
            };
            string subscriptionId;
            using (HttpResponseMessage response = await this.HttpClient.PostAsJsonAsync("sub", createSubscriptionCommand))
            {
                response.EnsureSuccessStatusCode();
                subscriptionId = await response?.Content.ReadAsStringAsync();
                this.Logger.LogInformation("A cloud event subscription to subject '{subject}' has been successfully created", subject);
            }
            return subscriptionId;
        }

        public async Task UnsubscribeFromAsync(string subscriptionId, CancellationToken cancellationToken = default)
        {
            using (HttpResponseMessage response = await this.HttpClient.DeleteAsync($"unsub?subscriptionId={subscriptionId}"))
            {
                response.EnsureSuccessStatusCode();
                this.Logger.LogInformation("The cloud event subscription with id '{subscriptionId}' has been successfully deleted", subscriptionId);
            }
        }

    }

}
