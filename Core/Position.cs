using System;

namespace RoverSim
{
    public readonly struct Position : IEquatable<Position>
    {
        public Position(Int32 x, Int32 y)
        {
            X = x;
            Y = y;
        }

        public Int32 X { get; }

        public Int32 Y { get; }

        public Boolean IsNegative => X < 0 || Y < 0;

        public Boolean IsPositive => X > 0 && Y > 0;

        public void Deconstruct(out Int32 x, out Int32 y)
        {
            x = X;
            y = Y;
        }

        public Boolean Equals(Position other) => X.Equals(other.X) && Y.Equals(other.Y);

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

        public static Position operator +(Position left, Direction right) => new Position(left.X + right.ChangeInX(), left.Y + right.ChangeInY());
    }
}
