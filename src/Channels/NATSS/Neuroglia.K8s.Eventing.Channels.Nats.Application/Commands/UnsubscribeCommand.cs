using MediatR;

namespace Neuroglia.K8s.Eventing.Channels.Nats.Application.Commands
{

    /// <summary>
    /// Represents the command used to delete an existing subscription
    /// </summary>
    public class UnsubscribeCommand
        : IRequest
    {

        /// <summary>
        /// Initializes a new <see cref="UnsubscribeCommand"/>
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to delete</param>
        public UnsubscribeCommand(string subscriptionId)
        {
            this.SubscriptionId = subscriptionId;
        }

        /// <summary>
        /// Gets the id of the subscription to delete
        /// </summary>
        public string SubscriptionId { get; }

    }

}
