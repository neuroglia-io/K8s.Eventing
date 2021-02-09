using CloudNative.CloudEvents;
using Neuroglia.Mediation;

namespace Neuroglia.K8s.Eventing.Channels.Commands
{

    /// <summary>
    /// Represents the <see cref="ICommand"/> used to ingest a <see cref="CloudEvent"/>
    /// </summary>
    public class IngestEventCommand
        : Command
    {

        /// <summary>
        /// Initializes a new <see cref="IngestEventCommand"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to ingest</param>
        public IngestEventCommand(CloudEvent e)
        {
            this.Event = e;
        }

        /// <summary>
        /// Gets the <see cref="CloudEvent"/> to ingest
        /// </summary>
        public CloudEvent Event { get; }

    }

}
