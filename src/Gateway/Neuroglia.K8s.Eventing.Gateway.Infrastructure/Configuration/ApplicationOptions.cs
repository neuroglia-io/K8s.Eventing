namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Configuration
{

    /// <summary>
    /// Represents the options used to configure the application
    /// </summary>
    public class ApplicationOptions
    {

        /// <summary>
        /// Initializes a new <see cref="ApplicationOptions"/>
        /// </summary>
        public ApplicationOptions()
        {
            this.Pod = new PodOptions();
        }

        /// <summary>
        /// Gets/sets the options used to configure the application's pod
        /// </summary>
        public PodOptions Pod { get; set; }

    }

}
