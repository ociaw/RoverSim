using System;
using System.Collections.Generic;

namespace RoverSim.Ais
{
    /// <summary>
    /// A tweaked version of the Scratch Mark I AI. Maintains a very small and fixed amount of memory.
    /// </summary>
    public sealed class PathfindingAi : IAi
    {
        private readonly Map _map;

        private readonly Pathfinder _pathfinder;

        private Stack<Direction> _path;

        internal PathfindingAi(SimulationParameters parameters, Map map)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _pathfinder = new Pathfinder(map);
        }

        public SimulationParameters Parameters { get; }

        public Int32 LowPowerThreshold => Parameters.MoveSmoothCost * 2 + Parameters.SampleCost;

        public IAi CloneFresh() => new PathfindingAi(Parameters, Map.Create(Parameters));

        public IEnumerable<RoverAction> Simulate(IRoverStatusAccessor rover)
        {
            foreach (var action in GatherPower(rover))
                yield return action;

            while (true)
            {
                var adjacent = rover.Adjacent;
                TerrainType occupied = adjacent[Direction.None];

                if (rover.MovesLeft <= 5)
                {
                    foreach (var action in DoLowMoves(rover, adjacent))
                        yield return action;
                    yield break;
                }

                if (occupied.IsSampleable())
                {
                    yield return RoverAction.CollectSample;
                    UpdateMap(rover);
                    if (rover.SamplesCollected >= Parameters.SamplesPerProcess && rover.Power > Parameters.ProcessCost + Parameters.MoveSmoothCost)
                        yield return RoverAction.ProcessSamples;
                }

                Direction nextMove = Direction.None;
                if (_path != null && _path.Count > 1)
                    nextMove = _path.Pop();
                else if (FindAdjacentSampleable(adjacent).HasValue)
                    nextMove = FindAdjacentSampleable(adjacent).Value;

                if (nextMove == Direction.None || adjacent[nextMove] == TerrainType.Impassable)
                {
                    Direction? reset = ResetDestination(rover.Position);
                    if (reset == null)
                        yield break; // We're stuck.

                    nextMove = reset.Value;
                }

                yield return new RoverAction(nextMove);
                UpdateMap(rover);
            }
        }

        private Direction? ResetDestination(Position roverPosition)
        {
            _path = _pathfinder.FindNearestSampleable(roverPosition);

            if (_path == null)
                return null;

            return _path.Pop();
        }

        private IEnumerable<RoverAction> GatherPower(IRoverStatusAccessor rover)
        {
            UpdateMap(rover);
            foreach (var action in Explore(rover))
                yield return action;
            foreach (var action in Exploit(rover))
                yield return action;
        }

        /// <summary>
        /// Searches for at least two contiguous smooth tiles to later gather power from.
        /// </summary>
        private IEnumerable<RoverAction> Explore(IRoverStatusAccessor rover)
        {
            Position center = Parameters.BottomRight / 2;
            while (true)
            {
                Position position = rover.Position;
                // First we check to see if we've found a position that we can gather power from.
                for (Int32 i = 0; i < Direction.DirectionCount + 1; i++)
                {
                    Direction direction = Direction.FromInt32(i);
                    CoordinatePair neighbor = position + direction;
                    if (_map[neighbor] == TerrainType.Smooth && _map.CountNeighborsOfType(neighbor, TerrainType.Smooth) > 0)
                    {
                        // If we have, break out of the exploration phase.
                        Int32 requiredPower = Parameters.GetMovementPowerCost(rover.Adjacent[direction]) + 1;
                        foreach (var action in EnsureSufficientPower(rover, requiredPower))
                            yield return action;
                        yield return new RoverAction(direction);
                        UpdateMap(rover);
                        yield break;
                    }
                }

                Direction exploreDirection = FindNextExploreDirection(center, position);

                if (exploreDirection != Direction.None)
                {
                    Int32 requiredPower = Parameters.GetMovementPowerCost(rover.Adjacent[exploreDirection]) + 1;
                    foreach (var action in EnsureSufficientPower(rover, requiredPower))
                        yield return action;
                    yield return new RoverAction(exploreDirection);
                    UpdateMap(rover);
                    continue;
                }

                // None of our immediate neighbors have any unknown neighbors, so just find a path towards the nearest unknown tile.
                var path = _pathfinder.FindNearestUnknown(position);
                while (path.Count > 1) // Don't move on to the unknown tile, just next to it.
                {
                    Int32 requiredPower = Parameters.GetMovementPowerCost(rover.Adjacent[path.Peek()]) + 1;
                    foreach (var action in EnsureSufficientPower(rover, requiredPower))
                        yield return action;
                    yield return new RoverAction(path.Pop());
                    UpdateMap(rover);
                }
            }
        }

