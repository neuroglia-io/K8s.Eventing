using CloudNative.CloudEvents;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Represents the object used to configure cloud event filtering
    /// </summary>
    public class EventFilter
    {

        /// <summary>
        /// Gets/sets the key/value pairs of the attributes to filter. The value can be a regular expression.
        /// </summary>
        [JsonProperty(PropertyName = "attributes")]
        public IDictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Determines whether or not the specified <see cref="CloudEvent"/> matches the <see cref="EventFilter"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to filter</param>
        /// <returns>A boolean indicating whether or not the specified <see cref="CloudEvent"/> matches the <see cref="EventFilter"/></returns>
        public virtual bool Matches(CloudEvent e)
        {
            bool matches = true;
            if (this.Attributes == null)
                return matches;
            IDictionary<string, object> eventAttributes = e.GetAttributes();
            foreach (KeyValuePair<string, string> attribute in this.Attributes)
            {
                if (!eventAttributes.TryGetValue(attribute.Key, out object value))
                    return false;
                if (!(value is string rawValue))
                    rawValue = value.ToString();
                if (!Regex.IsMatch(rawValue, attribute.Value))
                    return false;
            }
            return false;
        }

    }

}
