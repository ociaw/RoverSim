using System;

namespace RoverSim
{
    public sealed class Level
    {
        public Int32 Width { get; } = 32;

        public Int32 Height { get; } = 23;

        public Int32 CenterX => Width / 2;

        public Int32 CenterY => Height / 2;

        public TerrainType[,] Terrain { get; }

        public Level(Int32 width, Int32 height, TerrainType[,] terrain)
        {
            if (width < 1)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 1)
                throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            Terrain = terrain ?? throw new ArgumentNullException(nameof(terrain));
        }

        public TerrainSquare GetTerrainSquare(Int32 x, Int32 y)
        {
            if (x < 0 || y < 0 || x >= Width - 1 || y >= Height - 1)
                return new TerrainSquare(TerrainType.Impassable);
            else
                return new TerrainSquare(Terrain[x, y]);
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

        public Level Clone()
        {
            TerrainType[,] originalTerrain = new TerrainType[Width, Height];
            Array.Copy(Terrain, originalTerrain, Width * Height);
            return new Level(Width, Height, originalTerrain);
        }
    }
}
