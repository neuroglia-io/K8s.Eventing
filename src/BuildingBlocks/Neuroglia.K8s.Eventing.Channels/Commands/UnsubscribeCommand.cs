using CloudNative.CloudEvents;
using Neuroglia.Mediation;

namespace Neuroglia.K8s.Eventing.Channels.Commands
{
    /// <summary>
    /// Represents the <see cref="ICommand"/> used to unsubscribe from <see cref="CloudEvent"/>s
    /// </summary>
    public class UnsubscribeCommand
        : Command
    {

        /// <summary>
        /// Initializes a new <see cref="UnsubscribeCommand"/>
        /// </summary>
        /// <param name="id">The id of the subscription to delete</param>
        public UnsubscribeCommand(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the id of the subscription to delete
        /// </summary>
        public string Id { get; }

    }
}
