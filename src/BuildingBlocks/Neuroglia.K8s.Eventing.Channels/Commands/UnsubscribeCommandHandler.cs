using Neuroglia.K8s.Eventing.Channels.Services;
using Neuroglia.Mediation;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Commands
{
    /// <summary>
    /// Represents the service used to handle <see cref="UnsubscribeCommand"/>s
    /// </summary>
    public class UnsubscribeCommandHandler
        : ICommandHandler<UnsubscribeCommand>
    {

        /// <summary>
        /// Initializes a new <see cref="UnsubscribeCommandHandler"/>
        /// </summary>
        /// <param name="subscriptionManager">The service used to manage subscriptions</param>
        public UnsubscribeCommandHandler(ISubscriptionManager subscriptionManager)
        {
            this.SubscriptionsManager = subscriptionManager;
        }

        /// <summary>
        /// Gets the service used to manage subscriptions
        /// </summary>
        protected ISubscriptionManager SubscriptionsManager { get; }

        /// <inheritdoc/>
        public virtual async Task<IOperationResult> Handle(UnsubscribeCommand command, CancellationToken cancellationToken)
        {
            await this.SubscriptionsManager.UnsubscribeAsync(command.Id);
            return this.Ok();
        }

    }
}
