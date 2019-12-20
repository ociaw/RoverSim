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

        /// <summary>
        /// Applies an update to this object.
        /// </summary>
        /// <returns>Whether or not the state was modified.</returns>
        public Boolean Apply(in Update update)
        {
            RoverPosition = new Position(RoverPosition + update.PositionDelta);
            if (!update.Terrain.HasValue)
                return update.PositionDelta != default;

            for (Int32 i = 0; i < AdjacentTerrain.Count; i++)
            {
                Direction dir = (Direction)i;
                CoordinatePair position = RoverPosition + dir;
                if (position.IsNegative)
                    continue; // Out of bounds

                (Int32 x, Int32 y) = position;
                _terrain[x, y] = update.Terrain.Value[dir];
            }

            return true;
        }

        public static VisibleState GenerateBlank(SimulationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return GenerateBlank(parameters.BottomRight, parameters.InitialPosition);
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
