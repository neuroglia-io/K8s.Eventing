using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IOperationResult"/> interface
    /// </summary>
    public class OperationResult
        : IOperationResult
    {

        /// <summary>
        /// Initializes a new <see cref="OperationResult"/>
        /// </summary>
        /// <param name="statusCode">An integer representing the <see cref="IOperationResult"/>'s status code</param>
        /// <param name="errors">An array containing the <see cref="Error"/>s that have occured during the operation</param>
        public OperationResult(int statusCode, params Error[] errors)
        {
            this.StatusCode = statusCode;
            this._Errors = errors == null ? new List<Error>() : errors.ToList();
        }

        /// <inheritdoc/>
        public int StatusCode { get; }

        private List<Error> _Errors;
        /// <inheritdoc/>
        public IReadOnlyCollection<Error> Errors
        {
            get
            {
                return this._Errors.AsReadOnly();
            }
        }

        /// <inheritdoc/>
        public virtual bool HasData
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool IsSuccessfull
        {
            get
            {
                return this.StatusCode >= 200 && this.StatusCode < 300;
            }
        }

        /// <inheritdoc/>
        public virtual object GetData()
        {
            return null;
        }

        /// <inheritdoc/>
        public IOperationResult AddError(string code, string message)
        {
            this._Errors.Add(new Error(code, message));
            return this;
        }

    }

    /// <summary>
    /// Represents the default implementation of the <see cref="IOperationResult"/> interface
    /// </summary>
    /// <typeparam name="T">The type of data returned by the operation</typeparam>
    public class OperationResult<T>
        : OperationResult, IOperationResult<T>
    {

        /// <summary>
        /// Initializes a new <see cref="OperationResult"/>
        /// </summary>
        /// <param name="statusCode">An integer representing the <see cref="IOperationResult"/>'s status code</param>
        /// <param name="errors">An array containing the <see cref="Error"/>s that have occured during the operation</param>
        public OperationResult(int statusCode, params Error[] errors)
            : base(statusCode, errors)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="OperationResult"/>
        /// </summary>
        /// <param name="data">The data returned by the operation</param>
        public OperationResult(T data)
            : this((int)HttpStatusCode.OK)
        {
            this.Data = data;
        }

        /// <inheritdoc/>
        public T Data { get; }

        /// <inheritdoc/>
        public override bool HasData
        {
            get
            {
                return this.Data != null;
            }
        }

        /// <inheritdoc/>
        public override object GetData()
        {
            return this.Data;
        }

        IOperationResult<T> IOperationResult<T>.AddError(string code, string message)
        {
            this.AddError(code, message);
            return this;
        }

    }

}
