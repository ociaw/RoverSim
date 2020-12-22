using System;
using System.Collections.Generic;
using RandN.Distributions;
using RandN.Rngs;

namespace RoverSim.ScratchAis
{
    /// <summary>
    /// A stateless AI that tries to stay on smooth tiles. Moves in a random direction if no smooth tiles are nearby.
    /// </summary>
    public class IntelligentRandomAi : IScratchAi
    {
        private static readonly Uniform.Int32 _directionDist = Uniform.New(0, Direction.DirectionCount);

        private readonly Int32 _seed;

        private readonly Pcg32 _rng;

        private readonly List<TerrainType> adjacentSquares = new List<TerrainType>(5);

        public IntelligentRandomAi(Int32 seed)
        {
            _seed = seed;
            _rng = Pcg32.Create((UInt64)seed, 0);
        }

        public IScratchAi CloneFresh() => new IntelligentRandomAi(_seed);

        public IEnumerable<RoverAction> Simulate(ScratchRover rover)
        {
            while (true)
            {
                Direction smoothSquare = Direction.None;
                SenseAdjacentSquares(rover);
                for (Int32 i = 0; i < Direction.DirectionCount; i++)
                {
                    if (adjacentSquares[i] == TerrainType.Smooth)
                    {
                        smoothSquare = (Direction)i;
                        break;
                    }
                }

                if (rover.Power < 30 || (smoothSquare == Direction.None && adjacentSquares[4] == TerrainType.Smooth))
                {
                    if (rover.Power < 51 * rover.MovesLeft)
                    {
                        yield return RoverAction.CollectPower;
                    }
                }

                if (rover.MovesLeft < 3)
                {
                    if (rover.MovesLeft == 2)
                    {
                        yield return RoverAction.ProcessSamples;
                        yield return RoverAction.Transmit;
                        yield break;
                    }
                    yield return RoverAction.Transmit;
                }
                if (rover.Power < 41)
                {
                    if (rover.Power > 10)
                    {
                        yield return RoverAction.CollectPower;
                    }
                    if (rover.Power < 41)
                    {
                        yield return RoverAction.Transmit;
                    }
                }
                if (adjacentSquares[4] == TerrainType.Smooth || adjacentSquares[4] == TerrainType.Rough)
                {
                    yield return RoverAction.CollectSample;
                    if (rover.SamplesCollected >= 3)
                    {
                        yield return RoverAction.ProcessSamples;
                    }
                }
                if (adjacentSquares.Contains(TerrainType.Smooth))
                {
                    if (adjacentSquares[0] == TerrainType.Smooth)
                        yield return new RoverAction(Direction.Up);
                    else if (adjacentSquares[1] == TerrainType.Smooth)
                        yield return new RoverAction(Direction.Right);
                    else if (adjacentSquares[2] == TerrainType.Smooth)
                        yield return new RoverAction(Direction.Down);
                    else if (adjacentSquares[3] == TerrainType.Smooth)
                        yield return new RoverAction(Direction.Left);
                }
                else
                {
                    Int32 num = _directionDist.Sample(_rng);
                    if (num == 0)
                        yield return new RoverAction(Direction.Up);
                    else if (num == 1)
                        yield return new RoverAction(Direction.Right);
                    else if (num == 2)
                        yield return new RoverAction(Direction.Down);
                    else if (num == 3)
                        yield return new RoverAction(Direction.Left);
                }
            }
        }

        private void SenseAdjacentSquares(ScratchRover rover)
        {
            adjacentSquares.Clear();
            rover.SenseSquare(Direction.Up);
            adjacentSquares.Add(rover.Sense);
            rover.SenseSquare(Direction.Right);
            adjacentSquares.Add(rover.Sense);
            rover.SenseSquare(Direction.Down);
            adjacentSquares.Add(rover.Sense);
            rover.SenseSquare(Direction.Left);
            adjacentSquares.Add(rover.Sense);
            rover.SenseSquare(Direction.None);
            adjacentSquares.Add(rover.Sense);
        }
    }
}
