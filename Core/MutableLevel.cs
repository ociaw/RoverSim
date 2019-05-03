using System;

namespace RoverSim
{
    public sealed class MutableLevel
    {
        public Int32 Width { get; }

        public Int32 Height { get; }

        public Int32 CenterX => Width / 2;

        public Int32 CenterY => Height / 2;

        public TerrainType[,] Terrain { get; }

        internal MutableLevel(Int32 width, Int32 height, TerrainType[,] terrain)
        {
            if (width < 1)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 1)
                throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            Terrain = terrain ?? throw new ArgumentNullException(nameof(terrain));
        }

        public TerrainType GetTerrain(Int32 x, Int32 y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return TerrainType.Impassable;
            else
                return Terrain[x, y];
        }

        public TerrainType SampleSquare(Int32 x, Int32 y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException(nameof(y));

            if (Terrain[x, y] == TerrainType.Rough)
                Terrain[x, y] = TerrainType.SampledRough;
            else if (Terrain[x, y] == TerrainType.Smooth)
                Terrain[x, y] = TerrainType.SampledSmooth;

            return Terrain[x, y];
        }
    }
}
