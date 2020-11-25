using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using Neuroglia.StartupTasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Application.StartupTasks
{

    /// <summary>
    /// Represents a <see cref="StartupTask"/> used to initialize the <see cref="IEventStoreConnection"/>
    /// </summary>
    public class EventStoreInitializationTask
        : StartupTask
    {

        /// <summary>
        /// Initializes a new <see cref="EventStoreInitializationTask"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="startupTaskManager">The service used to manage <see cref="IStartupTask"/>s</param>
        /// <param name="eventStoreConnection">The connection to the remote EventStore server</param>
        public EventStoreInitializationTask(ILogger<EventStoreInitializationTask> logger, IStartupTaskManager startupTaskManager, IEventStoreConnection eventStoreConnection) 
            : base(startupTaskManager)
        {
            this.Logger = logger;
            this.EventStoreConnection = eventStoreConnection;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected Microsoft.Extensions.Logging.ILogger Logger { get; }

        /// <summary>
        /// Gets the connection to the remote EventStore server
        /// </summary>
        protected IEventStoreConnection EventStoreConnection { get; }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.Logger.LogInformation("Connecting to the remote EventStore server...");
                await this.EventStoreConnection.ConnectAsync();
                this.Logger.LogInformation("The connection with the remote EventStore server has been successfully established.");
            }
            catch(Exception ex)
            {
                this.Logger.LogError($"An error occured while connecting to the remote EventStore server:{Environment.NewLine}{{ex}}", ex.Message);
                throw;
            }
        }

    }
}
