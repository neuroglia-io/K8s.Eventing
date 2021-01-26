using Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services;
using Neuroglia.K8s.Eventing.Resources;
using Neuroglia.StartupTasks;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Application.StartupTasks
{

    /// <summary>
    /// Represents the <see cref="StartupTask"/> used to initialize the <see cref="IEventRegistry"/> service
    /// </summary>
    public class EventRegistryInitializationTask
        : StartupTask
    {

        /// <summary>
        /// Initializes a new <see cref="EventRegistryInitializationTask"/>
        /// </summary>
        /// <param name="startupTaskManager">The service used to manage <see cref="StartupTask"/>s</param>
        /// <param name="eventRegistry">The service used to manage <see cref="EventType"/>s</param>
        public EventRegistryInitializationTask(IStartupTaskManager startupTaskManager, IEventRegistry eventRegistry) 
            : base(startupTaskManager)
        {
            this.EventRegistry = eventRegistry;
        }

        /// <summary>
        /// Gets the service used to manage <see cref="EventType"/>s
        /// </summary>
        protected IEventRegistry EventRegistry { get; }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await this.EventRegistry.InitializeAsync(cancellationToken);
        }

    }

}
