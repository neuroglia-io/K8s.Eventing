namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Configuration
{

    /// <summary>
    /// Represents the options used to configure the application's pod
    /// </summary>
    public class PodOptions
    {

        /// <summary>
        /// Gets/sets the namespace the application's pod resides in
        /// </summary>
        public string Namespace { get; set; }

    }

}
