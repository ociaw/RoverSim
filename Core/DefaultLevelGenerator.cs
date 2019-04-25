using System;

namespace RoverSim
{
    public sealed class DefaultLevelGenerator : ILevelGenerator
    {
        public DefaultLevelGenerator(Random random, Int32 width, Int32 height)
        {
            Random = random ?? throw new ArgumentNullException(nameof(random));

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, "Must be positive");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, "Must be positive");

            Width = width;
            Height = height;
        }

        private Random Random { get; }

        public Int32 Width { get; }

        public Int32 Height { get; }

        public Level Generate()
        {
            TerrainType[,] terrain;
            do
            {
                // Generate the terrain and ensure that the starting square isn't completely blocked in
                terrain = Generate(Width, Height, Random);
            }
            while (!CheckOpen(terrain));

            return new Level(Width, Height, terrain);
        }

        private static TerrainType[,] Generate(Int32 width, Int32 height, Random random)
        {
            TerrainType[,] terrain = new TerrainType[width, height];

            for (Byte i = 0; i < width; i++)
            {
                for (Byte j = 0; j < height; j++)
                {
                    if (!(i == 0 || i == width - 1 || j == 0 || j == height - 1))
                    {
                        if (random.Next(1, 4) == 1)
                            terrain[i, j] = TerrainType.Rough;
                        else
                            terrain[i, j] = TerrainType.Smooth;
                        if (random.Next(1, 11) == 1)
                            terrain[i, j] = TerrainType.Impassable;
                        if (i == width / 2 && j == height / 2)
                            terrain[i, j] = TerrainType.Smooth;
                    }
                }
            }

            return terrain;
        }

        /// <summary>
        /// Checks if at least one of the squares adjacent to the center is not impassable.
        /// </summary>
        /// <param name="terrain"></param>
        /// <returns></returns>
        private static Boolean CheckOpen(TerrainType[,] terrain)
        {
            Int32 centerX = terrain.GetLength(0) / 2;
            Int32 centerY = terrain.GetLength(1) / 2;

            Boolean left = terrain[centerX - 1, centerY] != TerrainType.Impassable;
            Boolean right = terrain[centerX + 1, centerY] != TerrainType.Impassable;
            Boolean up = terrain[centerX, centerY - 1] != TerrainType.Impassable;
            Boolean down = terrain[centerX, centerY + 1] != TerrainType.Impassable;

            return left || right || up || down;
        }
    }
}
