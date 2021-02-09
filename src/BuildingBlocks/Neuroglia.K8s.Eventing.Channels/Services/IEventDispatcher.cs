using CloudNative.CloudEvents;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to dispatch <see cref="CloudEvent"/>s
    /// </summary>
    public interface IEventDispatcher
    {

        /// <summary>
        /// Dispatches the specified <see cref="CloudEvent"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to dispatch</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task DispatchAsync(CloudEvent e, CancellationToken cancellationToken = default);

    }

}
