using k8s.Models;
using Newtonsoft.Json;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the spec of a <see cref="Channel"/>
    /// </summary>
    public class ChannelSpec
    {

        /// <summary>
        /// Gets/sets the <see cref="Channel"/>'s specification
        /// </summary>
        [JsonProperty(PropertyName = "container")]
        public V1Container Container { get; set; }

    }

}
