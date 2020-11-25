using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.Configuration;
using Neuroglia.StartupTasks;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.EventStore.Application.StartupTasks
{

    /// <summary>
    /// Represents the <see cref="StartupTask"/> used to initialize the EventStore projections
    /// </summary>
    public class ProjectionsInitializationTask
        : StartupTask
    {

        /// <summary>
        /// Initializes a new <see cref="ProjectionsInitializationTask"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="applicationOptions">The service used to access the current <see cref="Infrastructure.Configuration.ApplicationOptions"/></param>
        /// <param name="startupTaskManager">The service used to manage <see cref="IStartupTask"/>s</param>
        /// <param name="projectionsManager">The service used to manage EventStore projections</param>
        public ProjectionsInitializationTask(ILogger<ProjectionsInitializationTask> logger, IOptions<ApplicationOptions> applicationOptions, IStartupTaskManager startupTaskManager, ProjectionsManager projectionsManager) 
            : base(startupTaskManager)
        {
            this.Logger = logger;
            this.ApplicationOptions = applicationOptions.Value;
            this.ProjectionsManager = projectionsManager;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the current <see cref="Infrastructure.Configuration.ApplicationOptions"/>
        /// </summary>
        protected ApplicationOptions ApplicationOptions { get; }

        /// <summary>
        /// Gets the service used to manage EventStore projections
        /// </summary>
        protected ProjectionsManager ProjectionsManager { get; }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            string query;
            UserCredentials credentials = new UserCredentials(this.ApplicationOptions.EventStore.Username, this.ApplicationOptions.EventStore.Password);
            try
            {
                using(Stream stream = typeof(EventChannelInitializationTask).Assembly.GetManifestResourceStream(ApplicationConstants.Resources.Projections.ByCloudEventSubject))
                {
                    using(StreamReader streamReader = new StreamReader(stream))
                    {
                        query = await streamReader.ReadToEndAsync();
                    }
                }
                await this.ProjectionsManager.CreateContinuousAsync(ApplicationConstants.Projections.ByCloudEventSubject, query, credentials);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Failed to create the continuous projection '{ApplicationConstants.Projections.ByCloudEventSubject}':{Environment.NewLine}{{ex}}", ex.Message);
            }
            try
            {
                using (Stream stream = typeof(EventChannelInitializationTask).Assembly.GetManifestResourceStream(ApplicationConstants.Resources.Projections.ByCloudEventSource))
                {
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        query = await streamReader.ReadToEndAsync();
                    }
                }
                await this.ProjectionsManager.CreateContinuousAsync(ApplicationConstants.Projections.ByCloudEventSource, query, credentials);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Failed to create the continuous projection '{ApplicationConstants.Projections.ByCloudEventSource}':{Environment.NewLine}{{ex}}", ex.Message);
            }
        }

    }

}
