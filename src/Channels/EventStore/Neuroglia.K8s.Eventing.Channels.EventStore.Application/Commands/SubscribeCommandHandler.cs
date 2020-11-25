using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.Services;
using Neuroglia.Mediation;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Application.Commands
{

    /// <summary>
    /// Represents the service used to handle <see cref="SubscribeCommand"/>s
    /// </summary>
    public class SubscribeCommandHandler
        : ICommandHandler<SubscribeCommand>
    {

        /// <summary>
        /// Initializes a new <see cref="SubscribeCommandHandler"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="channel">The service that wraps the the underlying EventStore connection</param>
        public SubscribeCommandHandler(ILogger<SubscribeCommandHandler> logger, IEventChannel channel)
        {
            this.Logger = logger;
            this.Channel = channel;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service that wraps the the underlying EventStore connection
        /// </summary>
        protected IEventChannel Channel { get; }

        /// <inheritdoc/>
        public virtual async Task<IOperationResult> Handle(SubscribeCommand request, CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Creating a new subscription on the underlying EventStore sink...");
            await this.Channel.SubscribeAsync(request.SubscriptionOptions, cancellationToken);
            this.Logger.LogInformation("The subscription has been successfully created on the underlying EventStore sink.");
            return this.Ok();
        }

    }

}
