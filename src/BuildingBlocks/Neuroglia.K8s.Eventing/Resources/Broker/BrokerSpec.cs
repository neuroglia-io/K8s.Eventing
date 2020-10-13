using Newtonsoft.Json;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the spec of a <see cref="Broker"/>
    /// </summary>
    public class BrokerSpec
    {

        /// <summary>
        /// Gets/sets the channel used by the broker
        /// </summary>
        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

    }

}
