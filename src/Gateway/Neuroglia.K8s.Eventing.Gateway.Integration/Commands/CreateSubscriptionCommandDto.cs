using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Neuroglia.K8s.Eventing.Gateway.Integration.Commands
{

    /// <summary>
    /// Describes the command used to create a new subscription
    /// </summary>
    public class CreateSubscriptionCommandDto
        : IDataTransferObject
    {

        /// <summary>
        /// Gets/sets the name of the channel the subscription to create is bound to
        /// </summary>
        [Required]
        public string Channel { get; set; }

        /// <summary>
        /// Gets/sets the cloud event subject the subscription to create applies to
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets/sets the cloud event type the subscription to create applies to
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets/sets the cloud event source the subscription to create applies to
        /// </summary>
        public Uri Source { get; set; }

        /// <summary>
        /// Gets/sets a boolean indicating whether or not the subscription to create is durable
        /// </summary>
        public bool Durable { get; set; }

        /// <summary>
        /// Gets/sets an integer representing the position in the cloud event stream starting from which to consume cloud events
        /// </summary>
        public long? StreamPosition { get; set; }

        /// <summary>
        /// Gets/sets a <see cref="IList{T}"/> containing the URIs of all the services that subscribe to the subscription to create
        /// </summary>
        public IList<Uri> Subscribers { get; set; }

    }

}
