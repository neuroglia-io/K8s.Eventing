using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Channels.Services;
using Neuroglia.Mediation;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Commands
{

    /// <summary>
    /// Represents the service used to handle <see cref="DispatchEventCommand"/>s
    /// </summary>
    public class DispatchEventCommandHandler
        : ICommandHandler<DispatchEventCommand>
    {

        /// <summary>
        /// Initializes a new <see cref="DispatchEventCommandHandler"/>
        /// </summary>
        /// <param name="eventDispatcher">The service used to dispatch <see cref="CloudEvent"/>s</param>
        public DispatchEventCommandHandler(IEventDispatcher eventDispatcher)
        {
            this.EventDispatcher = eventDispatcher;
        }

        /// <summary>
        /// Gets the service used to dispatch <see cref="CloudEvent"/>s
        /// </summary>
        protected IEventDispatcher EventDispatcher { get; }

        /// <inheritdoc/>
        public virtual async Task<IOperationResult> Handle(DispatchEventCommand command, CancellationToken cancellationToken)
        {
            await this.EventDispatcher.DispatchAsync(command.Event, cancellationToken);
            return this.Ok();
        }

    }
}
