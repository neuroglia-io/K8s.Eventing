using System;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Represents an <see cref="Exception"/> thrown during an operation
    /// </summary>
    public class OperationException
        : Exception
    {

        /// <summary>
        /// Initializes a new <see cref="OperationException"/>
        /// </summary>
        /// <param name="message">The message of the <see cref="OperationException"/></param>
        public OperationException(string message) 
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="OperationException"/>
        /// </summary>
        /// <param name="message">The message of the <see cref="OperationException"/></param>
        /// <param name="innerException">The inner <see cref="Exception"/> wrapped by the <see cref="OperationException"/></param>
        public OperationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

    }

}
