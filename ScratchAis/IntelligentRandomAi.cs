using System;
using System.Collections.Generic;

namespace RoverSim.ScratchAis
{
    /// <summary>
    /// A stateless AI that tries to stay on smooth tiles. Moves in a random direction if no smooth tiles are nearby.
    /// </summary>
    public class IntelligentRandomAi : IScratchAi
    {
        private readonly Random _random;

        private readonly List<TerrainType> adjacentSquares = new List<TerrainType>(5);

        public IntelligentRandomAi(Random random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public void Simulate(ScratchRover rover)
        {
            while (true)
            {
                if (Step(rover))
                    break;
            }
        }

        public Boolean Step(ScratchRover rover)
        {
            if (rover == null)
                throw new ArgumentNullException(nameof(rover));

            Direction smoothSquare = Direction.None;
            SenseAdjacentSquares(rover);
            for (Direction i = 0; i <= Direction.None; i++)
            {
                if (adjacentSquares[(Int32)i] == TerrainType.Smooth)
                {
                    smoothSquare = i;
                    break;
                }
            }

            if (rover.Power < 30 || (smoothSquare == Direction.None && adjacentSquares[4] == TerrainType.Smooth))
            {
                if (rover.Power < 51 * rover.MovesLeft)
                {
                    rover.CollectPower();
                }
            }

            if (rover.MovesLeft < 3)
            {
                if (rover.MovesLeft == 2)
                {
                    rover.ProcessSamples();
                    rover.Transmit();
                    return true;
                }
                rover.Transmit();
            }
            if (rover.Power < 41)
            {
                if (rover.Power > 10)
                {
                    rover.CollectPower();
                }
                if (rover.Power < 41)
                {
                    rover.Transmit();
                }
            }
            if (adjacentSquares[4] == TerrainType.Smooth || adjacentSquares[4] == TerrainType.Rough)
            {
                rover.CollectSample();
                if (rover.SamplesCollected >= 3)
                {
                    rover.ProcessSamples();
                }
            }
            if (adjacentSquares.Contains(TerrainType.Smooth))
            {
                if (adjacentSquares[0] == TerrainType.Smooth)
                    rover.Move(Direction.Up);
                else if (adjacentSquares[1] == TerrainType.Smooth)
                    rover.Move(Direction.Right);
                else if (adjacentSquares[2] == TerrainType.Smooth)
                    rover.Move(Direction.Down);
                else if (adjacentSquares[3] == TerrainType.Smooth)
                    rover.Move(Direction.Left);
            }
            else
            {
                Int32 num = _random.Next(0, 4);
                if (num == 0) rover.Move(Direction.Up);
                else if (num == 1) rover.Move(Direction.Right);
                else if (num == 2) rover.Move(Direction.Down);
                else if (num == 3) rover.Move(Direction.Left);
            }

            return false;
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