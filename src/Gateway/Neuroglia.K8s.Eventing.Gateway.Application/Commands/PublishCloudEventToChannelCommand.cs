using CloudNative.CloudEvents;
using MediatR;

namespace Neuroglia.K8s.Eventing.Gateway.Application.Commands
{

    /// <summary>
    /// Represents the command used to publish a <see cref="CloudEvent"/> to the channels it is bound to
    /// </summary>
    public class PublishCloudEventToChannelCommand
        : IRequest
    {

        /// <summary>
        /// Initializes a new <see cref="PublishCloudEventToChannelCommand"/>
        /// </summary>
        /// <param name="channel">The channel to publish the <see cref="CloudEvent"/> to</param>
        /// <param name="e">The <see cref="CloudEvent"/> to publish</param>
        public PublishCloudEventToChannelCommand(string channel, CloudEvent e)
        {
            this.Channel = channel;
            this.Event = e;
        }

        /// <summary>
        /// Gets the channel to publish the <see cref="CloudEvent"/> to
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Gets the <see cref="CloudEvent"/> to publish
        /// </summary>
        public CloudEvent Event { get; }

    }

}
