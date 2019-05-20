using System;
using System.Collections.Generic;

namespace RoverSim.Ais
{
    /// <summary>
    /// A tweaked version of the Scratch Mark I AI. Maintains a very small amount of memory.
    /// </summary>
    public sealed class MinimalStateAi : IAi
    {
        private const Int32 DirectionCount = 4;

        private Int32 _roundRobin = 0;

        private Direction _destination = Direction.None;

        private readonly HashSet<(Int32 x, Int32 y)> _deadEnds = new HashSet<(Int32 x, Int32 y)>();

        public MinimalStateAi(Int32 identifier, SimulationParameters parameters)
        {
            Identifier = identifier;
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public Int32 Identifier { get; }

        public SimulationParameters Parameters { get; }

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

                if (rover.Power < 30 || (!adjacentSmoothDir.HasValue && rover.SenseSquare(Direction.None) == TerrainType.Smooth))
                {
                    if (!HasExcessPower(rover))
                        rover.CollectPower();
                }

                if (occupied.IsSampleable())
                {
                    rover.CollectSample();
                    if (rover.SamplesCollected >= Parameters.HopperSize)
                        rover.ProcessSamples();
                }

                Boolean hasExcessPower = HasExcessPower(rover);
                (Boolean isDeadEnd, Direction deadEndEscape) = CheckDeadEnd(adjacent);

                Direction nextMove;
                if (adjacentSmoothDir.HasValue)
                    nextMove = adjacentSmoothDir.Value; //Prioritize smooth squares
                else if (hasExcessPower && adjacentRoughDir.HasValue)
                    nextMove = adjacentRoughDir.Value; // Visit rough squares if the rover has enough power
                else if (isDeadEnd)
                {
                    _deadEnds.Add((rover.PosX, rover.PosY));
                    nextMove = deadEndEscape;
                }
                else
                {
                    Direction destination = _destination;
                    while (destination == Direction.None || adjacent[(Int32)destination] == TerrainType.Impassable || _deadEnds.Contains(destination.NextCoords(rover)))
                        destination = DetermineNextMove(rover, adjacent); // Otherwise, we have to do some more complex logic

                    _destination = destination;
                    nextMove = _destination; // Move towards the destination
                }
                rover.Move(nextMove);
                _roundRobin++;
            }
        }

        private Direction DetermineNextMove(IRover rover, TerrainType[] adjacent)
        {
            _roundRobin++;
            return (Direction)(_roundRobin % DirectionCount);
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

        private Boolean HasExcessPower(IRover rover) => rover.Power >= (Parameters.MoveRoughCost + 1) * rover.MovesLeft;

        private void DoLowMoves(IRover rover)
        {
            if (rover.MovesLeft > 5)
                return;
            Boolean smoothOccupied = rover.SenseSquare(Direction.None) == TerrainType.Smooth;
            Boolean roughOccupied = rover.SenseSquare(Direction.None) == TerrainType.Rough;
            (Direction? smoothDir, Direction? roughDir) = FindAdjacentUnsampled(rover);
            if (rover.MovesLeft == 5) // And lots of power
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

            if (rover.MovesLeft == 4)
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
            TerrainType[] terrain = new TerrainType[DirectionCount];

            for (Int32 i = 0; i < terrain.Length; i++)
                terrain[i] = rover.SenseSquare((Direction)i);

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
                Int32 roundRobin = (i + _roundRobin) % DirectionCount;
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
    }
}
