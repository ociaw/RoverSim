using System;
using System.Collections.Generic;

namespace RoverSim.Ais
{
    /// <summary>
    /// A tweaked version of the Scratch Mark I AI. Maintains a very small and fixed amount of memory.
    /// </summary>
    public sealed class PathfindingAi : IAi
    {
        private Int32 _roundRobin = 0;

        private Boolean _gatheringPower = true;

        private Direction _gatherPowerDir = Direction.None;

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
            while (true)
            {
                var adjacent = rover.Adjacent;
                TerrainType occupied = adjacent[Direction.None];
                (Direction? adjacentSmoothDir, Direction? adjacentRoughDir, Int32 smoothCount) = FindAdjacentUnsampled(adjacent);

                if (rover.MovesLeft <= 5)
                {
                    foreach (var action in DoLowMoves(rover, adjacent))
                        yield return action;
                    yield break;
                }

                if (rover.Power < LowPowerThreshold || (!adjacentSmoothDir.HasValue && occupied == TerrainType.Smooth))
                {
                    if (rover.Power < CalculateExcessPowerThershold(rover) / 2 && rover.NoBacktrack > 3)
                        yield return RoverAction.CollectPower;
                    if (rover.Power < LowPowerThreshold)
                        yield return RoverAction.Transmit;
                }
                else if (_gatheringPower && HasExcessPower(rover, rover.CollectablePower))
                {
                    yield return RoverAction.CollectPower;
                    _gatheringPower = false;
                }

                // While we're gather power, we don't collect samples and instead abuse the Backtracking mechanic gather a large amount of power.
                if (occupied.IsSampleable() && (!_gatheringPower || (adjacentSmoothDir == null && occupied == TerrainType.Smooth) || _gatherPowerDir != Direction.None))
                {
                    yield return RoverAction.CollectSample;
                    _map.UpdateTerrain(rover.Position, rover.Adjacent);
                    if (rover.SamplesCollected >= Parameters.SamplesPerProcess && rover.Power > Parameters.ProcessCost + Parameters.MoveSmoothCost)
                        yield return RoverAction.ProcessSamples;
                }

                Boolean hasExcessPower = HasExcessPower(rover);
                if (hasExcessPower)
                    _gatheringPower = false;

                (Boolean isDeadEnd, Direction deadEndEscape) = CheckDeadEnd(adjacent);

                Direction destination = Direction.None;
                if (_gatherPowerDir != Direction.None)
                    destination = _gatherPowerDir;
                else if (_path != null && _path.Count > 0)
                    destination = _path.Pop();
                else if (adjacentSmoothDir.HasValue)
                    destination = adjacentSmoothDir.Value; // Prioritize smooth squares
                else if (hasExcessPower && !_gatheringPower && adjacentRoughDir.HasValue)
                    destination = adjacentRoughDir.Value; // Visit rough squares if the rover has enough power
                else if (isDeadEnd)
                    destination = ResetDestination(rover.Position) ?? deadEndEscape;

                Direction nextMove = destination;
                if (nextMove == Direction.None || adjacent[nextMove] == TerrainType.Impassable)
                {
                    Direction? reset = ResetDestination(rover.Position);
                    if (reset == null)
                        yield break; // We're stuck.

                    destination = reset.Value;
                    nextMove = destination;
                }

                if (_gatheringPower && rover.Adjacent.Occupied == TerrainType.Smooth && smoothCount > 1)
                    _gatherPowerDir = nextMove.Opposite();
                else
                    _gatherPowerDir = Direction.None;

                yield return new RoverAction(nextMove);
                _map.UpdateTerrain(rover.Position, rover.Adjacent);
                _roundRobin++;
            }
        }

        private Direction? ResetDestination(Position roverPosition)
        {
            if (_gatheringPower)
                _path = _pathfinder.GetPowerPath(roverPosition);
            else
                _path = _pathfinder.GetPathToNearestSampleable(roverPosition);

            if (_path == null)
                return null;

            return _path.Pop();
        }

        private (Boolean isDeadEnd, Direction escapeDir) CheckDeadEnd(AdjacentTerrain adjacent)
        {
            Direction direction = Direction.None;
            Int32 impassableCount = 0;
            for (Int32 i = 0; i < Direction.DirectionCount; i++)
            {
                Direction dir = (Direction)i;
                if (adjacent[dir] == TerrainType.Impassable)
                    impassableCount++;
                else
                    direction = dir;
            }
            return (impassableCount >= 3, direction);
        }

