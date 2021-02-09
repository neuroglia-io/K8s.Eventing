using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the spec of a <see cref="V1SubscriptionResource"/>
    /// </summary>
    public class V1SubscriptionSpec
    {

        /// <summary>
        /// Gets/sets the name of the channel the subscription is bound to
        /// </summary>
        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Gets/sets information about the subscription's cloud event stream
        /// </summary>
        [JsonProperty(PropertyName = "stream")]
        public V1StreamSpec Stream { get; set; }

        /// <summary>
        /// Gets/sets the object used to filter the events to deliver to the subscribers
        /// </summary>
        [JsonProperty(PropertyName = "filter")]
        public EventFilter Filter { get; set; }

        /// <summary>
        /// Gets/sets an <see cref="IList{T}"/> containing the subscription's subscribers
        /// </summary>
        [JsonProperty(PropertyName = "subscribers")]
        public IList<V1SubscriberSpec> Subscribers { get; set; }

        /// <summary>
        /// Validates the <see cref="V1SubscriberSpec"/>
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Channel))
                throw new ArgumentNullException(nameof(Channel));
        }

    }

}
