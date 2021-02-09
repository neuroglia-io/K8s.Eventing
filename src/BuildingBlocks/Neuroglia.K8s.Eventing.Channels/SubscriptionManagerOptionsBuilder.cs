namespace Neuroglia.K8s.Eventing.Channels
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ISubscriptionManagerOptionsBuilder"/> service
    /// </summary>
    public class SubscriptionManagerOptionsBuilder
        : ISubscriptionManagerOptionsBuilder
    {

        /// <summary>
        /// Initializes a new <see cref="SubscriptionManagerOptionsBuilder"/>
        /// </summary>
        /// <param name="options">The <see cref="SubscriptionManagerOptions"/> to configure</param>
        public SubscriptionManagerOptionsBuilder(SubscriptionManagerOptions options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Initializes a new <see cref="SubscriptionManagerOptionsBuilder"/>
        /// </summary>
        public SubscriptionManagerOptionsBuilder()
            : this(new SubscriptionManagerOptions())
        {
            
        }

        /// <summary>
        /// Gets the <see cref="SubscriptionManagerOptions"/> to configure
        /// </summary>
        protected SubscriptionManagerOptions Options { get; }

        /// <inheritdoc/>
        public virtual ISubscriptionManagerOptionsBuilder ForChannel(string channel)
        {
            this.Options.Channel = channel;
            return this;
        }

        /// <inheritdoc/>
        public virtual SubscriptionManagerOptions Build()
        {
            return this.Options;
        }

    }
}
