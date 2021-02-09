using CloudNative.CloudEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s.Eventing.Channels.Commands;
using Neuroglia.K8s.Eventing.Resources;
using Neuroglia.Mediation;
using System.Net;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Channels.Controllers
{

    /// <summary>
    /// Represents a <see cref="ControllerBase"/>
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
        /// Posts a cloud event
        /// </summary>
        /// <param name="e">The cloud event to post</param>
        /// <returns>A new <see cref="IActionResult"/></returns>
        [HttpPost("pub")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Pub([FromBody]CloudEvent e)
        {
            return this.Process(await this.Mediator.Send(new IngestEventCommand(e)), HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Subscribes to cloud events
        /// </summary>
        /// <param name="subscription">An object that describes the subscription to create</param>
        /// <returns>A new <see cref="IActionResult"/></returns>
        [HttpPost("sub")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Sub([FromBody]V1SubscriptionSpec subscription)
        {
            if (!this.ModelState.IsValid)
                return this.BadRequest(this.ModelState);
            return this.Process(await this.Mediator.Send(new SubscribeCommand(subscription)), HttpStatusCode.OK);
        }

        /// <summary>
        /// Unsuscribes from specific cloud events
        /// </summary>
        /// <returns>A new <see cref="IActionResult"/></returns>
        [HttpDelete("sub")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Unsub(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
                return this.NotFound();
            return this.Process(await this.Mediator.Send(new UnsubscribeCommand(subscriptionId)), HttpStatusCode.NoContent);
        }

        protected IActionResult Process(IOperationResult result, HttpStatusCode successStatusCode = HttpStatusCode.OK)
        {
            if (result.IsSuccessfull)
                return this.StatusCode((int)successStatusCode);
            return this.StatusCode(result.StatusCode);
        }

        protected IActionResult Process<T>(IOperationResult<T> result, HttpStatusCode successStatusCode = HttpStatusCode.OK)
        {
            if (result.IsSuccessfull)
                return this.StatusCode((int)successStatusCode, result.Data);
            return this.Process((IOperationResult)result, successStatusCode);
        }

    }

}
