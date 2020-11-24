using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.Services;
using Neuroglia.Mediation;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Application.Commands
{

    /// <summary>
    /// Represents the service used to handle <see cref="PublishCloudEventCommand"/>s
    /// </summary>
    public class PublishCloudEventCommandHandler
        : ICommandHandler<PublishCloudEventCommand>
    {

        /// <summary>
        /// Initializes a new <see cref="PublishCloudEventCommandHandler"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="channel">The service that wraps the the underlying NATS Streaming connection</param>
        public PublishCloudEventCommandHandler(ILogger<PublishCloudEventCommandHandler> logger, IEventChannel channel)
        {
            this.Logger = logger;
            this.Channel = channel;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service that wraps the the underlying NATS Streaming connection
        /// </summary>
        protected IEventChannel Channel { get; }

        /// <inheritdoc/>
        public virtual async Task<IOperationResult> Handle(PublishCloudEventCommand request, CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Publishing cloud event to the underlying NATS Streaming sink...");
            await this.Channel.PublishAsync(request.Event, cancellationToken);
            this.Logger.LogInformation("Cloud event published to the underlying NATS Streaming sink.");
            return this.Ok();
        }

    }

}
