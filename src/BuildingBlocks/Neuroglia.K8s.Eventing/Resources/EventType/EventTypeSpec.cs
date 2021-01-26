using Newtonsoft.Json;
using System;

namespace Neuroglia.K8s.Eventing.Resources
{
    /// <summary>
    /// Represents the spec of a <see cref="EventType"/>
    /// </summary>
    public class EventTypeSpec
    {

        /// <summary>
        /// Gets/sets the event type's subject
        /// </summary>
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets/sets the event's type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets/sets the event type's source
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        public Uri Source { get; set; }

        /// <summary>
        /// Gets/sets the event type's data schema
        /// </summary>
        [JsonProperty(PropertyName = "dataSchema")]
        public Uri DataSchema { get; set; }

    }

}
