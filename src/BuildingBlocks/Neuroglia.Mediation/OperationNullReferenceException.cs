namespace Neuroglia.Mediation
{
    /// <summary>
    /// Represents an <see cref="OperationException"/> thrown when a null reference occured during an operation
    /// </summary>
    public class OperationNullReferenceException
        : OperationException
    {

        /// <summary>
        /// Initializes a new <see cref="OperationNullReferenceException"/>
        /// </summary>
        /// <param name="message">The message of the <see cref="OperationNullReferenceException"/></param>
        public OperationNullReferenceException(string message) 
            : base(message)
        {

        }

    }

}
