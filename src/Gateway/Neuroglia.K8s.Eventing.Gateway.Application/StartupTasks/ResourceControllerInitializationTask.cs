using Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services;
using Neuroglia.StartupTasks;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Application.StartupTasks
{

    /// <summary>
    /// Represents the <see cref="StartupTask"/> used to initialize the <see cref="IResourceController"/> service
    /// </summary>
    public class ResourceControllerInitializationTask
        : StartupTask
    {

        /// <summary>
        /// Initializes a new <see cref="ResourceControllerInitializationTask"/>
        /// </summary>
        /// <param name="startupTaskManager">The service used to manage <see cref="StartupTask"/>s</param>
        /// <param name="resourceController">The service used to manage <see cref="CustomResource"/>s</param>
        public ResourceControllerInitializationTask(IStartupTaskManager startupTaskManager, IResourceController resourceController) 
            : base(startupTaskManager)
        {
            this.ResourceController = resourceController;
        }

        /// <summary>
        /// Gets the service used to manage <see cref="CustomResource"/>s
        /// </summary>
        protected IResourceController ResourceController { get; }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await this.ResourceController.InitializeAsync(cancellationToken);
        }

    }

}
