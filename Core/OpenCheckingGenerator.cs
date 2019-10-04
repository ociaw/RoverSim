using System;
using System.Collections.Generic;

namespace RoverSim
{
    /// <summary>
    /// Ensures that the starting tile isn't completely blocked in.
    /// </summary>
    public sealed class OpenCheckingGenerator : ILevelGenerator
    {
        private readonly ILevelGenerator _wrappedGenerator;

        public OpenCheckingGenerator(ILevelGenerator wrappedGenerator, Int32 minimumContiguousTiles)
        {
            _wrappedGenerator = wrappedGenerator ?? throw new ArgumentNullException(nameof(wrappedGenerator));
            MinimumContiguousTiles = minimumContiguousTiles >= 0 ? minimumContiguousTiles : throw new ArgumentOutOfRangeException(nameof(minimumContiguousTiles));
        }

        public Int32 MinimumContiguousTiles { get; } = 6;

        public Level Generate(SimulationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Level level;
            do
            {
                level = _wrappedGenerator.Generate(parameters);
            }
            while (!MeetsMinimumCount(level, parameters.InitialPosition));

            return level;
        }

        /// <summary>
        /// Checks if a level has a contiguous region of passable tiles that contains the initial position.
        /// </summary>
        /// <returns>
        /// true if <paramref name="level"/> contains the minimum nuber of contiguous tiles,
        /// or false otherwise.
        /// </returns>
        private Boolean MeetsMinimumCount(Level level, Position start)
        {
            Stack<CoordinatePair> stack = new Stack<CoordinatePair>();
            HashSet<CoordinatePair> visited = new HashSet<CoordinatePair>();

            Int32 count = 0;

            if (level[start] != TerrainType.Impassable)
                stack.Push(start);

            while (stack.Count > 0)
            {
                CoordinatePair current = stack.Pop();
                if (!visited.Add(current))
                    continue;

                count++;
                if (count == MinimumContiguousTiles)
                    return true;

                for (Int32 i = 0; i < Direction.DirectionCount; i++)
                {
                    Direction direction = (Direction)i;
                    CoordinatePair neighbor = current + direction;
                    if (!level.BottomRight.Contains(neighbor) || level[neighbor] == TerrainType.Impassable)
                        continue;

                    stack.Push(neighbor);
                }
            }

            return false;
        }
    }
}
