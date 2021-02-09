using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Channels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neuroglia.K8s.Eventing.Channels
{

    /// <summary>
    /// Defines extensions for <see cref="ISubscription"/>s
    /// </summary>
    public static class ISubscriptionExtensions
    {

        /// <summary>
        /// Determines whether or not the <see cref="ISubscription"/> correlates to the specified <see cref="CloudEvent"/>
        /// </summary>
        /// <param name="subscription">The <see cref="ISubscription"/> to check</param>
        /// <param name="e">The <see cref="CloudEvent"/> to check</param>
        /// <returns>A boolean indicating whether or not the <see cref="ISubscription"/> correlates to the specified <see cref="CloudEvent"/></returns>
        public static bool CorrelatesTo(this ISubscription subscription, CloudEvent e)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            if (subscription.Spec.Filter == null
                || subscription.Spec.Filter.Attributes == null
                || subscription.Spec.Filter.Attributes.Count < 1)
                return true;
            IDictionary<string, string> attributes = e.GetAttributes().ToDictionary(kvp => kvp.Key, kvp => (kvp.Value is string str) ? str : kvp.Value.ToString());
            foreach (KeyValuePair<string, string> attributeFilter in subscription.Spec.Filter.Attributes)
            {
                if (!attributes.TryGetValue(attributeFilter.Key, out string attribute)
                    || !Regex.IsMatch(attribute, $"^{attributeFilter.Value}"))
                    return false;
            }
            return true;
        }

    }

}
