using Neuroglia.K8s.Eventing.Resources;
using System;

namespace Neuroglia.K8s.Eventing.Channels.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to create <see cref="ISubscription"/>s
    /// </summary>
    public interface ISubscriptionFactory
    {

        /// <summary>
        /// Creates a new <see cref="ISubscription"/>
        /// </summary>
        /// <param name="name">The name of the <see cref="ISubscription"/> to create</param>
        /// <param name="spec">The <see cref="V1SubscriptionSpec"/> to create a new <see cref="ISubscription"/> for</param>
        /// <returns>A new <see cref="ISubscription"/></returns>
        ISubscription Create(string name, V1SubscriptionSpec spec);

    }

}
