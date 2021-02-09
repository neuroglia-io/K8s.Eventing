using CloudNative.CloudEvents.Extensions;

namespace Neuroglia.K8s.Eventing.Extensions
{

    /// <summary>
    /// Represents a long-based <see cref="SequenceExtension"/>
    /// </summary>
    public class LongSequenceExtension
        : SequenceExtension
    {

        /// <summary>
        /// Initializes a new <see cref="LongSequenceExtension"/>
        /// </summary>
        /// <param name="sequenceValue">The sequence value</param>
        public LongSequenceExtension(long? sequenceValue = null) 
            : base()
        {
            base.SequenceType = "long";
            this.Sequence = sequenceValue;
        }

        /// <summary>
        /// Gets/sets the sequence value
        /// </summary>
        public new long? Sequence
        {
            get
            {
                var sequence = base.Sequence;
                if (!string.IsNullOrWhiteSpace(sequence))
                    return long.Parse(sequence);
                return null;
            }
            set
            {
                base.Sequence = value?.ToString();
            }
        }

    }

}
