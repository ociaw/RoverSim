using System;

namespace RoverSim
{
    public sealed class MutableLevel
    {
        public Position BottomRight { get; }

        public TerrainType[,] Terrain { get; }

        internal MutableLevel(Position bottomRight, TerrainType[,] terrain)
        {
            BottomRight = bottomRight;
            Terrain = terrain ?? throw new ArgumentNullException(nameof(terrain));
        }

        public TerrainType GetTerrain(CoordinatePair coordinates)
        {
            if (coordinates.IsNegative || !BottomRight.Coordinates.Contains(coordinates))
                return TerrainType.Impassable;
            else
                return Terrain[coordinates.X, coordinates.Y];
        }

        public TerrainType SampleSquare(Position position)
        {
            if (!BottomRight.Contains(position))
                throw new ArgumentOutOfRangeException(nameof(position), position, "Must be contained within this level.");

            (Int32 x, Int32 y) = position;
            if (Terrain[x, y] == TerrainType.Rough)
                Terrain[x, y] = TerrainType.SampledRough;
            else if (Terrain[x, y] == TerrainType.Smooth)
                Terrain[x, y] = TerrainType.SampledSmooth;

            return Terrain[x, y];
        }
    }
}
