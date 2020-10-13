using Newtonsoft.Json;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the status of a <see cref="Broker"/>
    /// </summary>
    public class BrokerStatus
    {

        /// <summary>
        /// Gets the url of the broker's virtual service
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

    }

}
