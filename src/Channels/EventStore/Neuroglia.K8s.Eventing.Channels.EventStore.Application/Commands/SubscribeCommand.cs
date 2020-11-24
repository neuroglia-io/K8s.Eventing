using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using Neuroglia.Mediation;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Application.Commands
{

    /// <summary>
    /// Represents the command used to create a new subscription
    /// </summary>
    public class SubscribeCommand
        : Command
    {

        /// <summary>
        /// Initializes a new <see cref="SubscribeCommand"/>
        /// </summary>
        /// <param name="subscriptionOptions">The options used to configure the subscription to create</param>
        public SubscribeCommand(SubscriptionOptionsDto subscriptionOptions)
        {
            this.SubscriptionOptions = subscriptionOptions;
        }

        /// <summary>
        /// Gets the options used to configure the subscription to create
        /// </summary>
        public SubscriptionOptionsDto SubscriptionOptions { get; }

    }

}
