using System;

namespace Neuroglia.K8s.Eventing.Gateway.Integration
{

    /// <summary>
    /// Represents a structure used to wrap a position in a cloud event stream
    /// </summary>
    public struct StreamPosition
        : IEquatable<long>
    {

        /// <summary>
        /// Initializes a new <see cref="StreamPosition"/>
        /// </summary>
        /// <param name="position">The position</param>
        public StreamPosition(long position)
        {
            if (position < -2)
                throw new ArgumentOutOfRangeException(nameof(position));
            this.Position = position;
        }

        /// <summary>
        /// Gets/sets the position
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// Starts from the begining of the cloud event stream
        /// </summary>
        public static StreamPosition Start
        {
            get
            {
                return new StreamPosition(-1);
            }
        }

        /// <summary>
        /// Starts at the end of the cloud event stream
        /// </summary>
        public static StreamPosition End
        {
            get
            {
                return new StreamPosition(-2);
            }
        }

        /// <summary>
        /// Determines whether or not the <see cref="StreamPosition"/> matches the specified position
        /// </summary>
        /// <param name="other">The position to compare</param>
        /// <returns>A boolean indicating whether or not the <see cref="StreamPosition"/> matches the specified position</returns>
        public bool Equals(long other)
        {
            return this.Position == other;
        }

        /// <summary>
        /// Determines whether or not the <see cref="StreamPosition"/> matches the specified object
        /// </summary>
        /// <param name="obj">The position to compare</param>
        /// <returns>A boolean indicating whether or not the <see cref="StreamPosition"/> matches the specified object</returns>
        public override bool Equals(object obj)
        {
            if (obj is StreamPosition streamPosition)
                return this == streamPosition;
            if (obj is long position)
                return this.Position == position;
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Implicitly converts the specified <see cref="StreamPosition"/> into its <see cref="long"/> equivalency
        /// </summary>
        /// <param name="position">The <see cref="StreamPosition"/> to convert</param>
        public static implicit operator long(StreamPosition position)
        {
            return position.Position;
        }

        /// <summary>
        /// Determines whether or not the <see cref="StreamPosition"/> matches the specified position
        /// </summary>
        /// <param name="streamPosition">The <see cref="StreamPosition"/> to compare</param>
        /// <param name="position">The position to compare</param>
        /// <returns>A boolean indicating whether or not the <see cref="StreamPosition"/> matches the specified position</returns>
        public static bool operator ==(StreamPosition streamPosition, long position)
        {
            return streamPosition.Position == position;
        }

        /// <summary>
        /// Determines whether or not the <see cref="StreamPosition"/> does not match the specified position
        /// </summary>
        /// <param name="streamPosition">The <see cref="StreamPosition"/> to compare</param>
        /// <param name="position">The position to compare</param>
        /// <returns>A boolean indicating whether or not the <see cref="StreamPosition"/> does not match the specified position</returns>
        public static bool operator !=(StreamPosition streamPosition, long position)
        {
            return !(streamPosition == position);
        }

        /// <summary>
        /// Determines whether or not the specified <see cref="StreamPosition"/>s match
        /// </summary>
        /// <param name="a">The first <see cref="StreamPosition"/> to compare</param>
        /// <param name="b">The second <see cref="StreamPosition"/> to compare</param>
        /// <returns>A boolean indicating whether or not the specified <see cref="StreamPosition"/>s match</returns>
        public static bool operator ==(StreamPosition a, StreamPosition b)
        {
            return a.Position == b.Position;
        }

        /// <summary>
        /// Determines whether or not the specified <see cref="StreamPosition"/>s don't match
        /// </summary>
        /// <param name="a">The first <see cref="StreamPosition"/> to compare</param>
        /// <param name="b">The second <see cref="StreamPosition"/> to compare</param>
        /// <returns>A boolean indicating whether or not the specified <see cref="StreamPosition"/>s don't match</returns>
        public static bool operator !=(StreamPosition a, StreamPosition b)
        {
            return !(a == b);
        }

    }

}
