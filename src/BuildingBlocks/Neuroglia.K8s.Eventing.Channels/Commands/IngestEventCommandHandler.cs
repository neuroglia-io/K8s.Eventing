using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Channels.Services;
using Neuroglia.Mediation;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Commands
{

    /// <summary>
    /// Represents the service used to handle <see cref="IngestEventCommand"/>s
    /// </summary>
    public class IngestEventCommandHandler
        : ICommandHandler<IngestEventCommand>
    {

        /// <summary>
        /// Initializes a new <see cref="IngestEventCommandHandler"/>
        /// </summary>
        /// <param name="eventIngestor">The service used to ingest <see cref="CloudEvent"/>s</param>
        public IngestEventCommandHandler(IEventIngestor eventIngestor)
        {
            this.EventIngestor = eventIngestor;
        }

        /// <summary>
        /// Gets the service used to ingest <see cref="CloudEvent"/>s
        /// </summary>
        protected IEventIngestor EventIngestor { get; }

        /// <inheritdoc/>
        public virtual async Task<IOperationResult> Handle(IngestEventCommand command, CancellationToken cancellationToken)
        {
            await this.EventIngestor.IngestAsync(command.Event, cancellationToken);
            return this.Ok();
        }

    }

}
