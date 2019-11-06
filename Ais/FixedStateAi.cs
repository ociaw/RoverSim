using System;
using System.Collections.Generic;

namespace RoverSim.Ais
{
    /// <summary>
    /// A tweaked version of the Scratch Mark I AI. Maintains a very small and fixed amount of memory.
    /// </summary>
    public sealed class FixedStateAi : IAi
    {
        private Int32 _roundRobin = 0;

        private Direction _destination = Direction.None;

        private Direction _avoidanceDestination = Direction.None;

        private Boolean _gatheringPower = true;

        private Direction _gatherPowerDir = Direction.None;

        private readonly Queue<CoordinatePair> _deadEnds;

        public FixedStateAi(SimulationParameters parameters, Int32 deadEndMemory)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            DeadEndMemory = deadEndMemory >= 0 ? deadEndMemory : throw new ArgumentOutOfRangeException(nameof(deadEndMemory), deadEndMemory, "Must be non-negative.");
            _deadEnds = new Queue<CoordinatePair>(deadEndMemory);
        }

        public SimulationParameters Parameters { get; }

        public Int32 LowPowerThreshold => Parameters.MoveSmoothCost * 2 + Parameters.SampleCost;

        public Int32 DeadEndMemory { get; }

        public IAi CloneFresh() => new FixedStateAi(Parameters, DeadEndMemory);

        public IEnumerable<RoverAction> Simulate(IRoverStatusAccessor rover)
        {
            while (true)
            {
                var adjacent = GetAdjacent(rover);
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
                    if (rover.Power < CalculateExcessPowerThershold(rover) / 2)
                        yield return RoverAction.CollectPower;
                    if (rover.Power < LowPowerThreshold)
                        yield return RoverAction.Transmit;
                }
                else if (_gatheringPower && HasExcessPower(rover, rover.NoBacktrack * rover.NoBacktrack * rover.NoBacktrack))
                {
                    yield return RoverAction.CollectPower;
                    _gatheringPower = false;
                }

                // While we're gather power, we don't collect samples and instead abuse the Backtracking mechanic gather a large amount of power.
                if (occupied.IsSampleable() && (!_gatheringPower || (adjacentSmoothDir == null && occupied == TerrainType.Smooth) || _gatherPowerDir != Direction.None))
                {
                    yield return RoverAction.CollectSample;
                    if (rover.SamplesCollected >= Parameters.SamplesPerProcess && rover.Power > Parameters.ProcessCost + Parameters.MoveSmoothCost)
                        yield return RoverAction.ProcessSamples;
                }

                Boolean hasExcessPower = HasExcessPower(rover);
                if (hasExcessPower)
                    _gatheringPower = false;

                (Boolean isDeadEnd, Direction deadEndEscape) = CheckDeadEnd(adjacent);
                if (isDeadEnd && rover.Adjacent.Occupied != TerrainType.Smooth)
                    AddDeadEnd(rover.Position);

                if (_gatherPowerDir != Direction.None)
                    _destination = _gatherPowerDir;
                else if (adjacentSmoothDir.HasValue)
                    _destination = adjacentSmoothDir.Value; // Prioritize smooth squares
                else if (hasExcessPower && !_gatheringPower && adjacentRoughDir.HasValue)
                    _destination = adjacentRoughDir.Value; // Visit rough squares if the rover has enough power
                else if (isDeadEnd)
                    _destination = deadEndEscape;

                Direction nextMove = _destination;
                if (nextMove == Direction.None || adjacent[nextMove] == TerrainType.Impassable)
                {
                    if (_avoidanceDestination != Direction.None)
                    {
                        if (adjacent[_avoidanceDestination] == TerrainType.Impassable)
                        {
                            _destination = ResetDestination(adjacent);
                            nextMove = _destination;
                            _avoidanceDestination = Direction.None;
                        }
                        else
                        {
                            nextMove = _avoidanceDestination;
                        }
                    }
                    else
                    {
                        nextMove = AvoidObstacle(adjacent); // Obstacle avoidance
                        _avoidanceDestination = nextMove;
                    }
                }
                else
                {
                    _avoidanceDestination = Direction.None;
                }

                if (_gatheringPower && rover.Adjacent.Occupied == TerrainType.Smooth && smoothCount > 1)
                    _gatherPowerDir = nextMove.Opposite();
                else
                    _gatherPowerDir = Direction.None;

                yield return new RoverAction(nextMove);
                _roundRobin++;
            }
        }

        private Direction AvoidObstacle(AdjacentTerrain adjacent)
        {
            // We use round robin here to avoid always checking the same side first,
            // as that causes the rover to favor one area.
            Direction turn = _roundRobin % 8 <= 5 ? _destination.RotateCW() : _destination.RotateCCW();

            if (adjacent[turn] != TerrainType.Impassable)
                return turn;
            if (adjacent[turn.Opposite()] != TerrainType.Impassable)
                return turn.Opposite();

            return _destination.Opposite();
        }

        private Direction ResetDestination(AdjacentTerrain adjacent)
        {
            // We do some weird things here to avoid getting stuck in a loop too easily.
            Int32 addend = _roundRobin % 7;
            Boolean shouldAdd = _roundRobin % 3 != 0;
            for (Int32 i = 0; i < Direction.DirectionCount; i++)
            {
                Int32 modified = _roundRobin + addend + (shouldAdd ? i : Direction.DirectionCount - i);
                Direction dir = Direction.FromInt32(modified % Direction.DirectionCount);
                if (adjacent[dir] != TerrainType.Impassable)
                    return dir;
            }

            return Direction.None;
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
                Direction? moveDir = smoothDir ?? roughDir;
                if (moveDir.HasValue)
                {
                    yield return new RoverAction(moveDir.Value);
                    yield return RoverAction.CollectSample;
                }

                yield return RoverAction.ProcessSamples;
                yield return RoverAction.Transmit;
                yield break;
            }

            if (rover.MovesLeft >= 4)
            {
                yield return RoverAction.CollectPower;
                if (rover.Power > Parameters.ProcessCost)
                {
                    if (rover.Power > Parameters.ProcessCost + Parameters.SampleCost && smoothOccupied)
                        yield return RoverAction.CollectSample;
                    yield return RoverAction.ProcessSamples;
                }
            }
            else if (rover.MovesLeft == 3)
            {
                if (rover.Power > Parameters.ProcessCost + Parameters.SampleCost && smoothOccupied)
                    yield return RoverAction.CollectSample;
                else
                    yield return RoverAction.CollectPower;
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

        private AdjacentTerrain GetAdjacent(IRoverStatusAccessor rover)
        {
            var adjacent = rover.Adjacent;
            var position = rover.Position;
            return new AdjacentTerrain
            (
                IsDeadEnd(position, Direction.Up) ? TerrainType.Impassable : adjacent.Up,
                IsDeadEnd(position, Direction.Right) ? TerrainType.Impassable : adjacent.Right,
                IsDeadEnd(position, Direction.Down) ? TerrainType.Impassable : adjacent.Down,
                IsDeadEnd(position, Direction.Left) ? TerrainType.Impassable : adjacent.Left,
                IsDeadEnd(position, Direction.None) ? TerrainType.Impassable : adjacent.Occupied
            );
        }

        private Boolean IsDeadEnd(Position position, Direction direction) => _deadEnds.Contains(position + direction);

        private void AddDeadEnd(Position position)
        {
            if (DeadEndMemory == 0)
                return;

            if (_deadEnds.Count == DeadEndMemory)
                _deadEnds.Dequeue();

            _deadEnds.Enqueue(position);
        }
    }
}
