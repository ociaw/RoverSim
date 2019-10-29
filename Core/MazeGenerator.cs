using System;
using System.Collections.Generic;

namespace RoverSim
{
    public sealed class MazeGenerator : ILevelGenerator
    {
        public MazeGenerator(SimulationParameters parameters) => Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

        public SimulationParameters Parameters { get; }

        public Level Generate(Int32 rngSeed)
        {
            Random rng = new Random(rngSeed);

            Int32 width = Parameters.BottomRight.X + 1;
            Int32 height = Parameters.BottomRight.Y + 1;

            TerrainType[,] terrain = new TerrainType[width, height];

            Position start = Parameters.InitialPosition;
            Stack<CoordinatePair> stack = new Stack<CoordinatePair>();
            HashSet<CoordinatePair> visited = new HashSet<CoordinatePair>();
            stack.Push(start);
            visited.Add(start);
            terrain[start.X, start.Y] = TerrainType.Smooth;

            while (stack.Count > 0)
            {
                var current = stack.Peek();

                Int32 offset = rng.Next(Direction.DirectionCount);
                Boolean anyNewNeighbors = false;
                for (Int32 i = 0; i < Direction.DirectionCount; i++)
                {
                    Direction direction = (Direction)((i + offset) % Direction.DirectionCount);

                    var passage = current + direction;
                    var neighbor = passage + direction;
                    var boundaryCheck = neighbor + direction;
                    if (!Parameters.BottomRight.Contains(boundaryCheck) || visited.Contains(neighbor))
                        continue;

                    anyNewNeighbors = true;
                    terrain[passage.X, passage.Y] = TerrainType.Smooth;
                    terrain[neighbor.X, neighbor.Y] = TerrainType.Smooth;
                    visited.Add(neighbor);
                    stack.Push(neighbor);
                }

                if (!anyNewNeighbors)
                {
                    if (rng.Next(5) == 0)
                    {
                        // 20 % chance of clearing a wall
                        Direction direction = (Direction)rng.Next(Direction.DirectionCount);
                        var passage = current + direction;
                        var boundaryCheck = passage + direction;
                        if (Parameters.BottomRight.Contains(boundaryCheck) && terrain[passage.X, passage.Y] == TerrainType.Impassable)
                            terrain[passage.X, passage.Y] = TerrainType.Rough;
                    }

                    stack.Pop();
                }
            }

            return new Level(terrain, new ProtoLevel(this, rngSeed));
        }
    }
}
