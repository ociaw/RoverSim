using System;
using System.Collections.Generic;

namespace RoverSim
{
    public sealed class MazeGenerator : ILevelGenerator
    {
        public MazeGenerator(Random random)
        {
            Random = random ?? throw new ArgumentNullException(nameof(random));
        }

        private Random Random { get; }

        public Level Generate(SimulationParameters parameters)
        {
            Int32 width = parameters.BottomRight.X + 1;
            Int32 height = parameters.BottomRight.Y + 1;

            TerrainType[,] terrain = new TerrainType[width, height];

            HashSet<Position> visited = new HashSet<Position>();
            Stack<Position> intersections = new Stack<Position>();
            intersections.Push(parameters.InitialPosition);
            
            while (intersections.Count > 0)
            {
                Position current = intersections.Pop();
                if (visited.Contains(current))
                    continue;

                visited.Add(current);
                for (Int32 i = 0; i < Direction.DirectionCount; i++)
                {
                    Direction direction = (Direction)i;
                    CoordinatePair passage = current + direction;
                    CoordinatePair intersection = passage + direction;
                    if (!parameters.BottomRight.Contains(intersection + direction))
                        continue;

                    Boolean createIntersection = Random.Next(0, 10) < 7;
                    if (createIntersection)
                    {
                        intersections.Push(new Position(intersection));
                        terrain[passage.X, passage.Y] = TerrainType.Smooth;
                        terrain[intersection.X, intersection.Y] = TerrainType.Smooth;
                    }
                }

            }

            return new Level(terrain);
        }
    }
}
