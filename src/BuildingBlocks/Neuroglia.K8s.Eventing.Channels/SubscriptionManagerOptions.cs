using Neuroglia.K8s.Eventing.Channels.Services;

namespace Neuroglia.K8s.Eventing.Channels
{

    /// <summary>
    /// Represents the options used to configure an <see cref="ISubscriptionManager"/>
    /// </summary>
    public class SubscriptionManagerOptions
    {

        /// <summary>
        /// Gets/sets the name of the channel for which to manage subscriptions
        /// </summary>
        public string Channel { get; set; }

    }

}
