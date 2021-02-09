using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Neuroglia.K8s.Eventing
{

    /// <summary>
    /// Enumerates all modes of cloud event delivery
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventDeliveryMode
    {
        /// <summary>
        /// Indicates that the cloud event should be published in a unicast fashion. Default.
        /// </summary>
        [EnumMember(Value = "Unicast")]
        Unicast,
        /// <summary>
        /// Indicates that the cloud event must be publishes to all IPs registered behind the subscriber's DNS.
        /// </summary>
        [EnumMember(Value = "Multicast")]
        Multicast
    }

}
