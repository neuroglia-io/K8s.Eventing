using k8s;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Resources;

namespace Neuroglia.K8s.Eventing.Controller.Application.Services
{

    /// <summary>
    /// Represents the service used to control <see cref="V1BrokerResource"/>s
    /// </summary>
    public class BrokerController
        : Controller<V1BrokerResource>
    {

        /// <summary>
        /// Initializes a new <see cref="BrokerController"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="kubernetes">The service used to interact with the Kubernetes API</param>
        /// <param name="resources">The service used to watch <see cref="V1BrokerResource"/>s</param>
        public BrokerController(ILoggerFactory loggerFactory, IKubernetes kubernetes, ICustomResourceWatcher<V1BrokerResource> resources)
            : base(loggerFactory, kubernetes, resources)
        {

        }

        /// <inheritdoc/>
        protected override void OnEvent(IResourceEvent<V1BrokerResource> e)
        {
            base.OnEvent(e);
            switch (e.Type)
            {
                case WatchEventType.Added:

                    break;
                case WatchEventType.Deleted:

                    break;
            }
        }

    }

}
