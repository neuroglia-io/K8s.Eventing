using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ISubscriptionManager"/> interface
    /// </summary>
    public class SubscriptionManager
        : ISubscriptionManager
    {

        private object _Lock = new object();

        /// <summary>
        /// Initializes a new <see cref="SubscriptionManager"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        public SubscriptionManager(ILogger<SubscriptionManager> logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        private List<ISubscription> _Subscriptions = new List<ISubscription>();
        /// <summary>
        /// Gets an <see cref="IReadOnlyCollection{T}"/> containing the registered <see cref="ISubscription"/>s
        /// </summary>
        protected IReadOnlyCollection<ISubscription> Subscriptions
        {
            get
            {
                return this._Subscriptions.AsReadOnly();
            }
        }

        /// <inheritdoc/>
        public virtual ISubscription RegisterSubscription(string id, string subject, string type, Uri source, string channelName, IEnumerable<Uri> subscribers)
        {
            ISubscription subscription = new Subscription(id, subject, type, source, channelName, subscribers);
            lock (this._Lock)
            {
                this._Subscriptions.Add(subscription);
            }
            return subscription;
        }

        /// <inheritdoc/>
        public virtual void UnregisterSubscription(string id)
        {
            ISubscription subscription = this.Subscriptions.FirstOrDefault(s => s.Id == id);
            if (subscription == null)
                return;
            lock (this._Lock)
            {
                this._Subscriptions.Remove(subscription);
            }
        }

        /// <inheritdoc/>
        public virtual ISubscription GetSubscriptionById(string id)
        {
            return this.Subscriptions.FirstOrDefault(s => s.Id == id);
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ISubscription> GetSubscriptionsBySubject(string subject)
        {
            return this.Subscriptions.Where(s => s.Subject == subject);
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ISubscription> GetSubscriptionsByEventType(string eventType)
        {
            return this.Subscriptions.Where(s => s.Type == eventType);
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ISubscription> GetSubscriptionsByEventSource(Uri eventSource)
        {
            return this.Subscriptions.Where(s => s.Source == eventSource);
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ISubscription> GetSubscriptionsByChannel(string channelName)
        {
            return this.Subscriptions.Where(s => s.ChannelName == channelName);
        }

    }

}
