using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the spec of a <see cref="Subscription"/>
    /// </summary>
    public class SubscriptionSpec
    {

        /// <summary>
        /// Gets/sets the subscription's id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the event subject the subscription applies to
        /// </summary>
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets/sets the event type the subscription applies to
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets/sets the event source the subscription applies to
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        public Uri Source { get; set; }

        /// <summary>
        /// Gets/sets the name of the channel the subscription is bound to
        /// </summary>
        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Gets/sets a boolean indicating whether or not the subscription is durable
        /// </summary>
        [JsonProperty(PropertyName = "durable")]
        public bool IsDurable { get; set; }

        /// <summary>
        /// Gets/sets an <see cref="IList{T}"/> containing the subscription's subscribers
        /// </summary>
        [JsonProperty(PropertyName = "subscriber")]
        public IList<Subscriber> Subscriber { get; set; }

    }

}
