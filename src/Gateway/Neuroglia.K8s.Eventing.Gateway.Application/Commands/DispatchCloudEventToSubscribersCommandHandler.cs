using CloudNative.CloudEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Application.Commands
{

    /// <summary>
    /// Represents the service used to handle <see cref="DispatchCloudEventToSubscribersCommand"/>s
    /// </summary>
    public class DispatchCloudEventToSubscribersCommandHandler
        : IRequestHandler<DispatchCloudEventToSubscribersCommand>
    {

        /// <summary>
        /// Initializes a new <see cref="DispatchCloudEventToSubscribersCommandHandler"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="dispatcher">The service used to dispatch <see cref="CloudEvent"/>s</param>
        /// <param name="httpClientFactory">The service used to create <see cref="System.Net.Http.HttpClient"/>s</param>
        public DispatchCloudEventToSubscribersCommandHandler(ILogger<DispatchCloudEventToSubscribersCommandHandler> logger, ICloudEventDispatcher dispatcher, IHttpClientFactory httpClientFactory)
        {
            this.Logger = logger;
            this.Dispatcher = dispatcher;
            this.HttpClient = httpClientFactory.CreateClient(typeof(DispatchCloudEventToSubscribersCommandHandler).Name);
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to dispatch <see cref="CloudEvent"/>s
        /// </summary>
        protected ICloudEventDispatcher Dispatcher { get; }

        /// <summary>
        /// Gets the <see cref="System.Net.Http.HttpClient"/> used to dispatch <see cref="CloudEvent"/>s to subscribers
        /// </summary>
        protected HttpClient HttpClient { get; }

        /// <inheritdoc/>
        public virtual async Task<Unit> Handle(DispatchCloudEventToSubscribersCommand request, CancellationToken cancellationToken)
        {
            await this.Dispatcher.DispatchAsync(request.Event, cancellationToken);
            return Unit.Value;
        }

    }

}
