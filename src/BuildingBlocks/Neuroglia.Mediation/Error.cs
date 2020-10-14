namespace Neuroglia.Mediation
{
    /// <summary>
    /// Describes an error
    /// </summary>
    public class Error
    {

        /// <summary>
        /// Initializes a new <see cref="Error"/>
        /// </summary>
        /// <param name="code">The <see cref="Error"/>'s code</param>
        /// <param name="message">The <see cref="Error"/>'s message</param>
        public Error(string code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        /// <summary>
        /// Gets the <see cref="Error"/>'s code
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the <see cref="Error"/>'s message
        /// </summary>
        public string Message { get; }

    }

}