        private Boolean HasExcessPower(IRoverStatusAccessor rover) => HasExcessPower(rover, 0);

        private Boolean HasExcessPower(IRoverStatusAccessor rover, Int32 potentialPower)
        {
            Int32 targetPower = CalculateExcessPowerThershold(rover);
            return rover.Power + potentialPower >= targetPower;
        }

        private Int32 CalculateExcessPowerThershold(IRoverStatusAccessor rover)
        {
            const Int32 smoothRoughRatio = 10; // For the default generator, the ratio is 2:1
            Double meanUnsampledMoveCost = (smoothRoughRatio * Parameters.MoveSmoothCost + Parameters.MoveRoughCost) / (smoothRoughRatio + 1.0);
            Double processCostPerSample = (Double)Parameters.ProcessCost / Parameters.SamplesPerProcess;
            Double meanSampleCost = meanUnsampledMoveCost + Parameters.SampleCost + processCostPerSample;

            Double sampleToLogisticMoveRatio = 3.9; // For 2 every move/sample/process sequences, we estimate 1 moves will be purely logistics (move, power).
            Double meanCost = (meanSampleCost * sampleToLogisticMoveRatio + Parameters.MoveSmoothCost) / (sampleToLogisticMoveRatio + 1);

            Int32 targetPower = (Int32)((rover.MovesLeft - 1) * meanCost) + Parameters.TransmitCost;
            return targetPower;
        }

        private IEnumerable<RoverAction> DoLowMoves(IRoverStatusAccessor rover, AdjacentTerrain adjacent)
        {
            if (rover.MovesLeft > 5)
                yield break;

            Boolean smoothOccupied = adjacent[Direction.None] == TerrainType.Smooth;
            Boolean roughOccupied = adjacent[Direction.None] == TerrainType.Rough;
            (Direction? smoothDir, Direction? roughDir, _) = FindAdjacentUnsampled(adjacent);
            if (rover.MovesLeft == 5 && rover.Power > Parameters.SampleCost + Parameters.MoveRoughCost + Parameters.SampleCost + Parameters.ProcessCost)
            {
                if (smoothOccupied || roughOccupied)
                    yield return RoverAction.CollectSample;
                _map.UpdateTerrain(rover.Position, rover.Adjacent);
                Direction? moveDir = smoothDir ?? roughDir;
                if (moveDir.HasValue)
                {
                    yield return new RoverAction(moveDir.Value);
                    yield return RoverAction.CollectSample;
                    _map.UpdateTerrain(rover.Position, rover.Adjacent);
                }

                yield return RoverAction.ProcessSamples;
                yield return RoverAction.Transmit;
                yield break;
            }

            if (rover.MovesLeft >= 4)
            {
                if (rover.Power < Parameters.ProcessCost + Parameters.SampleCost + Parameters.ProcessCost)
                    yield return RoverAction.CollectPower;
                yield return RoverAction.CollectPower;
                if (rover.Power > Parameters.ProcessCost)
                {
                    if (rover.Power > Parameters.ProcessCost + Parameters.SampleCost && smoothOccupied)
                        yield return RoverAction.CollectSample;
                    _map.UpdateTerrain(rover.Position, rover.Adjacent);
                    yield return RoverAction.ProcessSamples;
                }
            }
            else if (rover.MovesLeft == 3)
            {
                if (rover.Power > Parameters.ProcessCost + Parameters.SampleCost && smoothOccupied)
                    yield return RoverAction.CollectSample;
                else
                    yield return RoverAction.CollectPower;
                _map.UpdateTerrain(rover.Position, rover.Adjacent);
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

        private (Direction? smoothDir, Direction? roughDir, Int32 smoothCount) FindAdjacentUnsampled(AdjacentTerrain adjacent)
        {
            Direction? adjacentSmooth = null;
            Direction? adjacentRough = null;
            Int32 smoothCount = 0;
            for (Int32 i = 0; i < Direction.DirectionCount; i++)
            {
                Direction roundRobin = Direction.FromInt32((i + _roundRobin) % Direction.DirectionCount);
                switch (adjacent[roundRobin])
                {
                    case TerrainType.Smooth:
                        adjacentSmooth = roundRobin;
                        smoothCount++;
                        break;
                    case TerrainType.Rough:
                        adjacentRough = roundRobin;
                        break;
                }
            }

            return (adjacentSmooth, adjacentRough, smoothCount);
        }
    }
}
