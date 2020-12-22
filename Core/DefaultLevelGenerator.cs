using System;
using RandN;
using RandN.Distributions;
using RandN.Rngs;

namespace RoverSim
{
    public sealed class DefaultLevelGenerator : ILevelGenerator
    {
        private static readonly Bernoulli _roughChance = Bernoulli.FromRatio(1, 3);

        private static readonly Bernoulli _impassableChance = Bernoulli.FromRatio(1, 10);

        public Level Generate(SimulationParameters parameters, Int32 rngSeed)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            IRng rng = Pcg32.Create((UInt64)rngSeed, 0);
            // Generate the terrain and ensure that the starting square isn't completely blocked in
            TerrainType[,] terrain = Generate(parameters, rng);
            return new Level(terrain, new ProtoLevel(parameters, this, rngSeed));
        }

        private static TerrainType[,] Generate(SimulationParameters parameters, IRng rng)
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
                        if (_roughChance.Sample(rng))
                            terrain[i, j] = TerrainType.Rough;
                        else
                            terrain[i, j] = TerrainType.Smooth;
                        if (_impassableChance.Sample(rng))
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
