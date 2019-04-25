using System;

namespace RoverSim
{
    public sealed class Level
    {
        public Int32 Width { get; }

        public Int32 Height { get; }

        public Int32 CenterX => Width / 2;

        public Int32 CenterY => Height / 2;

        private TerrainType[,] Terrain { get; }

        public Level(TerrainType[,] terrain)
        {
            if (terrain == null)
                throw new ArgumentNullException(nameof(terrain));
            if (terrain.Length < 1)
                throw new ArgumentOutOfRangeException(nameof(terrain), "Must have at least one element.");

            Width = terrain.GetLength(0);
            Height = terrain.GetLength(1);
            Terrain = CloneArray(terrain);
        }

        public TerrainSquare GetTerrainSquare(Int32 x, Int32 y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return new TerrainSquare(TerrainType.Impassable);
            else
                return new TerrainSquare(Terrain[x, y]);
        }

        public MutableLevel AsMutable() => new MutableLevel(Width, Height, CloneArray(Terrain));

        private static TerrainType[,] CloneArray(TerrainType[,] original)
        {
            Int32 width = original.GetLength(0);
            Int32 height = original.GetLength(1);

            TerrainType[,] newTerrain = new TerrainType[width, height];
            Array.Copy(original, newTerrain, width * height);
            return newTerrain;
        }
    }
}
