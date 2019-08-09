using System;

namespace RoverSim
{
    public readonly struct Direction : IEquatable<Direction>, IComparable<Direction>
    {
        public const Int32 DirectionCount = 4;

        private Direction(Int32 value)
        {
            Value = value;
        }

        public Int32 Value { get; }

        public Boolean IsValid => Value < DirectionCount;

        public static Direction Up => new Direction(0);

        public static Direction Right => new Direction(1);

        public static Direction Down => new Direction(2);

        public static Direction Left => new Direction(3);

        public static Direction None => new Direction(4);

        public static Direction Create(Int32 value)
        {
            if (value < 0 || value > DirectionCount)
                throw new ArgumentOutOfRangeException(nameof(value), $"Must be between 0 and {DirectionCount}.");

            return new Direction(value);
        }

        public Int32 ChangeInX()
        {
            switch (Value)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 2:
                    return 0;
                case 3:
                    return -1;
                default:
                    return 0;
            }
        }

        public Int32 ChangeInY()
        {
            switch (Value)
            {
                case 0:
                    return -1;
                case 1:
                    return 0;
                case 2:
                    return 1;
                case 3:
                    return 0;
                default:
                    return 0;
            }
        }

        public (Int32 x, Int32 y) NextCoords(IRover rover)
        {
            if (rover == null)
                throw new ArgumentNullException(nameof(rover));

            return (rover.PosX + ChangeInX(), rover.PosY + ChangeInY());
        }

        public Direction Opposite() => new Direction((Value + 2) % DirectionCount);

        public Direction RotateCW() => new Direction((Value + 1) % DirectionCount);

        public Direction RotateCCW() => new Direction((Value + 3) % DirectionCount);

        public Boolean Equals(Direction other) => Value.Equals(other.Value);

        public Int32 CompareTo(Direction value) => Value.CompareTo(value.Value);

        public override Boolean Equals(Object obj) => obj is Direction direction && Equals(direction);

        public override Int32 GetHashCode() => Value.GetHashCode();

        public override String ToString()
        {
            switch (Value)
            {
                case 0:
                    return nameof(Up);
                case 1:
                    return nameof(Right);
                case 2:
                    return nameof(Down);
                case 3:
                    return nameof(Left);
                default:
                    return nameof(None);
            }
        }

        public static Boolean operator ==(Direction left, Direction right) => left.Equals(right);

        public static Boolean operator !=(Direction left, Direction right) => !(left == right);

        public static implicit operator Int32(Direction direction) => direction.Value;

        public static explicit operator Direction(Int32 value) => Create(value);
    }
}
