using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Resources;
using System;

namespace Neuroglia.K8s.Eventing.Channels.Services
{
    /// <summary>
    /// Defines the fundamentals of a service used to build <see cref="ISubscriptionOptions"/>
    /// </summary>
    public interface ISubscriptionSpecBuilder
    {

        /// <summary>
        /// Filters <see cref="CloudEvent"/>s based on the specified attribute
        /// </summary>
        /// <param name="attribute">The name of the attribute to filter the <see cref="CloudEvent"/> on</param>
        /// <param name="expression">The Regex used to match the attribute</param>
        /// <returns>The configured <see cref="ISubscriptionSpecBuilder"/></returns>
        ISubscriptionSpecBuilder FilterBy(string attribute, string expression);

        /// <summary>
        /// Adds a new subscriber
        /// </summary>
        /// <param name="url">The <see cref="Uri"/> of the subscriber to add</param>
        /// <param name="deliveryMode">The <see cref="EventDeliveryMode"/> to use</param>
        /// <returns>The configured <see cref="ISubscriptionSpecBuilder"/></returns>
        ISubscriptionSpecBuilder AddSubscriber(Uri url, EventDeliveryMode deliveryMode = EventDeliveryMode.Unicast);

        /// <summary>
        /// Builds a new <see cref="V1SubscriptionSpec"/>
        /// </summary>
        /// <returns>A new <see cref="V1SubscriptionSpec"/></returns>
        V1SubscriptionSpec Build();

    }

}
