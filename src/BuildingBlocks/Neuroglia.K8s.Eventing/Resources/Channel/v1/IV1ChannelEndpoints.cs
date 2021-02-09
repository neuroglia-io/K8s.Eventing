namespace Neuroglia.K8s.Eventing.Resources
{
    /// <summary>
    /// Defines the fundamentals of an object used to configure a channel's endpoints
    /// </summary>
    public interface IV1ChannelEndpoints
    {

        /// <summary>
        /// Gets/sets the path to the channel's pub endpoint 
        /// </summary>
        string Pub { get; set; }

        /// <summary>
        /// Gets/sets the path to the channel's sub endpoint 
        /// </summary>
        string Sub { get; set; }

    }

}
