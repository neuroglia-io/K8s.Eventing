using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using Neuroglia.Mediation;
using System;
using System.Collections.Generic;

namespace Neuroglia.K8s.Eventing.Gateway.Application.Commands
{

    /// <summary>
    /// Represents the command used to create a new subscription
    /// </summary>
    public class CreateSubscriptionCommand
        : Command<SubscriptionDto>
    {

        /// <summary>
        /// Initializes a new <see cref="CreateSubscriptionCommand"/>
        /// </summary>
        /// <param name="channel">The name of the channel the subscription to create is bound to</param>
        /// <param name="subject">The cloud event subject the subscription to create applies to</param>
        /// <param name="type">The cloud event type the subscription to create applies to</param>
        /// <param name="source">The cloud event source the subscription to create applies to</param>
        /// <param name="durable">A boolean indicating whether or not the subscription to create is durable</param>
        /// <param name="streamPosition">An integer representing the position in the cloud event stream starting from which to consume cloud events</param>
        /// <param name="subscribers">An <see cref="IList{T}"/> containing the URIs of all the services that subscribe to the subscription to create</param>
        public CreateSubscriptionCommand(string channel, string subject, string type, Uri source, bool durable, int? streamPosition, IList<Uri> subscribers)
        {
            this.Channel = channel;
            this.Subject = subject;
            this.Type = type;
            this.Source = source;
            this.Durable = durable;
            this.StreamPosition = streamPosition;
            this.Subscribers = subscribers;
        }

        /// <summary>
        /// Gets/sets the name of the channel the subscription to create is bound to
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Gets/sets the cloud event subject the subscription to create applies to
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// Gets/sets the cloud event type the subscription to create applies to
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets/sets the cloud event source the subscription to create applies to
        /// </summary>
        public Uri Source { get; }

        /// <summary>
        /// Gets/sets a boolean indicating whether or not the subscription to create is durable
        /// </summary>
        public bool Durable { get; }

        /// <summary>
        /// Gets/sets an integer representing the position in the cloud event stream starting from which to consume cloud events
        /// </summary>
        public long? StreamPosition { get; }

        /// <summary>
        /// Gets/sets a <see cref="IList{T}"/> containing the URIs of all the services that subscribe to the subscription to create
        /// </summary>
        public IList<Uri> Subscribers { get; }

    }

}
