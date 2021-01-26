using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to manage <see cref="EventType"/>
    /// </summary>
    public interface IEventRegistry
    {

        /// <summary>
        /// Initializes the <see cref="IEventRegistry"/>
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task InitializeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds the specified <see cref="CloudEvent"/> to the registry
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to add</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task AddAsync(CloudEvent e, CancellationToken cancellationToken = default);

    }

}
