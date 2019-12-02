using System;

namespace RoverSim.Ais
{
    public sealed class Map
    {
        private readonly TerrainType[,] _map;

        private Map(SimulationParameters parameters, TerrainType[,] map)
        {
            Parameters = parameters;
            _map = map;
        }

        public SimulationParameters Parameters { get; }

        public TerrainType this[Position index]
        {
            get => Contains(index) ? _map[index.X, index.Y]: TerrainType.Impassable;
            private set => _map[index.X, index.Y] = value;
        }

        public TerrainType this[CoordinatePair index] => Contains(index) ? this[new Position(index)] : TerrainType.Impassable;

        public static Map Create(SimulationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            (Int32 x, Int32 y) = parameters.BottomRight;
            var map = new TerrainType[x + 1, y + 1];

            for (Int32 i = 0; i <= x; i++)
            {
                for (Int32 j = 0; j <= y; j++)
                    map[i, j] = TerrainType.Unknown;
            }

            return new Map(parameters, map);
        }
        
        public Boolean Contains(CoordinatePair coordinates) => Parameters.BottomRight.Contains(coordinates);

        public Boolean Contains(Position position) => Parameters.BottomRight.Contains(position);

        public void UpdateTerrain(Position center, AdjacentTerrain adjacentTerrain)
        {
            for (Int32 dir = 0; dir < Direction.DirectionCount; dir++)
            {
                Direction direction = Direction.FromInt32(dir);
                CoordinatePair coords = center + direction;
                if (!Parameters.BottomRight.Contains(coords))
                    continue;

                Position position = new Position(coords);
                UpdateTerrain(position, adjacentTerrain[direction]);
            }
        }

        public void UpdateTerrain(Position position, TerrainType terrain) => this[position] = terrain;

        public Int32 CountNeighborsOfType(CoordinatePair coordinates, TerrainType terrain)
        {
            Int32 count = 0;
            for (Int32 i = 0; i < Direction.DirectionCount; i++)
            {
                Direction direction = Direction.FromInt32(i);
                CoordinatePair neighbor = coordinates + direction;
                if (terrain == this[neighbor])
                    count++;
            }

            return count;
        }
    }
}
