using Newtonsoft.Json;
using System;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the object used to configure a subscriber
    /// </summary>
    public class Subscriber
    {

        /// <summary>
        /// Gets/sets the subscriber's uri
        /// </summary>
        [JsonProperty(PropertyName = "uri")]
        public Uri Uri { get; set; }

    }

}
