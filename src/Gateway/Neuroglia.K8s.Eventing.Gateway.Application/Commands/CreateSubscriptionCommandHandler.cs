using MediatR;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure;
using Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Application.Commands
{

    /// <summary>
    /// Represents the service used to handle <see cref="CreateSubscriptionCommand"/>s
    /// </summary>
    public class CreateSubscriptionCommandHandler
        : IRequestHandler<CreateSubscriptionCommand, string>
    {

        /// <summary>
        /// Initializes a new <see cref="CreateSubscriptionCommandHandler"/>
        /// </summary>
        /// <param name="channelManager">The service used to manage <see cref="IChannel"/>s</param>
        /// <param name="subscriptionManager">The service used to manage <see cref="ISubscription"/>s</param>
        public CreateSubscriptionCommandHandler(IChannelManager channelManager, ISubscriptionManager subscriptionManager)
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
        public virtual async Task<string> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken)
        {
            if (!this.ChannelManager.TryGetChannel(command.Channel, out IChannel channel))
                throw new ArgumentException($"Failed to find a channel with the specified name '{command.Channel}'", nameof(command.Channel));
            string subscriptionId = Guid.NewGuid().ToString();
            SubscriptionOptionsDto subscriptionOptions = new SubscriptionOptionsDto()
            {
                Id = subscriptionId,
                Subject = command.Subject,
                DurableName = command.Durable ? subscriptionId : null,
                StreamPosition = command.StreamPosition
            };
            await channel.SubscribeAsync(subscriptionOptions, cancellationToken);
            this.SubscriptionManager.RegisterSubscription(subscriptionId, command.Subject, command.Type, command.Source, command.Channel, command.Subscribers);
            return subscriptionId;
        }

    }

}
