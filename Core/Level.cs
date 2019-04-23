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

        private Level(Int32 width, Int32 height, TerrainType[,] terrain)
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

        public static Level Create(Random random) => Create(random, 32, 23);

        public static Level Create(Random random, Int32 width, Int32 height)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, "Must be positive");
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, "Must be positive");

            TerrainType[,] terrain;
            do
            {
                // Generate the terrain and ensure that the starting square isn't completely blocked in
                terrain = Generate(width, height, random);
            }
            while (!CheckOpen(terrain));
            
            return new Level(width, height, terrain);
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
