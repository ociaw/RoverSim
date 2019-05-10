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
    }
}