        private Direction FindNextExploreDirection(Position center, Position roverPos)
        {
            // Determine next exploration square
            // Prioritize directions to check based on potential squares
            Span<Direction> testDirections = new Direction[4];
            if (Parameters.BottomRight.X >= Parameters.BottomRight.Y)
            {
                FillHorizontal(testDirections, center, roverPos);
                FillVertical(testDirections.Slice(2, 2), center, roverPos);
            }
            else
            {
                FillVertical(testDirections, center, roverPos);
                FillHorizontal(testDirections.Slice(2, 2), center, roverPos);
            }

            // We look for smooth tiles first, then we want the neighbor with the most number of unknown tiles.
            Int32 bestUnknownCount = 0;
            Direction bestUnknownDirection = Direction.None;
            TerrainType bestTerrain = TerrainType.Impassable;
            for (Int32 i = 0; i < testDirections.Length; i++)
            {
                Direction dir = testDirections[i];
                CoordinatePair neighbor = roverPos + dir;
                if (_map[neighbor] == TerrainType.Impassable)
                    continue;

                Int32 unknownCount = _map.CountNeighborsOfType(neighbor, TerrainType.Unknown);
                if (unknownCount == 0)
                    continue;
                if (bestTerrain == TerrainType.Smooth && _map[neighbor] != TerrainType.Smooth)
                    continue;
                if (unknownCount <= bestUnknownCount)
                    continue;
                bestUnknownCount = unknownCount;
                bestUnknownDirection = dir;
                bestTerrain = _map[neighbor];
            }

            return bestUnknownDirection;
        }

        /// <summary>
        /// Gathers power by staying on smooth squares and revealing as much as possible until the threshold is reached.
        /// </summary>
        private IEnumerable<RoverAction> Exploit(IRoverStatusAccessor rover)
        {
            Stack<Direction> pathToUnknown = _pathfinder.FindNearestUnknownThroughSmooth(rover.Position);
            Boolean isPathDirty = false;
            while (!HasExcessPower(rover, rover.CollectablePower))
            {
                Int32 noBacktrackTarget = CalculateNoBacktrackTarget(rover);
                Int32 movesToTarget = noBacktrackTarget - rover.NoBacktrack;
                Int32 powerNeeded = movesToTarget * Parameters.MoveSmoothCost + 1;
                Int32 excessPower = rover.Power - powerNeeded;

                Direction? preferredDir = _map.FindNeighborWithMultipleSmoothNeighbors(rover.Position);
                if (rover.Adjacent.Occupied == TerrainType.Smooth && preferredDir.HasValue && excessPower > 0 && !rover.IsHopperFull)
                {
                    yield return RoverAction.CollectSample;
                    yield return new RoverAction(preferredDir.Value);
                    UpdateMap(rover);
                    isPathDirty = true;
                    continue;
                }

                if (rover.Power < Parameters.MoveSmoothCost + 1)
                {
                    Int32 cost = Parameters.GetMovementPowerCost(TerrainType.Smooth);
                    Int32 requiredPower;
                    if (cost <= 3)
                    {
                        requiredPower = cost + 1;
                    }
                    else
                    {
                        Int32 movesNeeded = (Int32)(Math.Ceiling(Math.Sqrt(4 * cost - 3) - 3) / 2);
                        requiredPower = movesNeeded * cost + 1;
                    }

                    foreach (var action in EnsureSufficientPower(rover, requiredPower))
                        yield return action;
                    continue;
                }

                if (pathToUnknown != null)
                {
                    // We reveal unknown tiles as we go, but without leaving the smooth area.
                    if (pathToUnknown.Count <= 1 || isPathDirty)
                    {
                        pathToUnknown = _pathfinder.FindNearestUnknownThroughSmooth(rover.Position);
                        isPathDirty = false;
                        continue;
                    }
                    
                    yield return new RoverAction(pathToUnknown.Pop());
                }
                else
                {
                    Direction? smoothDir = FindAdjacentSmooth(rover.Adjacent);
                    yield return new RoverAction(smoothDir.Value);
                }
                UpdateMap(rover);
            }

            yield return RoverAction.CollectPower;
        }

        private void FillHorizontal(Span<Direction> directions, Position center, Position roverPos)
        {
            if (roverPos.X < center.X)
            {
                directions[0] = Direction.Right;
                directions[1] = Direction.Left;
                return;
            }

            directions[0] = Direction.Left;
            directions[1] = Direction.Right;
        }

        private void FillVertical(Span<Direction> directions, Position center, Position roverPos)
        {
            if (roverPos.Y < center.Y)
            {
                directions[0] = Direction.Down;
                directions[1] = Direction.Up;
                return;
            }

            directions[0] = Direction.Up;
            directions[1] = Direction.Down;
        }

