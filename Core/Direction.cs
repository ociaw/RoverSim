using System;

namespace RoverSim
{
    public readonly struct Direction : IEquatable<Direction>
    {
        public const Int32 DirectionCount = 4;

        private Direction(Int32 value)
        {
            Value = value;
        }

        public Int32 Value { get; }

        public Boolean IsValid => Value < DirectionCount;

        public CoordinatePair Delta
        {
            get
            {
                Int32 x = 0;
                switch (Value)
                {
                    case 1:
                        x = 1;
                        break;
                    case 3:
                        x = -1;
                        break;
                }
                Int32 y = 0;
                switch (Value)
                {
                    case 0:
                        y = -1;
                        break;
                    case 2:
                        y = 1;
                        break;
                }

                return new CoordinatePair(x, y);
            }
        }

        public static Direction Up => new Direction(0);

        public static Direction Right => new Direction(1);

        public static Direction Down => new Direction(2);

        public static Direction Left => new Direction(3);

        public static Direction None => new Direction(4);

        public static Direction FromInt32(Int32 value)
        {
            if (value < 0 || value > DirectionCount)
                throw new ArgumentOutOfRangeException(nameof(value), $"Must be between 0 and {DirectionCount}.");

            return new Direction(value);
        }

        public Direction Opposite() => new Direction((Value + 2) % DirectionCount);

        public Direction RotateCW() => new Direction((Value + 1) % DirectionCount);

        public Direction RotateCCW() => new Direction((Value + 3) % DirectionCount);

        public Boolean Equals(Direction other) => Value.Equals(other.Value);

        public override Boolean Equals(Object obj) => obj is Direction direction && Equals(direction);

        public override Int32 GetHashCode() => Value.GetHashCode();

        public override String ToString()
        {
            return Value switch
            {
                0 => nameof(Up),
                1 => nameof(Right),
                2 => nameof(Down),
                3 => nameof(Left),
                _ => nameof(None),
            };
        }

        public static Boolean operator ==(Direction left, Direction right) => left.Equals(right);

        public static Boolean operator !=(Direction left, Direction right) => !(left == right);

        public static implicit operator Int32(Direction direction) => direction.Value;

        public static explicit operator Direction(Int32 value) => FromInt32(value);
    }
}
