using System;

namespace RoverSim
{
    public sealed class MutableLevel
    {
        private readonly TerrainType[,] _terrain;

        public Position BottomRight { get; }

        internal MutableLevel(Position bottomRight, TerrainType[,] terrain)
        {
            BottomRight = bottomRight;
            _terrain = terrain ?? throw new ArgumentNullException(nameof(terrain));
        }

        public TerrainType GetTerrain(CoordinatePair coordinates)
        {
            if (coordinates.IsNegative || !BottomRight.Coordinates.Contains(coordinates))
                return TerrainType.Impassable;
            else
                return _terrain[coordinates.X, coordinates.Y];
        }

        public TerrainType SampleSquare(Position position)
        {
            if (!BottomRight.Contains(position))
                throw new ArgumentOutOfRangeException(nameof(position), position, "Must be contained within this level.");

            (Int32 x, Int32 y) = position;
            if (_terrain[x, y] == TerrainType.Rough)
                _terrain[x, y] = TerrainType.SampledRough;
            else if (_terrain[x, y] == TerrainType.Smooth)
                _terrain[x, y] = TerrainType.SampledSmooth;

            return _terrain[x, y];
        }
    }
}
