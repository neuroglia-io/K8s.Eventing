using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Defines extensions for <see cref="ControllerBase"/>s
    /// </summary>
    public static class ControllerBaseExtensions
    {

        /// <summary>
        /// Processes the specified <see cref="IOperationResult"/>
        /// </summary>
        /// <typeparam name="TResult">The type of <see cref="IOperationResult"/> to process</typeparam>
        /// <param name="controller">The extended <see cref="ControllerBase"/></param>
        /// <param name="operationResult">The <see cref="IOperationResult"/> to process</param>
        /// <param name="successStatusCode">The status code to return when processing a successfull <see cref="IOperationResult"/>. If not set, the <see cref="IOperationResult"/>'s status code will be returned</param>
        /// <returns>A new <see cref="IActionResult"/> based on the specified <see cref="IOperationResult"/></returns>
        public static IActionResult Process<TResult>(this ControllerBase controller, TResult operationResult, int? successStatusCode = null)
            where TResult : IOperationResult
        {
            IActionResult actionResult;
            if (operationResult.IsSuccessfull)
            {
                if((operationResult.StatusCode == (int)HttpStatusCode.Created && !successStatusCode.HasValue)
                    || (successStatusCode.HasValue && successStatusCode.Value == (int)HttpStatusCode.Created))
                {
                    actionResult = new CreatedResult(new Uri(controller.HttpContext.Request.GetDisplayUrl()), operationResult.GetData());
                }
                else
                {
                    if (operationResult.HasData)
                        actionResult = new ObjectResult(operationResult.GetData()) { StatusCode = successStatusCode.HasValue ? successStatusCode.Value : operationResult.StatusCode };
                    else
                        actionResult = new StatusCodeResult(successStatusCode.HasValue ? successStatusCode.Value : operationResult.StatusCode);
                }
            }
            else
            {
                operationResult.Errors.ToList().ForEach(e => controller.ModelState.AddModelError(e.Code, e.Message));
                actionResult = new ObjectResult(new SerializableError(controller.ModelState)) { StatusCode = operationResult.StatusCode };
            }
            return actionResult;
        }

    }

}
