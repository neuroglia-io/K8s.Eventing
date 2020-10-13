using MediatR;

namespace Neuroglia.K8s.Eventing.Gateway.Application.Commands
{

    /// <summary>
    /// Represents the command used to delete an existing subscription
    /// </summary>
    public class DeleteSubscriptionCommand
        : IRequest
    {

        /// <summary>
        /// Initializes a new <see cref="DeleteSubscriptionCommand"/>
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to delete</param>
        public DeleteSubscriptionCommand(string subscriptionId)
        {
            this.SubscriptionId = subscriptionId;
        }

        /// <summary>
        /// Gets the id of the subscription to delete
        /// </summary>
        public string SubscriptionId { get; }

    }

}
