using CloudNative.CloudEvents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to ingest <see cref="CloudEvent"/>s
    /// </summary>
    public interface IEventIngestor
        : IDisposable
    {

        /// <summary>
        /// Ingests the specified <see cref="CloudEvent"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to ingest</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task IngestAsync(CloudEvent e, CancellationToken cancellationToken = default);

    }

}
