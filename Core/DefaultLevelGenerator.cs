using System;

namespace RoverSim
{
    public sealed class DefaultLevelGenerator : ILevelGenerator
    {
        public DefaultLevelGenerator(Random random)
        {
            Random = random ?? throw new ArgumentNullException(nameof(random));
        }

        private Random Random { get; }

        public Level Generate(SimulationParameters parameters)
        {
            TerrainType[,] terrain;
            do
            {
                // Generate the terrain and ensure that the starting square isn't completely blocked in
                terrain = Generate(parameters, Random);
            }
            while (!CheckOpen(terrain, parameters.InitialPosition));

            return new Level(terrain);
        }

        private static TerrainType[,] Generate(SimulationParameters parameters, Random random)
        {
            Int32 width = parameters.BottomRight.X + 1;
            Int32 height = parameters.BottomRight.Y + 1;

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
                        if (i == parameters.InitialPosition.X && j == parameters.InitialPosition.Y)
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
        private static Boolean CheckOpen(TerrainType[,] terrain, Position position)
        {
            (Int32 x, Int32 y) = position;

            Boolean left = terrain[x - 1, y] != TerrainType.Impassable;
            Boolean right = terrain[x + 1, y] != TerrainType.Impassable;
            Boolean up = terrain[x, y - 1] != TerrainType.Impassable;
            Boolean down = terrain[x, y + 1] != TerrainType.Impassable;

            return left || right || up || down;
        }
    }
}
