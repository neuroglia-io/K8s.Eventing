using Newtonsoft.Json;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the spec of a <see cref="V1BrokerResource"/>
    /// </summary>
    public class V1BrokerSpec
    {

        /// <summary>
        /// Gets/sets the channel used by the broker
        /// </summary>
        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

    }

}
