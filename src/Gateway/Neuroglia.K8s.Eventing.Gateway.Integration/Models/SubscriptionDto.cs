using System;
using System.Collections.Generic;

namespace Neuroglia.K8s.Eventing.Gateway.Integration.Models
{

    /// <summary>
    /// Describes a subscription
    /// </summary>
    public class SubscriptionDto
        : IDataTransferObject
    {

        /// <summary>
        /// Gets/sets the subscription's id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the cloud event subject the subscription applies to, if any
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets/sets the cloud event type the subscription applies to, if any
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets/sets the cloud event source the subscription applies to, if any
        /// </summary>
        public Uri Source { get; set; }

        /// <summary>
        /// Gets/sets the name of the channel the subscription is bound to
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Gets/sets a list containing the URIs of the services applying to the subscription
        /// </summary>
        public IList<Uri> Subscribers { get; set; }

    }

}
