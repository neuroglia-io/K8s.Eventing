using Neuroglia.K8s.Eventing.Resources;
using System;

namespace Neuroglia.K8s.Eventing.Channels.Services
{

    /// <summary>
    /// Defines the fundamentals of a subscription
    /// </summary>
    public interface ISubscription
        : IDisposable
    {

        /// <summary>
        /// Gets the <see cref="ISubscription"/>'s name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the <see cref="ISubscription"/>'s <see cref="V1SubscriptionSpec"/>
        /// </summary>
        V1SubscriptionSpec Spec { get; }

    }

}
