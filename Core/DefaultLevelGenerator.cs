using System;
using System.Collections.Generic;

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
            while (CountOpen(terrain, parameters.InitialPosition, 6) < 6);

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

        /// <summary>
        /// Counts the number of contiguous non-impassable terrain tiles throuh a BFS.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="limit">The maximum number of matching tiles to count.</param>
        private static Int32 CountOpen(TerrainType[,] terrain, Position start, Int32 limit)
        {
            Int32 width = terrain.GetLength(0);
            Int32 height = terrain.GetLength(1);

            Stack<CoordinatePair> stack = new Stack<CoordinatePair>();
            HashSet<CoordinatePair> visited = new HashSet<CoordinatePair>();

            Int32 count = 0;

            if (terrain[start.X, start.Y] != TerrainType.Impassable)
                stack.Push(start);

            while (stack.Count > 0)
            {
                CoordinatePair current = stack.Pop();
                if (!visited.Add(current))
                    continue;

                count++;
                if (count == limit)
                    return count;

                for (Int32 i = 0; i < Direction.DirectionCount; i++)
                {
                    Direction direction = (Direction)i;
                    CoordinatePair neighbor = current + direction;
                    if (neighbor.X >= width || neighbor.Y >= height || neighbor.X < 0 || neighbor.Y < 0 || terrain[neighbor.X, neighbor.Y] == TerrainType.Impassable)
                        continue;

                    stack.Push(neighbor);
                }
            }

            return count;
        }
    }
}
