using CloudNative.CloudEvents;
using GatewayClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GatewayClient.Controllers
{

    [Route("[controller]")]
    public class EventsController
        : ControllerBase
    {

        public EventsController(ILogger<EventsController> logger, IEventBus eventBus)
        {
            this.Logger = logger;
            this.EventBus = eventBus;
        }

        protected ILogger Logger { get; }

        protected IEventBus EventBus { get; }

        [HttpPost("pub")]
        public async Task<IActionResult> Pub([FromBody]CloudEvent e)
        {
            await this.EventBus.PublishAsync(e);
            return this.Accepted();
        }

        [HttpPost("sub")]
        public async Task<IActionResult> Sub(string subject)
        {
            var subscriptionId = await this.EventBus.SubscribeToAsync(subject);
            return this.Ok(subscriptionId);
        }

        [HttpDelete("unsub")]
        public async Task<IActionResult> Unsub(string subscriptionId)
        {
            await this.EventBus.UnsubscribeFromAsync(subscriptionId);
            return this.Ok();
        }

        [HttpPost]
        public IActionResult Consume([FromBody]CloudEvent e)
        {
            this.Logger.LogInformation("Received a cloud event with type '{type}' and subject '{subject}'", e.Type, e.Subject);
            return this.Accepted();
        }

    }

}
