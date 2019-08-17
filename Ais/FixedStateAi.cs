﻿using System;
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

        private readonly Queue<CoordinatePair> _deadEnds;

        public FixedStateAi(Int32 identifier, SimulationParameters parameters, Int32 deadEndMemory)
        {
            Identifier = identifier;
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            DeadEndMemory = deadEndMemory >= 0 ? deadEndMemory : throw new ArgumentOutOfRangeException(nameof(deadEndMemory), deadEndMemory, "Must be non-negative.");
            _deadEnds = new Queue<CoordinatePair>(deadEndMemory);
        }

        public Int32 Identifier { get; }

        public SimulationParameters Parameters { get; }

        public Int32 LowPowerThreshold => Parameters.MoveSmoothCost * 2 + Parameters.SampleCost;

        public Int32 DeadEndMemory { get; }

        public void Simulate(IRover rover)
        {
            while (true)
            {
                var adjacent = SenseAdjacent(rover);
                TerrainType occupied = rover.SenseSquare(Direction.None);
                (Direction? adjacentSmoothDir, Direction? adjacentRoughDir) = FindAdjacentUnsampled(adjacent);

                if (rover.MovesLeft <= 5)
                {
                    DoLowMoves(rover);
                    return;
                }

                if (rover.Power < LowPowerThreshold || (!adjacentSmoothDir.HasValue && occupied == TerrainType.Smooth))
                {
                    if (!HasExcessPower(rover))
                        rover.CollectPower();
                    if (rover.Power < LowPowerThreshold)
                        rover.Transmit();
                }

                // While we're gather power, we don't collect samples and instead abuse the Backtracking mechanic gather a large amount of power.
                if (occupied.IsSampleable() && (!_gatheringPower || (adjacentSmoothDir == null && occupied == TerrainType.Smooth)))
                {
                    rover.CollectSample();
                    if (rover.SamplesCollected >= Parameters.SamplesPerProcess && rover.Power > Parameters.ProcessCost + Parameters.MoveSmoothCost)
                        rover.ProcessSamples();
                }

                Boolean hasExcessPower = HasExcessPower(rover);
                if (hasExcessPower)
                    _gatheringPower = false;

                (Boolean isDeadEnd, Direction deadEndEscape) = CheckDeadEnd(adjacent);
                if (isDeadEnd)
                    AddDeadEnd(rover.Position);

                if (adjacentSmoothDir.HasValue)
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

                rover.Move(nextMove);
                _roundRobin++;
            }
        }

        private Direction AvoidObstacle(TerrainType[] adjacent)
        {
            // We use round robin here to avoid always checking the same side first,
            // as that causes the rover to favor one area.
            Direction turn = _roundRobin % 2 == 0 ? _destination.RotateCW() : _destination.RotateCCW();

            if (adjacent[turn] != TerrainType.Impassable)
                return turn;
            if (adjacent[turn.Opposite()] != TerrainType.Impassable)
                return turn.Opposite();

            return _destination.Opposite();
        }

        private Direction ResetDestination(TerrainType[] adjacent)
        {
            for (Int32 i = _roundRobin; i < _roundRobin + Direction.DirectionCount; i++)
            {
                Int32 dir = i % Direction.DirectionCount;
                if (adjacent[dir] != TerrainType.Impassable)
                    return (Direction)dir;
            }

            return Direction.None;
        }

        private (Boolean isDeadEnd, Direction escapeDir) CheckDeadEnd(TerrainType[] adjacent)
        {
            Direction direction = Direction.None;
            Int32 impassableCount = 0;
            for (Int32 i = 0; i < adjacent.Length; i++)
            {
                if (adjacent[i] == TerrainType.Impassable)
                    impassableCount++;
                else
                    direction = (Direction)i;
            }
            return (impassableCount >= 3, direction);
        }
        
        private Boolean HasExcessPower(IRover rover) => rover.Power >= (7) * rover.MovesLeft + 1;

        private void DoLowMoves(IRover rover)
        {
            if (rover.MovesLeft > 5)
                return;

            Boolean smoothOccupied = rover.SenseSquare(Direction.None) == TerrainType.Smooth;
            Boolean roughOccupied = rover.SenseSquare(Direction.None) == TerrainType.Rough;
            (Direction? smoothDir, Direction? roughDir) = FindAdjacentUnsampled(rover);
            if (rover.MovesLeft == 5 && rover.Power > Parameters.SampleCost + Parameters.MoveRoughCost + Parameters.SampleCost + Parameters.ProcessCost)
            {
                if (smoothOccupied || roughOccupied)
                    rover.CollectSample();
                Direction? moveDir = smoothDir ?? roughDir;
                if (moveDir.HasValue)
                {
                    rover.Move(moveDir.Value);
                    rover.CollectSample();
                }

                rover.ProcessSamples();
                rover.Transmit();
                return;
            }

            if (rover.MovesLeft >= 4)
            {
                rover.CollectPower();
                if (rover.Power > Parameters.ProcessCost)
                {
                    if (rover.Power > Parameters.ProcessCost + Parameters.SampleCost && smoothOccupied)
                        rover.CollectSample();
                    rover.ProcessSamples();
                }
            }
            else if (rover.MovesLeft == 3)
            {
                if (rover.Power > Parameters.ProcessCost + Parameters.SampleCost && smoothOccupied)
                    rover.CollectSample();
                else
                    rover.CollectPower();
                if (rover.Power > Parameters.ProcessCost)
                    rover.ProcessSamples();
            }
            if (rover.MovesLeft == 2)
            {
                if (rover.Power == 0)
                    rover.CollectPower();
                else if (rover.Power > Parameters.ProcessCost && rover.SamplesCollected > 0)
                    rover.ProcessSamples();
            }
            if (rover.SamplesProcessed > 0)
                rover.Transmit();
            return;
        }

        private TerrainType[] SenseAdjacent(IRover rover)
        {
            TerrainType[] terrain = new TerrainType[Direction.DirectionCount];

            for (Int32 i = 0; i < terrain.Length; i++)
            {
                Direction direction = (Direction)i;
                TerrainType tile = rover.SenseSquare(direction);

                // For simplicity, we'll just ensure dead ends are always considered to be impassable.
                if (IsDeadEnd(rover, direction))
                    tile = TerrainType.Impassable;

                terrain[i] = tile;
            }

            return terrain;
        }

        private (Direction? smoothDir, Direction? roughDir) FindAdjacentUnsampled(IRover rover)
        {
            var adjacent = SenseAdjacent(rover);
            return FindAdjacentUnsampled(adjacent);
        }

        private (Direction? smoothDir, Direction? roughDir) FindAdjacentUnsampled(TerrainType[] adjacent)
        {
            Direction? adjacentSmooth = null;
            Direction? adjacentRough = null;
            for (Int32 i = 0; i < adjacent.Length; i++)
            {
                Int32 roundRobin = (i + _roundRobin) % Direction.DirectionCount;
                switch (adjacent[roundRobin])
                {
                    case TerrainType.Smooth:
                        adjacentSmooth = (Direction)roundRobin;
                        break;
                    case TerrainType.Rough:
                        adjacentRough = (Direction)roundRobin;
                        break;
                }
            }

            return (adjacentSmooth, adjacentRough);
        }

        private Boolean IsDeadEnd(IRover rover, Direction direction) => _deadEnds.Contains(rover.Position + direction);

        private void AddDeadEnd(Position position)
        {
            if (_deadEnds.Count == DeadEndMemory)
                _deadEnds.Dequeue();

            _deadEnds.Enqueue(position);
        }
    }
}