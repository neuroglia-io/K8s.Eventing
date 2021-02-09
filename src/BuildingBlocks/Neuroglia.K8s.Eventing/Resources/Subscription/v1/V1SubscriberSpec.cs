using Newtonsoft.Json;
using System;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the object used to configure a subscriber
    /// </summary>
    public class V1SubscriberSpec
    {

        /// <summary>
        /// Gets/sets the subscriber's url
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public Uri Url { get; set; }

        /// <summary>
        /// Gets/sets the way events are delivered to the subscriber
        /// </summary>
        [JsonProperty(PropertyName = "deliveryMode")]
        public EventDeliveryMode DeliveryMode { get; set; }

    }

}
