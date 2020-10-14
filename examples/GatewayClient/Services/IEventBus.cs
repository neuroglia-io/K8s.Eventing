using CloudNative.CloudEvents;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GatewayClient.Services
{

    public interface IEventBus
    {

        Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default);

        Task<SubscriptionDto> SubscribeToAsync(string subject, CancellationToken cancellationToken = default);

        Task UnsubscribeFromAsync(string subscriptionId, CancellationToken cancellationToken = default);

    }

}
