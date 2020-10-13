using CloudNative.CloudEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Neuroglia.K8s.Eventing.Gateway.Application.Commands;
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
        /// Publishes the specified cloud event
        /// </summary>
        /// <param name="cloudEvent">The cloud event to publish</param>
        /// <returns>A new <see cref="IActionResult"/></returns>
        [HttpPost]
        public async Task<IActionResult> Pub([FromBody]CloudEvent cloudEvent)
        {
            this.Logger.LogInformation($"Cloud event received. Payload:{Environment.NewLine}{{payload}}", Encoding.UTF8.GetString(new JsonEventFormatter().EncodeStructuredEvent(cloudEvent, out ContentType contentType)));
            if (this.Request.Headers.TryGetValue(EventingDefaults.Headers.Channel, out StringValues values))
                await this.Mediator.Send(new PublishCloudEventToChannelCommand(values.First(), cloudEvent));
            else if (this.Request.Headers.ContainsKey(EventingDefaults.Headers.Origin))
                await this.Mediator.Send(new DispatchCloudEventToSubscribersCommand(cloudEvent));
            return this.Accepted();
        }

    }

}
