using System;

namespace RoverSim
{
    public sealed class DefaultLevelGenerator : ILevelGenerator
    {
        public Level Generate(SimulationParameters parameters, Int32 rngSeed)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Random rng = new Random(rngSeed);
            // Generate the terrain and ensure that the starting square isn't completely blocked in
            TerrainType[,] terrain = Generate(parameters, rng);
            return new Level(terrain);
        }

        private static TerrainType[,] Generate(SimulationParameters parameters, Random random)
        {
            Int32 width = parameters.BottomRight.X + 1;
            Int32 height = parameters.BottomRight.Y + 1;

            (Int32 initialX, Int32 initialY) = parameters.InitialPosition;

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
                        if (i == initialX && j == initialY)
                            terrain[i, j] = TerrainType.Smooth;
                    }
                }
            }

            return terrain;
        }
    }
}
