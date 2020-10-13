using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IChannel"/> interface
    /// </summary>
    public class Channel
        : IChannel
    {

        /// <summary>
        /// Initializes a new <see cref="Channel"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="httpClientFactory">The service used to create <see cref="HttpClient"/> instances</param>
        /// <param name="name">The <see cref="Channel"/>'s name</param>
        /// <param name="address">The <see cref="Channel"/>'s remote address</param>
        public Channel(ILogger<Channel> logger, IHttpClientFactory httpClientFactory, string name, Uri address)
        {
            this.Logger = logger;
            this.HttpClient = httpClientFactory.CreateClient(typeof(Channel).Name);
            this.HttpClient.BaseAddress = address;
            this.Name = name;
            this.Address = address;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the <see cref="System.Net.Http.HttpClient"/> used to request the remote channel
        /// </summary>
        protected HttpClient HttpClient { get; private set; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Uri Address { get; }

        /// <inheritdoc/>
        public virtual async Task SubscribeAsync(SubscriptionOptionsDto subscriptionOptions, CancellationToken cancellationToken = default)
        {
            this.Logger.LogInformation("Creating a new subscription with id '{subscriptionId}' on channel '{channelName}'...", subscriptionOptions.Id, this.Name);
            string json = JsonConvert.SerializeObject(subscriptionOptions);
            using (HttpResponseMessage response = await this.HttpClient.PostAsync("sub", new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)))
            {
                string responseContent = await response.Content?.ReadAsStringAsync();
                if(!response.IsSuccessStatusCode)
                    this.Logger.LogError($"An error occured while subscribing to the channel '{{channel}}': the remote server responded with a '{{statusCode}}' status code.{Environment.NewLine}Details: {{responseContent}}", this.Name, response.StatusCode, responseContent);
            }
            this.Logger.LogInformation("A new subscription with id '{subscriptionId}' has been succesfully created on channel '{channelName}'", subscriptionOptions.Id, this.Name);
        }

        /// <inheritdoc/>
        public virtual async Task UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default)
        {
            this.Logger.LogInformation("Deleting the subscription with id '{subscriptionId}' on channel '{channelName}'...", subscriptionId, this.Name);
            using (HttpResponseMessage response = await this.HttpClient.DeleteAsync($"unsub?subscriptionId={subscriptionId}"))
            {
                string responseContent = await response.Content?.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    this.Logger.LogError($"An error occured while unsubscribing from the channel '{{channel}}': the remote server responded with a '{{statusCode}}' status code.{Environment.NewLine}Details: {{responseContent}}", this.Name, response.StatusCode, responseContent);
            }
            this.Logger.LogInformation("The subscription with id '{subscriptionId}' has been successfully deleted on channel '{channelName}'", subscriptionId, this.Name);
        }

        /// <inheritdoc/>
        public virtual async Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default)
        {
            this.Logger.LogInformation("Publishing a cloud event to the channel '{channelName}'...", this.Name);
            using (HttpResponseMessage response = await this.HttpClient.PostAsync("pub", new CloudEventContent(e, ContentMode.Structured, new JsonEventFormatter()), cancellationToken))
            {
                string responseContent = await response.Content?.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    this.Logger.LogError($"An error occured while publishing the specified cloud event to the channel '{{channel}}': the remote server responded with a '{{statusCode}}' status code.{Environment.NewLine}Details: {{responseContent}}", this.Name, response.StatusCode, responseContent);
            }
            this.Logger.LogInformation("A cloud event has been successfully published to the channel '{channelName}'", this.Name);
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="Channel"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="Channel"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    this.HttpClient?.Dispose();
                    this.HttpClient = null;
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
