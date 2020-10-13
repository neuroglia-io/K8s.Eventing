using CloudNative.CloudEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Channels.Nats.Application.Commands;
using Neuroglia.K8s.Eventing.Gateway.Integration.Models;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Nats.Api.Controllers
{

    /// <summary>
    /// Represents the <see cref="ControllerBase"/> used to manage <see cref="CloudEvent"/>s
    /// </summary>
    [Route("[controller]")]
    public class EventsController
        : ControllerBase
    {

        /// <summary>
        /// Initializes a new <see cref="EventsController"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="mediator">The service used to mediate calls</param>
        public EventsController(ILogger<EventsController> logger, IMediator mediator)
        {
            this.Logger = logger;
            this.Mediator = mediator;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to mediate calls
        /// </summary>
        protected IMediator Mediator { get; }

        /// <summary>
        /// Creates a new subscription
        /// </summary>
        /// <param name="subscription">The object used to configure the subscription to create</param>
        /// <returns>A new <see cref="IActionResult"/></returns>
        [HttpPost("sub")]
        public async Task<IActionResult> Subscribe([FromBody]SubscriptionOptionsDto subscription)
        {
            await this.Mediator.Send(new SubscribeCommand(subscription));
            return this.Accepted();
        }

        /// <summary>
        /// Deletes an existing subscription
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to delete</param>
        /// <returns>A new <see cref="IActionResult"/></returns>
        [HttpDelete("unsub")]
        public async Task<IActionResult> Unsubscribe(string subscriptionId)
        {
            await this.Mediator.Send(new UnsubscribeCommand(subscriptionId));
            return this.Accepted();
        }

        /// <summary>
        /// Publishes a <see cref="CloudEvent"/>
        /// </summary>
        /// <param name="e">The <see cref="CloudEvent"/> to publish</param>
        /// <returns>A new <see cref="IActionResult"/></returns>
        [HttpPost("pub")]
        public async Task<IActionResult> Publish([FromBody]CloudEvent e)
        {
            await this.Mediator.Send(new PublishCloudEventCommand(e));
            return this.Accepted();
        }

    }

}
