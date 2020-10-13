using MediatR;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Channels.Nats.Infrastructure.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Nats.Application.Commands
{

    /// <summary>
    /// Represents the service used to handle <see cref="UnsubscribeCommand"/>s
    /// </summary>
    public class UnsubscribeCommandHandler
        : IRequestHandler<UnsubscribeCommand>
    {

        /// <summary>
        /// Initializes a new <see cref="UnsubscribeCommandHandler"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="channel">The service that wraps the the underlying NATS Streaming connection</param>
        public UnsubscribeCommandHandler(ILogger<UnsubscribeCommandHandler> logger, IEventChannel channel)
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
        public virtual async Task<Unit> Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Deleting an existing subscription from the underlying NATS Streaming sink");
            await this.Channel.UnsubscribeAsync(request.SubscriptionId, cancellationToken);
            this.Logger.LogInformation("The subscription has been successfully deleted from the underlying NATS Streaming sink");
            return Unit.Value;
        }

    }

}
