﻿using System;
using System.Collections.Generic;
using RandN.Distributions;
using RandN.Rngs;

namespace RoverSim
{
    public sealed class MazeGenerator : ILevelGenerator
    {
        private static readonly Uniform.Int32 _directionDist = Uniform.New(0, Direction.DirectionCount);
        private static readonly Bernoulli _wallClearChance = Bernoulli.FromRatio(1, 5); // 20% chance of clearing a wall

        public Level Generate(SimulationParameters parameters, Int32 rngSeed)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Pcg32 rng = Pcg32.Create((UInt64)rngSeed, 0);

            Int32 width = parameters.BottomRight.X + 1;
            Int32 height = parameters.BottomRight.Y + 1;

            TerrainType[,] terrain = new TerrainType[width, height];

            Position start = parameters.InitialPosition;
            Stack<CoordinatePair> stack = new Stack<CoordinatePair>();
            HashSet<CoordinatePair> visited = new HashSet<CoordinatePair>();
            stack.Push(start);
            visited.Add(start);
            terrain[start.X, start.Y] = TerrainType.Smooth;

            while (stack.Count > 0)
            {
                var current = stack.Peek();

                Int32 offset = _directionDist.Sample(rng);
                Boolean anyNewNeighbors = false;
                for (Int32 i = 0; i < Direction.DirectionCount; i++)
                {
                    Direction direction = (Direction)((i + offset) % Direction.DirectionCount);

                    var passage = current + direction;
                    var neighbor = passage + direction;
                    var boundaryCheck = neighbor + direction;
                    if (!parameters.BottomRight.Contains(boundaryCheck) || visited.Contains(neighbor))
                        continue;

                    anyNewNeighbors = true;
                    terrain[passage.X, passage.Y] = TerrainType.Smooth;
                    terrain[neighbor.X, neighbor.Y] = TerrainType.Smooth;
                    visited.Add(neighbor);
                    stack.Push(neighbor);
                }

                if (!anyNewNeighbors)
                {
                    if (_wallClearChance.Sample(rng))
                    {
                        Direction direction = (Direction)_directionDist.Sample(rng);
                        var passage = current + direction;
                        var boundaryCheck = passage + direction;
                        if (parameters.BottomRight.Contains(boundaryCheck) && terrain[passage.X, passage.Y] == TerrainType.Impassable)
                            terrain[passage.X, passage.Y] = TerrainType.Rough;
                    }

                    stack.Pop();
                }
            }

            return new Level(terrain, new ProtoLevel(parameters, this, rngSeed));
        }
    }
}
