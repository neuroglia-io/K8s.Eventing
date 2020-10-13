using AutoMapper;
using CloudNative.CloudEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Neuroglia.K8s.Eventing.Gateway.Application.Commands;
using Neuroglia.K8s.Eventing.Gateway.Integration.Commands;
using System;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Gateway.Api.Controllers
{

    /// <summary>
    /// Represents the <see cref="Controller"/> used to manage <see cref="CloudEvent"/>s
    /// </summary>
    [Route("[controller]")]
    public class EventsController
        : Controller
    {

        /// <summary>
        /// Initializes a new <see cref="EventsController"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="mediator">The service used to mediate calls</param>
        /// <param name="mapper">The service used to map objects</param>
        public EventsController(ILogger<EventsController> logger, IMediator mediator, IMapper mapper)
        {
            this.Logger = logger;
            this.Mediator = mediator;
            this.Mapper = mapper;
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
        /// Gets the service used to map objects
        /// </summary>
        protected IMapper Mapper { get; }

        /// <summary>
        /// Publishes the specified cloud event
        /// </summary>
        /// <param name="cloudEvent">The cloud event to publish</param>
        /// <returns>A new <see cref="IActionResult"/></returns>
        [HttpPost("pub")]
        public async Task<IActionResult> Pub([FromBody]CloudEvent cloudEvent)
        {
            this.Logger.LogInformation($"Cloud event received. Payload:{Environment.NewLine}{{payload}}", Encoding.UTF8.GetString(new JsonEventFormatter().EncodeStructuredEvent(cloudEvent, out ContentType contentType)));
            if (this.Request.Headers.TryGetValue(EventingDefaults.Headers.Channel, out StringValues values))
                await this.Mediator.Send(new PublishCloudEventToChannelCommand(values.First(), cloudEvent));
            else if (this.Request.Headers.ContainsKey(EventingDefaults.Headers.Origin))
                await this.Mediator.Send(new DispatchCloudEventToSubscribersCommand(cloudEvent));
            return this.Accepted();
        }

        /// <summary>
        /// Creates a new cloud event subscription
        /// </summary>
        /// <param name="command">The object that describes the command to execute</param>
        /// <returns>A new <see cref="IActionResult"/></returns>
        [HttpPost("sub")]
        public async Task<IActionResult> Sub([FromBody]CreateSubscriptionCommandDto command)
        {
            string subscriptionId = await this.Mediator.Send(this.Mapper.Map<CreateSubscriptionCommand>(command));
            return this.Ok(subscriptionId);
        }

        /// <summary>
        /// Deletes an existing cloud event subscription
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to delete</param>
        /// <returns>A new <see cref="IActionResult"/></returns>
        [HttpDelete("unsub")]
        public async Task<IActionResult> Unsub(string subscriptionId)
        {
            await this.Mediator.Send(new DeleteSubscriptionCommand(subscriptionId));
            return this.Ok();
        }

    }

}