        private IEnumerable<RoverAction> EnsureSufficientPower(IRoverStatusAccessor rover, Int32 target)
        {
            if (rover.Power >= target)
                return System.Linq.Enumerable.Empty<RoverAction>();

            Int32 powerDeficit = Math.Max(target - rover.Power - rover.CollectablePower, 0);
            return System.Linq.Enumerable.Repeat(RoverAction.CollectPower, powerDeficit + 1);
        }

        private Boolean HasExcessPower(IRoverStatusAccessor rover, Int32 potentialPower)
        {
            Int32 targetPower = CalculateExcessPowerThershold(rover);
            return rover.Power + potentialPower >= targetPower;
        }

        private Int32 CalculateExcessPowerThershold(IRoverStatusAccessor rover)
        {
            const Double smoothRoughRatio = 1;
            Double meanUnsampledMoveCost = (smoothRoughRatio * Parameters.MoveSmoothCost + Parameters.MoveRoughCost) / (smoothRoughRatio + 1.0);
            Double processCostPerSample = (Double)Parameters.ProcessCost / Parameters.SamplesPerProcess;
            Double meanSampleSequenceCost = meanUnsampledMoveCost + Parameters.SampleCost + processCostPerSample;

            Int32 maxSampleSequenceCount = (rover.MovesLeft - 1) / 3;

            Int32 targetPower = (Int32)(maxSampleSequenceCount * meanSampleSequenceCost) + Parameters.TransmitCost;
            return targetPower;
        }

        private Int32 CalculateNoBacktrackTarget(IRoverStatusAccessor rover)
        {
            Int32 power = CalculateExcessPowerThershold(rover);
            return (Int32)Math.Ceiling(Math.Pow(power, 1.0 / 3.0));
        }

        private IEnumerable<RoverAction> DoLowMoves(IRoverStatusAccessor rover, AdjacentTerrain adjacent)
        {
            if (rover.MovesLeft > 5)
                yield break;

            Boolean smoothOccupied = adjacent[Direction.None] == TerrainType.Smooth;
            Boolean roughOccupied = adjacent[Direction.None] == TerrainType.Rough;
            Direction? sampleableDir = FindAdjacentSampleable(adjacent);
            if (rover.MovesLeft == 5 && rover.Power > Parameters.SampleCost + Parameters.MoveRoughCost + Parameters.SampleCost + Parameters.ProcessCost)
            {
                if (smoothOccupied || roughOccupied)
                {
                    yield return RoverAction.CollectSample;
                    UpdateMap(rover);
                }

                Direction? moveDir = sampleableDir;
                if (moveDir.HasValue)
                {
                    yield return new RoverAction(moveDir.Value);
                    yield return RoverAction.CollectSample;
                    UpdateMap(rover);
                }

                yield return RoverAction.ProcessSamples;
                yield return RoverAction.Transmit;
                yield break;
            }

            if (rover.MovesLeft >= 4)
            {
                if (rover.Power < Parameters.ProcessCost + Parameters.SampleCost + Parameters.ProcessCost)
                    yield return RoverAction.CollectPower;
                if (rover.Power > Parameters.ProcessCost)
                {
                    if (rover.Power > Parameters.ProcessCost + Parameters.SampleCost && smoothOccupied)
                    {
                        yield return RoverAction.CollectSample;
                        UpdateMap(rover);
                    }

                    yield return RoverAction.ProcessSamples;
                }
            }
            else if (rover.MovesLeft == 3)
            {
                if (rover.Power > Parameters.ProcessCost + Parameters.SampleCost && smoothOccupied)
                {
                    yield return RoverAction.CollectSample;
                    UpdateMap(rover);
                }
                else
                {
                    yield return RoverAction.CollectPower;
                }
                if (rover.Power > Parameters.ProcessCost)
                    yield return RoverAction.ProcessSamples;
            }
            if (rover.MovesLeft == 2)
            {
                if (rover.Power == 0)
                    yield return RoverAction.CollectPower;
                else if (rover.Power > Parameters.ProcessCost && rover.SamplesCollected > 0)
                    yield return RoverAction.ProcessSamples;
            }
            if (rover.SamplesProcessed > 0)
                yield return RoverAction.Transmit;
            yield break;
        }

        private void UpdateMap(IRoverStatusAccessor rover) => _map.UpdateTerrain(rover.Position, rover.Adjacent);

        private Direction? FindAdjacentSampleable(AdjacentTerrain adjacent)
        {
            for (Int32 i = Direction.DirectionCount - 1; i >= 0; i--)
            {
                Direction direction = Direction.FromInt32(i);
                if (adjacent[direction].IsSampleable())
                    return direction;
            }

            return null;
        }

        private Direction? FindAdjacentSmooth(AdjacentTerrain adjacent)
        {
            for (Int32 i = Direction.DirectionCount - 1; i >= 0; i--)
            {
                Direction direction = Direction.FromInt32(i);
                if (adjacent[direction] == TerrainType.Smooth)
                    return direction;
            }

            return null;
        }
    }
}
