using Newtonsoft.Json;
using System;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the object that holds channel status information
    /// </summary>
    public class V1ChannelStatus
        : IV1ChannelStatus
    {

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "operational")]
        public bool? Operational { get; set; }

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "address")]
        public Uri Address { get; set; }

        /// <summary>
        /// gets/sets the channel's endpoints
        /// </summary>
        [JsonProperty(PropertyName = "endpoints")]
        public V1ChannelEndpoints Endpoints { get; set; }

        IV1ChannelEndpoints IV1ChannelStatus.Endpoints
        {
            get
            {
                return this.Endpoints;
            }
        }

    }

}
