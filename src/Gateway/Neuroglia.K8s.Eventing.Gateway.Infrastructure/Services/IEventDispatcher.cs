using CloudNative.CloudEvents;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to dispatch <see cref="CloudEvent"/>s to subscribers
    /// </summary>
    public interface IEventDispatcher
    {

        /// <summary>
        /// Dispatches the specified <see cref="CloudEvent"/> to its subscribers
        /// </summary>
        /// <param name="cloudEvent">The <see cref="CloudEvent"/> to dispatch</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task DispatchAsync(CloudEvent cloudEvent, CancellationToken cancellationToken = default);

    }

}
