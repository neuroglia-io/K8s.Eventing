using Newtonsoft.Json;

namespace Neuroglia.K8s.Eventing.Resources
{
    /// <summary>
    /// Represents the spec of a subscription stream
    /// </summary>
    public class V1StreamSpec
    {

        /// <summary>
        /// Gets/sets a boolean indicating whether or not the stream is durable. Defaults to false
        /// </summary>
        [JsonProperty(PropertyName = "isDurable")]
        public bool IsDurable { get; set; }

        /// <summary>
        /// Gets/sets the stream offset starting from which to start receiving events.<para></para>
        /// If not null, the offset must be strictly superior or equal to 0, with the exception of -1 which indicates the end of the stream.
        /// If the offset is null, events will be played starting by the end of the stream.
        /// </summary>
        [JsonProperty(PropertyName = "offset")]
        public long? Offset { get; set; }

    }

}
