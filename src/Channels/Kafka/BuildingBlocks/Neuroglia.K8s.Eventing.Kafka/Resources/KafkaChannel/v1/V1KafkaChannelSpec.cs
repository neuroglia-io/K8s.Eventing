using Neuroglia.K8s.Eventing.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Neuroglia.K8s.Eventing.Kafka.Resources
{

    /// <summary>
    /// Represents the spec of a Kafka channel
    /// </summary>
    public class V1KafkaChannelSpec
        : V1ChannelSpec
    {

        /// <summary>
        /// Gets/sets an <see cref="IList{T}"/> containing the addresses of the Kafka servers to use
        /// </summary>
        [JsonProperty(PropertyName = "servers")]
        public IList<string> Servers { get; set; }

    }

}
