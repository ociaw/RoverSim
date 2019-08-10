using System;

namespace RoverSim
{
    /// <summary>
    /// Represents an X-Y coordinate pair where X and Y are any non-negative integer.
    /// </summary>
    public readonly struct Position : IEquatable<Position>, IEquatable<CoordinatePair>
    {
        public Position(Int32 x, Int32 y)
            : this(new CoordinatePair(x, y))
        { }

        public Position(CoordinatePair coordinates)
        {
            if (coordinates.IsNegative)
                throw new ArgumentOutOfRangeException(nameof(coordinates), coordinates, "Both X and Y coordinates must be non-negative.");

            Coordinates = coordinates;
        }

        public CoordinatePair Coordinates { get; }

        public Int32 X => Coordinates.X;

        public Int32 Y => Coordinates.Y;

        public void Deconstruct(out Int32 x, out Int32 y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Indicates whether or not <paramref name="other"/> is between this pair and (0, 0).
        /// </summary>
        public Boolean Contains(CoordinatePair other) => !other.IsNegative && other.X <= X && other.Y <= Y;

        /// <summary>
        /// Indicates whether or not <paramref name="other"/> is between this pair and (0, 0).
        /// </summary>
        public Boolean Contains(Position other) => other.X <= X && other.Y <= Y;

        public Boolean Equals(Position other) => Coordinates.Equals(other.Coordinates);

        public Boolean Equals(CoordinatePair other) => Coordinates.Equals(other);

        public override Boolean Equals(Object obj) => obj is Position position && Equals(position);

        public override Int32 GetHashCode()
        {
            unchecked
            {
                const Int32 multiplier = 92821;
                Int32 hash = X.GetHashCode();
                hash = hash * multiplier + Y.GetHashCode();
                return hash;
            }
        }

        public override String ToString() => $"({X}, {Y})";

        public static Boolean operator ==(Position left, Position right) => left.Equals(right);

        public static Boolean operator !=(Position left, Position right) => !(left == right);

        public static Boolean operator ==(Position left, CoordinatePair right) => left.Equals(right);

        public static Boolean operator !=(Position left, CoordinatePair right) => !(left == right);

        public static Boolean operator ==(CoordinatePair left, Position right) => right.Equals(left);

        public static Boolean operator !=(CoordinatePair left, Position right) => !(left == right);

        public static implicit operator CoordinatePair(Position position) => position.Coordinates;

        public static Position operator +(Position left, Position right) => checked(new Position(left.X + right.X, left.Y + right.Y));

        public static Position operator /(Position left, Int32 right) => right > 0 ? new Position(left.X / right, left.Y / right) : throw new ArgumentOutOfRangeException(nameof(right));

        public static CoordinatePair operator +(Position left, CoordinatePair right) => left.Coordinates + right;

        public static CoordinatePair operator +(CoordinatePair left, Position right) => right + left;

        public static CoordinatePair operator +(Position left, Direction right) => left + right.Delta;
    }
}
