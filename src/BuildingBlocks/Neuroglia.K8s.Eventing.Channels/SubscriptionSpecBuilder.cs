using Neuroglia.K8s.Eventing.Resources;
using System;
using System.Collections.Generic;

namespace Neuroglia.K8s.Eventing.Channels.Services
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ISubscriptionSpecBuilder"/> interface
    /// </summary>
    public class SubscriptionSpecBuilder
        : ISubscriptionSpecBuilder
    {

        /// <summary>
        /// Initializes a new <see cref="SubscriptionSpecBuilder"/>
        /// </summary>
        /// <param name="spec">The <see cref="V1SubscriptionSpec"/> to build</param>
        public SubscriptionSpecBuilder(V1SubscriptionSpec spec)
        {
            this.Spec = spec;
        }

        /// <summary>
        /// Initializes a new <see cref="SubscriptionSpecBuilder"/>
        /// </summary>
        /// <param name="channel">The name of the channel to create a new <see cref="V1SubscriberSpec"/> for</param>
        public SubscriptionSpecBuilder(string channel)
            : this(new V1SubscriptionSpec() { Channel = channel })
        {

        }

        /// <summary>
        /// Gets the <see cref="V1SubscriptionSpec"/> to build
        /// </summary>
        protected V1SubscriptionSpec Spec { get; }

        /// <inheritdoc/>
        public virtual ISubscriptionSpecBuilder FilterBy(string attribute, string expression)
        {
            if (this.Spec.Filter == null)
                this.Spec.Filter = new EventFilter();
            if (this.Spec.Filter.Attributes == null)
                this.Spec.Filter.Attributes = new Dictionary<string, string>();
            this.Spec.Filter.Attributes.Add(attribute, expression);
            return this;
        }

        /// <inheritdoc/>
        public virtual ISubscriptionSpecBuilder AddSubscriber(Uri uri, EventDeliveryMode deliveryMode = EventDeliveryMode.Unicast)
        {
            if (this.Spec.Subscribers == null)
                this.Spec.Subscribers = new List<V1SubscriberSpec>();
            this.Spec.Subscribers.Add(new V1SubscriberSpec() { Url = uri, DeliveryMode = deliveryMode });
            return this;
        }

        /// <inheritdoc/>
        public virtual V1SubscriptionSpec Build()
        {
            this.Spec.Validate();
            return this.Spec;
        }

    }

}
