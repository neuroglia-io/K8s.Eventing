using Neuroglia.K8s.Eventing.Channels.Services;
using Neuroglia.Mediation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Commands
{

    /// <summary>
    /// Represents the service used to handle <see cref="SubscribeCommand"/>s
    /// </summary>
    public class SubscribeCommandHandler
        : ICommandHandler<SubscribeCommand, string>
    {

        /// <summary>
        /// Initializes a new <see cref="SubscribeCommandHandler"/>
        /// </summary>
        /// <param name="subscriptionManager">The service used to manage subscriptions</param>
        public SubscribeCommandHandler(ISubscriptionManager subscriptionManager)
        {
            this.SubscriptionsManager = subscriptionManager;
        }

        /// <summary>
        /// Gets the service used to manage subscriptions
        /// </summary>
        protected ISubscriptionManager SubscriptionsManager{ get; }

        /// <inheritdoc/>
        public virtual async Task<IOperationResult<string>> Handle(SubscribeCommand command, CancellationToken cancellationToken)
        {
            string subscriptionId = await this.SubscriptionsManager.SubscribeAsync(Guid.NewGuid().ToString(), command.Spec, cancellationToken);
            return this.Ok(subscriptionId);
        }

    }

}
