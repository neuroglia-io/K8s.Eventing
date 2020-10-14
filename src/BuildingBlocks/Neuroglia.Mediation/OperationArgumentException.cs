namespace Neuroglia.Mediation
{
    /// <summary>
    /// Represents an <see cref="OperationException"/> thrown when one of the argument of an operation is not valid
    /// </summary>
    public class OperationArgumentException
        : OperationException
    {

        /// <summary>
        /// Initializes a new <see cref="OperationArgumentException"/>
        /// </summary>
        /// <param name="message">The message of the <see cref="OperationArgumentException"/></param>
        /// <param name="argument">The name of the invalid argument</param>
        public OperationArgumentException(string message, string argument) 
            : base(message)
        {
            this.Argument = argument;
        }

        /// <summary>
        /// Gets the name of the invalid argument
        /// </summary>
        public string Argument { get; }

    }

}
