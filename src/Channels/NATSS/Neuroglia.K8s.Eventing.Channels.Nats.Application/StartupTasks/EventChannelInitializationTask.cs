using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Channels.Nats.Infrastructure.Services;
using Neuroglia.StartupTasks;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Nats.Application.StartupTasks
{

    /// <summary>
    /// Represents the <see cref="StartupTask"/> used to initialize the <see cref="IEventChannel"/> service
    /// </summary>
    public class EventChannelInitializationTask
        : StartupTask
    {

        /// <summary>
        /// Initializes a new <see cref="EventChannelInitializationTask"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="startupTaskManager">The service used to manage <see cref="IStartupTask"/>s</param>
        /// <param name="channel">The service that wraps the the underlying NATS Streaming connection</param>
        public EventChannelInitializationTask(ILogger<EventChannelInitializationTask> logger, IStartupTaskManager startupTaskManager, IEventChannel channel)
            : base(startupTaskManager)
        {
            this.Logger = logger;
            this.Channel = channel;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service that wraps the the underlying NATS Streaming connection
        /// </summary>
        protected IEventChannel Channel { get; }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await this.Channel.InitializeAsync(cancellationToken);
        }

    }

}
