using MediatR;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Represents an <see cref="IPipelineBehavior{TRequest, TResponse}"/> used to catch <see cref="OperationException"/>s and transform them into an <see cref="IOperationResult"/>
    /// </summary>
    /// <typeparam name="TCommand">The type of <see cref="ICommand{TResult}"/>s handled by the <see cref="IPipelineBehavior{TRequest, TResponse}"/></typeparam>
    /// <typeparam name="TResult">The type of response returned by the <see cref="ICommand{TResult}"/>s handled by the <see cref="IPipelineBehavior{TRequest, TResponse}"/></typeparam>
    public class OperationExceptionHandlingBehavior<TCommand, TResult>
        : IPipelineBehavior<TCommand, TResult>
        where TCommand : IRequest<TResult>
        where TResult : IOperationResult
    {

        /// <inheritdoc/>
        public virtual async Task<TResult> Handle(TCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<TResult> next)
        {
            TResult response;
            try
            {
                response = await next();
            }
            catch(OperationArgumentException ex)
            {
                response = this.CreateOperationResult((int)HttpStatusCode.BadRequest, new Error(ex.Argument, ex.Message));
            }
            catch(OperationNullReferenceException ex)
            {
                response = this.CreateOperationResult((int)HttpStatusCode.NotFound, new Error(string.Empty, ex.Message));
            }
            catch(OperationException ex)
            {
                response = this.CreateOperationResult((int)HttpStatusCode.BadRequest, new Error(string.Empty, ex.Message));
            }
            return response;
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with the specified status code and <see cref="Error"/>s
        /// </summary>
        /// <param name="statusCode">The status code of the <see cref="IOperationResult"/> to create</param>
        /// <param name="errors">An array containing the <see cref="Error"/>s contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        protected virtual TResult CreateOperationResult(int statusCode, params Error[] errors)
        {
            IOperationResult result;
            if (typeof(TResult).IsGenericType)
                result = (IOperationResult)Activator.CreateInstance(typeof(OperationResult<>).MakeGenericType(typeof(TResult).GetGenericArguments()[0]), new object[] { statusCode, errors });
            else
                result = new OperationResult(statusCode, errors);
            return (TResult)result;
        }

    }

}
