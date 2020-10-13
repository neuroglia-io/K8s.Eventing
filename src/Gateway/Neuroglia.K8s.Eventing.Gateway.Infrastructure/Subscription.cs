using CloudNative.CloudEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ISubscription"/> interface
    /// </summary>
    public class Subscription
        : ISubscription
    {

        /// <summary>
        /// Initializes a new <see cref="Subscription"/>
        /// </summary>
        /// <param name="id">The <see cref="Subscription"/>'s id</param>
        /// <param name="subject">The <see cref="CloudEvent"/> subject the <see cref="Subscription"/> applies to</param>
        /// <param name="type">The <see cref="CloudEvent"/> type the <see cref="Subscription"/> applies to</param>
        /// <param name="source">The <see cref="CloudEvent"/> source the <see cref="Subscription"/> applies to</param>
        /// <param name="channelName">The name of the channel the <see cref="Subscription"/> is bound to</param>
        /// <param name="subscribers">An <see cref="IEnumerable{T}"/> containing the <see cref="Uri"/>s of all the subscribers</param>
        public Subscription(string id, string subject, string type, Uri source, string channelName, IEnumerable<Uri> subscribers)
        {
            this.Id = id;
            this.Subject = subject;
            this.Type = type;
            this.Source = source;
            this.ChannelName = channelName;
            this.Subscribers = subscribers == null ? new List<Uri>() : subscribers.ToList();
        }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public string Subject { get; set; }

        /// <inheritdoc/>
        public string Type { get; set; }

        /// <inheritdoc/>
        public Uri Source { get; set; }

        /// <inheritdoc/>
        public string ChannelName { get; set; }

        /// <inheritdoc/>
        public bool IsChannelBound
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.ChannelName);
            }
        }

        /// <inheritdoc/>
        public IList<Uri> Subscribers { get; set; }

    }

}
