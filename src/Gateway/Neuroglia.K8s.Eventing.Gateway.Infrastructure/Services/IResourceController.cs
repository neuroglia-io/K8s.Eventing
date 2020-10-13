using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Infrastructure.Services
{
    /// <summary>
    /// Defines the fundamentals of a service used to manage eventing-related <see cref="CustomResource"/>s
    /// </summary>
    public interface IResourceController
        : IDisposable
    {

        /// <summary>
        /// Initializes the <see cref="IResourceController"/>
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task InitializeAsync(CancellationToken cancellationToken = default);

    }

}
