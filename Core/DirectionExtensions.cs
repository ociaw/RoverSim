using System;

namespace RoverSim
{
    public static class DirectionExtensions
    {
        public static Int32 ChangeInX(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return 0;
                case Direction.Right:
                    return 1;
                case Direction.Down:
                    return 0;
                case Direction.Left:
                    return -1;
                default:
                    return 0;
            }
        }

        public static Int32 ChangeInY(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return -1;
                case Direction.Right:
                    return 0;
                case Direction.Down:
                    return 1;
                case Direction.Left:
                    return 0;
                default:
                    return 0;
            }
        }

        public static (Int32 x, Int32 y) NextCoords(this Direction direction, IRover rover)
        {
            if (rover == null)
                throw new ArgumentNullException(nameof(rover));

            return (rover.PosX + direction.ChangeInX(), rover.PosY + direction.ChangeInY());
        }

        public static Direction RotateCW(this Direction direction) => (Direction)((Int32)(direction + 1) % 4);

        public static Direction RotateCCW(this Direction direction) => (Direction)((Int32)(direction + 3) % 4);

        public static Direction Opposite(this Direction direction) => (Direction)((Int32)(direction + 2) % 4);
    }
}
