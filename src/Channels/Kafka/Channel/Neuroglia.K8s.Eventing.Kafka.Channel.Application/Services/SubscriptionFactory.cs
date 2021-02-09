using Microsoft.Extensions.DependencyInjection;
using Neuroglia.K8s.Eventing.Channels.Services;
using Neuroglia.K8s.Eventing.Resources;
using System;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ISubscriptionFactory"/> interface
    /// </summary>
    public class SubscriptionFactory
        : ISubscriptionFactory
    {

        /// <summary>
        /// Initializes a new <see cref="SubscriptionFactory"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        public SubscriptionFactory(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <inheritdoc/>
        public virtual ISubscription Create(string name, V1SubscriptionSpec spec)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));
            return ActivatorUtilities.CreateInstance<Subscription>(this.ServiceProvider, name, spec);
        }

    }

}
