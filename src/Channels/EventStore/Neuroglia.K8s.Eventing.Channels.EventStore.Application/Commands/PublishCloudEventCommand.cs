using CloudNative.CloudEvents;
using Neuroglia.Mediation;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Application.Commands
{

    /// <summary>
    /// Represents the command used to publish a <see cref="CloudEvent"/>
    /// </summary>
    public class PublishCloudEventCommand
        : Command
    {

        /// <summary>
        /// Initialize a new <see cref="PublishCloudEventCommand"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to publish</param>
        public PublishCloudEventCommand(CloudEvent e)
        {
            this.Event = e;
        }

        /// <summary>
        /// Gets the <see cref="CloudEvent"/> to publish
        /// </summary>
        public CloudEvent Event { get; }

    }

}
