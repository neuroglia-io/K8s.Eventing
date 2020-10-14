using System.Collections.Generic;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Defines the fundamentals of an operation result
    /// </summary>
    public interface IOperationResult
    {

        /// <summary>
        /// Gets an integer representing the <see cref="IOperationResult"/>'s status code
        /// </summary>
        int StatusCode { get; }

        /// <summary>
        /// Gets an <see cref="IReadOnlyCollection{T}"/> containing the <see cref="Error"/>s that have occured during the operation
        /// </summary>
        IReadOnlyCollection<Error> Errors { get; }

        /// <summary>
        /// Gets a boolean indicating whether or not the <see cref="IOperationResult"/> contains data
        /// </summary>
        bool HasData { get; }

        /// <summary>
        /// Gets a boolean indicating whether or not the operation was successfull
        /// </summary>
        bool IsSuccessfull { get; }

        /// <summary>
        /// Gets the data returned by the operation, if any
        /// </summary>
        /// <returns>The data returned by the operation, if any</returns>
        object GetData();

        /// <summary>
        /// Adds an <see cref="Error"/> to the <see cref="IOperationResult"/>
        /// </summary>
        /// <param name="code">The code of the <see cref="Error"/> to add</param>
        /// <param name="message">The message of the <see cref="Error"/> to add</param>
        /// <returns>The <see cref="IOperationResult"/>, for chaining purposes</returns>
        IOperationResult AddError(string code, string message);

    }

    /// <summary>
    /// Defines the fundamentals of an operation result
    /// </summary>
    /// <typeparam name="T">The type of data returned by the operation</typeparam>
    public interface IOperationResult<T>
        : IOperationResult
    {

        /// <summary>
        /// Gets the data returned by the operation
        /// </summary>
        T Data { get; }

        /// <summary>
        /// Adds an <see cref="Error"/> to the <see cref="IOperationResult"/>
        /// </summary>
        /// <param name="code">The code of the <see cref="Error"/> to add</param>
        /// <param name="message">The message of the <see cref="Error"/> to add</param>
        /// <returns>The <see cref="IOperationResult"/>, for chaining purposes</returns>
        new IOperationResult<T> AddError(string code, string message);

    }

}
