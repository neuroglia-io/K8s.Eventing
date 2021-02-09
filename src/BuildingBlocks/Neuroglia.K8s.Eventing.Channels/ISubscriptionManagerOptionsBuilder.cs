using Neuroglia.K8s.Eventing.Channels.Services;

namespace Neuroglia.K8s.Eventing.Channels
{
    /// <summary>
    /// Defines the fundamentals of a service used to build <see cref="SubscriptionManagerOptions"/>
    /// </summary>
    public interface ISubscriptionManagerOptionsBuilder
    {

        /// <summary>
        /// Sets the channel to create the <see cref="ISubscriptionManager"/> for
        /// </summary>
        /// <param name="channel">The name of the channel to create the <see cref="ISubscriptionManager"/> for</param>
        /// <returns>The configured <see cref="ISubscriptionManagerOptionsBuilder"/></returns>
        ISubscriptionManagerOptionsBuilder ForChannel(string channel);

        /// <summary>
        /// Builds the <see cref="SubscriptionManagerOptions"/>
        /// </summary>
        /// <returns>A new <see cref="SubscriptionManagerOptions"/></returns>
        SubscriptionManagerOptions Build();

    }
}
