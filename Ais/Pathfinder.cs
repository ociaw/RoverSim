using System;
using System.Collections.Generic;
using Priority_Queue;

namespace RoverSim.Ais
{
    internal sealed class Pathfinder
    {
        private static readonly List<TerrainType> _sampleableClosed = new List<TerrainType>(2) { TerrainType.Impassable };
        private static readonly List<TerrainType> _sampleableTargets = new List<TerrainType>(2) { TerrainType.Smooth, TerrainType.Rough };
        
        private static readonly List<TerrainType> _unknownClosed = new List<TerrainType>(2) { TerrainType.Impassable, };
        private static readonly List<TerrainType> _unknownTargets = new List<TerrainType>(2) { TerrainType.Unknown };

        private static readonly List<TerrainType> _unknownSmoothClosed = new List<TerrainType>(4)
        { 
            TerrainType.Impassable, TerrainType.Rough, TerrainType.SampledRough, TerrainType.SampledSmooth
        };

        public Pathfinder(Map map)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public Map Map { get; }

        public Stack<Direction> FindNearestUnknown(Position start) => GetPathToNearest(start, _unknownClosed, _unknownTargets);

        public Stack<Direction> FindNearestUnknownThroughSmooth(Position start) => GetPathToNearest(start, _unknownSmoothClosed, _unknownTargets);

        public Stack<Direction> FindNearestSampleable(Position start) => GetPathToNearest(start, _sampleableClosed, _sampleableTargets);

        public Stack<Direction> GetPathToNearest(Position start, List<TerrainType> closedTerrain, List<TerrainType> targetTerrain)
        {
            var distances = new Dictionary<CoordinatePair, Int32>();
            var previouses = new Dictionary<CoordinatePair, CoordinatePair>();
            
            var explored = new HashSet<CoordinatePair>();
            var queue = new SimplePriorityQueue<CoordinatePair, Int32>();
            explored.Add(start);
            queue.Enqueue(start, 0);
            distances[start] = 0;
            
            while (queue.Count > 0)
            {
                CoordinatePair node = queue.Dequeue();
                TerrainType terrain = Map[node];
                if (targetTerrain.Contains(terrain) && node != start)
                {
                    // We've found the solution
                    return ExtractPath(previouses, node);
                }

                Int32 nodeDist = distances[node];
                Int32 candidateDist = nodeDist + 1;
                for (Int32 i = 0; i < Direction.DirectionCount; i++)
                {
                    Direction dir = Direction.FromInt32(i);
                    CoordinatePair neighbor = node + dir;
                    if (closedTerrain.Contains(Map[neighbor]))
                        continue;

                    if (distances.TryGetValue(neighbor, out Int32 existingDist) && candidateDist > existingDist)
                        continue;

                    distances[neighbor] = candidateDist;
                    previouses[neighbor] = node;
                    if (explored.Contains(neighbor))
                        queue.UpdatePriority(neighbor, candidateDist);
                    else
                        queue.Enqueue(neighbor, candidateDist);
                    explored.Add(neighbor);
                }
            }

            // There is no path to the destination.
            return null;
        }

        private Stack<Direction> ExtractPath(Dictionary<CoordinatePair, CoordinatePair> previouses, CoordinatePair destination)
        {
            Stack<Direction> path = new Stack<Direction>();
            
            CoordinatePair currentNode = destination;
            while (previouses.TryGetValue(currentNode, out CoordinatePair previousNode))
            {
                CoordinatePair difference = currentNode - previousNode;
                path.Push(difference.PrimaryDirection);
                currentNode = previousNode;
            }

            return path;
        }
    }
}
