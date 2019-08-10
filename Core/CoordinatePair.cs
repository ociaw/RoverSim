﻿using System;

namespace RoverSim
{
    /// <summary>
    /// Represents an X-Y coordinate pair where X and Y are any integer.
    /// </summary>
    public readonly struct CoordinatePair
    {
        public CoordinatePair(Int32 x, Int32 y)
        {
            X = x;
            Y = y;
        }

        public Int32 X { get; }

        public Int32 Y { get; }

        /// <summary>
        /// Indicates whether or not either coordinate is negative.
        /// </summary>
        public Boolean IsNegative => X < 0 || Y < 0;

        public void Deconstruct(out Int32 x, out Int32 y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Indicates whether or not <paramref name="other"/> is between this pair and (0, 0).
        /// </summary>
        public Boolean Contains(CoordinatePair other)
        {
            return Math.Sign(X) == Math.Sign(other.X) && Math.Abs(X) >= Math.Abs(other.X)
                && Math.Sign(Y) == Math.Sign(other.Y) && Math.Abs(Y) >= Math.Abs(other.Y);
        }

        public Boolean Equals(CoordinatePair other) => X.Equals(other.X) && Y.Equals(other.Y);

        public override Boolean Equals(Object obj) => obj is CoordinatePair coordinates && Equals(coordinates);

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

        public static Boolean operator ==(CoordinatePair left, CoordinatePair right) => left.Equals(right);

        public static Boolean operator !=(CoordinatePair left, CoordinatePair right) => !(left == right);

        public static CoordinatePair operator +(CoordinatePair left, CoordinatePair right) => new CoordinatePair(left.X + right.X, left.Y + right.Y);

        public static CoordinatePair operator +(CoordinatePair left, Direction right) => left + right.Delta;
    }
}
