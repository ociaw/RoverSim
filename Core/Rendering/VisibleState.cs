using System;

namespace RoverSim.Rendering
{
    /// <summary>
    /// Contains the information necessary to render the rover.
    /// </summary>
    public sealed class VisibleState
    {
        private readonly TerrainType[,] _terrain;

        private VisibleState(Position bottomRight, TerrainType[,] terrain, Position roverPosition)
        {
            BottomRight = bottomRight;
            _terrain = terrain;
            RoverPosition = roverPosition;
        }

        public Position BottomRight { get; }

        public Int32 Width => BottomRight.X + 1;

        public Int32 Height => BottomRight.Y + 1;

        public TerrainType this[Int32 x, Int32 y] => _terrain[x, y];

        public Position RoverPosition { get; private set; }

        public void UpdateRoverPos(PositionUpdate update)
        {
            if (!BottomRight.Contains(update.Previous))
                throw new ArgumentOutOfRangeException(nameof(update), update.Previous, "Previous position must lie within bottom right position.");
            if (!BottomRight.Contains(update.New))
                throw new ArgumentOutOfRangeException(nameof(update), update.New, "New position must lie within bottom right position.");

            RoverPosition = update.New;
        }

        public void UpdateTerrain(TerrainUpdate update)
        {
            if (!BottomRight.Contains(update.Position))
                return; // We can ignore out of bound updates, as these are not accessible anyway.

            (Int32 x, Int32 y) = update.Position;
            _terrain[x, y] = update.NewTerrain;
        }

        public static VisibleState GenerateBlank(Position bottomRight, Position roverPos)
        {
            if (!bottomRight.Contains(roverPos))
                throw new ArgumentOutOfRangeException(nameof(roverPos), roverPos, "Must lie within bottom right position.");

            (Int32 x, Int32 y) = bottomRight;
            Int32 width = x + 1;
            Int32 height = y + 1;
            TerrainType[,] terrain = new TerrainType[width, height];
            for (Byte i = 1; i < width - 1; i++)
            {
                for (Byte j = 1; j < height - 1; j++)
                {
                    terrain[i, j] = TerrainType.Unknown;
                }
            }

            return new VisibleState(bottomRight, terrain, roverPos);
        }
    }
}