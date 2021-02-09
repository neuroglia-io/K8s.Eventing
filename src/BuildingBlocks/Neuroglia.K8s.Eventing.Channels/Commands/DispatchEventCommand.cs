using CloudNative.CloudEvents;
using Neuroglia.Mediation;

namespace Neuroglia.K8s.Eventing.Channels.Commands
{

    /// <summary>
    /// Represents the <see cref="ICommand"/> used to dispatch <see cref="CloudEvent"/>s
    /// </summary>
    public class DispatchEventCommand
        : Command
    {

        /// <summary>
        /// Initializes a new <see cref="DispatchEventCommand"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to dispatch</param>
        public DispatchEventCommand(CloudEvent e)
        {
            this.Event = e;
        }

        /// <summary>
        /// Gets the <see cref="CloudEvent"/> to dispatch
        /// </summary>
        public CloudEvent Event { get; }

    }
}
