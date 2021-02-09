using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Resources;
using Neuroglia.Mediation;

namespace Neuroglia.K8s.Eventing.Channels.Commands
{

    /// <summary>
    /// Represents the <see cref="ICommand"/> used to subscribe to <see cref="CloudEvent"/>s
    /// </summary>
    public class SubscribeCommand
        : Command<string>
    {

        /// <summary>
        /// Initializes a new <see cref="SubscribeCommand"/>
        /// </summary>
        /// <param name="spec">The <see cref="V1SubscriptionSpec"/> of the subscription to create</param>
        public SubscribeCommand(V1SubscriptionSpec spec)
        {
            this.Spec = spec;
        }

        /// <summary>
        /// Gets the <see cref="V1SubscriptionSpec"/> of the subscription to create
        /// </summary>
        public V1SubscriptionSpec Spec { get; }

    }

}
