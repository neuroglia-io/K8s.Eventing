using Neuroglia.K8s.Eventing.Gateway.Infrastructure;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services;
using Neuroglia.Mediation;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Application.Commands
{

    /// <summary>
    /// Represents the service used to handle <see cref="DeleteSubscriptionCommand"/>s
    /// </summary>
    public class DeleteSubscriptionCommandHandler
        : ICommandHandler<DeleteSubscriptionCommand>
    {

        /// <summary>
        /// Initializes a new <see cref="DeleteSubscriptionCommandHandler"/>
        /// </summary>
        /// <param name="channelManager">The service used to manage <see cref="IChannel"/>s</param>
        /// <param name="subscriptionManager">The service used to manage <see cref="ISubscription"/>s</param>
        public DeleteSubscriptionCommandHandler(IChannelManager channelManager, ISubscriptionManager subscriptionManager)
        {
            this.ChannelManager = channelManager;
            this.SubscriptionManager = subscriptionManager;
        }

        /// <summary>
        /// Gets the service used to manage <see cref="IChannel"/>s
        /// </summary>
        protected IChannelManager ChannelManager { get; }

        /// <summary>
        /// Gets the service used to manage <see cref="ISubscription"/>s
        /// </summary>
        protected ISubscriptionManager SubscriptionManager { get; }

        /// <inheritdoc/>
        public virtual async Task<IOperationResult> Handle(DeleteSubscriptionCommand command, CancellationToken cancellationToken)
        {
            ISubscription subscription = this.SubscriptionManager.GetSubscriptionById(command.SubscriptionId);
            if (subscription == null)
                throw new OperationNullReferenceException($"Failed to find a cloud event subscription with the specified id '{command.SubscriptionId}'");
            if (subscription.IsChannelBound
                && this.ChannelManager.TryGetChannel(subscription.ChannelName, out IChannel channel))
                await channel.UnsubscribeAsync(subscription.Id, cancellationToken);
            this.SubscriptionManager.UnregisterSubscription(command.SubscriptionId);
            return this.Ok();
        }

    }

}
