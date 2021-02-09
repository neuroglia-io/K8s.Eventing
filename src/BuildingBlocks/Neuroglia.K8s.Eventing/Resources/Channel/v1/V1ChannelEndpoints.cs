using Newtonsoft.Json;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the object used to configure a channel's endpoints
    /// </summary>
    public class V1ChannelEndpoints
        : IV1ChannelEndpoints
    {

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "pub")]
        public string Pub { get; set; }

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "sub")]
        public string Sub { get; set; }

    }

}
