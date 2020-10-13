using CloudNative.CloudEvents;
using System.Threading;
using System.Threading.Tasks;

namespace GatewayClient.Services
{

    public interface IEventBus
    {

        Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default);

        Task<string> SubscribeToAsync(string subject, CancellationToken cancellationToken = default);

        Task UnsubscribeFromAsync(string subscriptionId, CancellationToken cancellationToken = default);

    }

}
