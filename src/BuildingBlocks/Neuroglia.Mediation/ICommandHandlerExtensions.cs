using System.Net;

namespace Neuroglia.Mediation
{
    /// <summary>
    /// Defines extensions for the <see cref="ICommandHandler{TCommand}"/> interface
    /// </summary>
    public static class ICommandHandlerExtensions
    {

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 200 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult Ok<TCommand>(this ICommandHandler<TCommand> handler)
            where TCommand : ICommand<IOperationResult>
        {
            return new OperationResult((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 400 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <param name="errors">An array of the <see cref="Error"/>s contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult BadRequest<TCommand>(this ICommandHandler<TCommand> handler, params Error[] errors)
            where TCommand : ICommand<IOperationResult>
        {
            return new OperationResult((int)HttpStatusCode.BadRequest, errors);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 400 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <param name="errorCode">The code of the <see cref="Error"/> contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult BadRequest<TCommand>(this ICommandHandler<TCommand> handler, string errorCode, string errorMessage)
            where TCommand : ICommand<IOperationResult>
        {
            return handler.BadRequest(new Error(errorCode, errorMessage));
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 400 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult BadRequest<TCommand>(this ICommandHandler<TCommand> handler)
          where TCommand : ICommand<IOperationResult>
        {
            return handler.BadRequest(null);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 404 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <param name="errors">An array of the <see cref="Error"/>s contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult NotFound<TCommand>(this ICommandHandler<TCommand> handler, params Error[] errors)
            where TCommand : ICommand<IOperationResult>
        {
            return new OperationResult((int)HttpStatusCode.NotFound, errors);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 404 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <param name="errorCode">The code of the <see cref="Error"/> contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult NotFound<TCommand>(this ICommandHandler<TCommand> handler, string errorCode, string errorMessage)
            where TCommand : ICommand<IOperationResult>
        {
            return handler.NotFound(new Error(errorCode, errorMessage));
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 404 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult NotFound<TCommand>(this ICommandHandler<TCommand> handler)
            where TCommand : ICommand<IOperationResult>
        {
            return handler.NotFound(null);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 304 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <param name="errors">An array of the <see cref="Error"/>s contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult NotModified<TCommand>(this ICommandHandler<TCommand> handler, params Error[] errors)
            where TCommand : ICommand<IOperationResult>
        {
            return new OperationResult((int)HttpStatusCode.NotModified, errors);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 304 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <param name="errorCode">The code of the <see cref="Error"/> contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult NotModified<TCommand>(this ICommandHandler<TCommand> handler, string errorCode, string errorMessage)
            where TCommand : ICommand<IOperationResult>
        {
            return handler.NotModified(new Error(errorCode, errorMessage));
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 304 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult NotModified<TCommand>(this ICommandHandler<TCommand> handler)
            where TCommand : ICommand<IOperationResult>
        {
            return handler.NotModified(null);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 200 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <typeparam name="T">The type of data returned by the operation</typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, T}"/></param>
        /// <param name="data">The data returned by the operation</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult<T> Ok<TCommand, T>(this ICommandHandler<TCommand, T> handler, T data)
            where TCommand : ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>(data);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 400 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <typeparam name="T">The type of data returned by the operation</typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, T}"/></param>
        /// <param name="errors">An array of the <see cref="Error"/>s contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult<T> BadRequest<TCommand, T>(this ICommandHandler<TCommand, T> handler, params Error[] errors)
            where TCommand : ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>((int)HttpStatusCode.BadRequest, errors);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 400 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <typeparam name="T">The type of data returned by the operation</typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <param name="errorCode">The code of the <see cref="Error"/> contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult<T> BadRequest<TCommand, T>(this ICommandHandler<TCommand, T> handler, string errorCode, string errorMessage)
            where TCommand : ICommand<IOperationResult<T>, T>
        {
            return handler.BadRequest(new Error(errorCode, errorMessage));
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 400 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <typeparam name="T">The type of data returned by the operation</typeparam>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult<T> BadRequest<TCommand, T>(this ICommandHandler<TCommand, T> handler)
            where TCommand : ICommand<IOperationResult<T>, T>
        {
            return handler.BadRequest(null);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 404 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <typeparam name="T">The type of data returned by the operation</typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, T}"/></param>
        /// <param name="errors">An array of the <see cref="Error"/>s contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult<T> NotFound<TCommand, T>(this ICommandHandler<TCommand, T> handler, params Error[] errors)
            where TCommand : ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>((int)HttpStatusCode.NotFound, errors);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 404 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <typeparam name="T">The type of data returned by the operation</typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <param name="errorCode">The code of the <see cref="Error"/> contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult<T> NotFound<TCommand, T>(this ICommandHandler<TCommand, T> handler, string errorCode, string errorMessage)
            where TCommand : ICommand<IOperationResult<T>, T>
        {
            return handler.NotFound(new Error(errorCode, errorMessage));
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 404 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <typeparam name="T">The type of data returned by the operation</typeparam>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult<T> NotFound<TCommand, T>(this ICommandHandler<TCommand, T> handler)
            where TCommand : ICommand<IOperationResult<T>, T>
        {
            return handler.NotFound(null);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 304 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <typeparam name="T">The type of data returned by the operation</typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand, T}"/></param>
        /// <param name="errors">An array of the <see cref="Error"/>s contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult<T> NotModified<TCommand, T>(this ICommandHandler<TCommand, T> handler, params Error[] errors)
            where TCommand : ICommand<IOperationResult<T>, T>
        {
            return new OperationResult<T>((int)HttpStatusCode.NotModified, errors);
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 304 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <typeparam name="T">The type of data returned by the operation</typeparam>
        /// <param name="handler">The extended <see cref="ICommandHandler{TCommand}"/></param>
        /// <param name="errorCode">The code of the <see cref="Error"/> contained by the <see cref="IOperationResult"/> to create</param>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult<T> NotModified<TCommand, T>(this ICommandHandler<TCommand, T> handler, string errorCode, string errorMessage)
            where TCommand : ICommand<IOperationResult<T>, T>
        {
            return handler.NotModified(new Error(errorCode, errorMessage));
        }

        /// <summary>
        /// Creates a new <see cref="IOperationResult"/> with 304 status code 
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="ICommand"/> handled by the extended <see cref="ICommandHandler{TCommand}"/></typeparam>
        /// <typeparam name="T">The type of data returned by the operation</typeparam>
        /// <returns>A new <see cref="IOperationResult"/></returns>
        public static IOperationResult<T> NotModified<TCommand, T>(this ICommandHandler<TCommand, T> handler)
            where TCommand : ICommand<IOperationResult<T>, T>
        {
            return handler.NotModified(null);
        }

    }

}
