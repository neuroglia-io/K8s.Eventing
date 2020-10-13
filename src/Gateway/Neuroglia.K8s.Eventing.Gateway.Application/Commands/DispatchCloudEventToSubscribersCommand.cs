using CloudNative.CloudEvents;
using MediatR;

namespace Neuroglia.K8s.Eventing.Gateway.Application.Commands
{

    /// <summary>
    /// Represents the command used to publish a <see cref="CloudEvent"/> to its subscribers, if any
    /// </summary>
    public class DispatchCloudEventToSubscribersCommand
        : IRequest
    {

        /// <summary>
        /// Initializes a new <see cref="DispatchCloudEventToSubscribersCommand"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to publish</param>
        public DispatchCloudEventToSubscribersCommand(CloudEvent e)
        {
            this.Event = e;
        }

        /// <summary>
        /// Gets the <see cref="CloudEvent"/> to publish
        /// </summary>
        public CloudEvent Event { get; }

    }

}
