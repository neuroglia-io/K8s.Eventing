namespace Neuroglia.K8s.Eventing.Gateway.Integration.Models
{

    /// <summary>
    /// Represents the <see cref="IDataTransferObject"/> used to configure a new cloud event subscription
    /// </summary>
    public class SubscriptionOptionsDto
        : IDataTransferObject
    {

        /// <summary>
        /// Gets/sets the id of the subscription to configure
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the subject of the subscription to configure
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets/sets the durable name of the subscription to configure
        /// </summary>
        public string DurableName { get; set; }

        /// <summary>
        /// Gets/sets the stream position of the subscription to configure
        /// </summary>
        public long? StreamPosition { get; set; }

    }

}
