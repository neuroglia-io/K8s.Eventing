using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services;
using Neuroglia.K8s.Eventing.Resources;
using Neuroglia.Mediation;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Application.Commands
{

    /// <summary>
    /// Represents the service used to handle <see cref="PublishCloudEventToChannelCommand"/>s
    /// </summary>
    public class PublishCloudEventToChannelCommandHandler
        : ICommandHandler<PublishCloudEventToChannelCommand>
    {

        /// <summary>
        /// Initializes a new <see cref="PublishCloudEventToChannelCommandHandler"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="channelManager">The service used to manage <see cref="IChannel"/>s</param>
        /// <param name="eventRegistry">The service used to manage <see cref="EventType"/>s</param>
        public PublishCloudEventToChannelCommandHandler(ILogger<PublishCloudEventToChannelCommandHandler> logger, IChannelManager channelManager, IEventRegistry eventRegistry)
        {
            this.Logger = logger;
            this.ChannelManager = channelManager;
            this.EventRegistry = eventRegistry;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to manage <see cref="IChannel"/>s
        /// </summary>
        protected IChannelManager ChannelManager { get; }

        /// <summary>
        /// Gets the service used to manage <see cref="EventType"/>s
        /// </summary>
        protected IEventRegistry EventRegistry { get; }

        /// <inheritdoc/>
        public virtual async Task<IOperationResult> Handle(PublishCloudEventToChannelCommand command, CancellationToken cancellationToken)
        {
            await this.EventRegistry.AddAsync(command.Event, cancellationToken);
            if (this.ChannelManager.TryGetChannel(command.Channel, out IChannel channel))
                await channel.PublishAsync(command.Event, cancellationToken);
            return this.Ok();
        }

    }

}
