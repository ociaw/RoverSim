using System;
using System.Collections.Generic;

namespace RoverSim.ScratchAis
{
    public sealed class MarkI : IScratchAi
    {
        private Direction destination = Direction.None;
        private Int32 PosX = 0;
        private Int32 PosY = 0;
        private Direction moveDir = Direction.None;

        private readonly List<Int32> PreviousX = new List<Int32>(5);
        private readonly List<Int32> PreviousY = new List<Int32>(5);

        private Int32 PosXNext = 0;
        private Int32 PosYNext = 0;

        private readonly List<TerrainType> adjacentSquares = new List<TerrainType>(5);

        public MarkI(Int32 identifier)
        {
            Identifier = identifier;
        }

        public Int32 Identifier { get; }

        public IEnumerable<RoverAction> Simulate(ScratchRover rover)
        {
            while (true)
            {
                Direction SmoothSquare = Direction.None;
                SenseAdjacentSquares(rover);
                for (Int32 i = 0; i < 5; i++)
                {
                    if (adjacentSquares[i] == TerrainType.Smooth)
                    {
                        SmoothSquare = (Direction)i;
                        break;
                    }
                }

                if (rover.Power < 30 || (SmoothSquare == Direction.None && adjacentSquares[4] == TerrainType.Smooth))
                {
                    if (rover.Power / rover.MovesLeft < 51)
                    {
                        yield return RoverAction.CollectPower;
                    }
                }

                if (rover.MovesLeft < 3)
                {
                    if (rover.MovesLeft == 2)
                    {
                        yield return RoverAction.ProcessSamples;
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
                if (SmoothSquare != Direction.None)
                {
                    moveDir = SmoothSquare;
                }
                else
                {
                    moveDir = Direction.None;
                    for (Int32 i = 0; i < 5; i++)
                    {
                        if (adjacentSquares[i] == TerrainType.Rough)
                        {
                            moveDir = (Direction)i;
                            destination = Direction.None;
                            break;
                        }
                    }
                    if (moveDir == Direction.None)
                    {
                        if (destination == Direction.None)
                        {
                            SetDestination();
                        }
                        if (adjacentSquares[(Int32)destination] != TerrainType.Impassable)
                        {
                            moveDir = destination;
                        }
                        else
                        {
                            FindOpenSquare();
                        }
                    }
                }
                CheckStuck();
                yield return Move();

                if (rover.MovesLeft == 0 || rover.Power == 0)
                    yield break;
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

        private void SetDestination()
        {
            if (Math.Abs(PosX) > Math.Abs(PosY))
            {
                if (PosX < 0)
                {
                    destination = Direction.Right;
                }
                else
                {
                    destination = Direction.Left;
                }
            }
            else
            {
                // Reverse Up and Down in Scratch
                if (PosY < 0)
                {
                    destination = Direction.Down;
                }
                else
                {
                    destination = Direction.Up;
                }
            }
        }

        private void SimNextMove()
        {
            PosXNext = PosX;
            PosYNext = PosY;

            if (moveDir == Direction.Up)
                PosYNext -= 1; // Reversed for Scratch
            else if (moveDir == Direction.Right)
                PosXNext += 1;
            else if (moveDir == Direction.Down)
                PosYNext += 1; // Reversed for scratch
            else
                PosXNext -= 1;
        }

        private RoverAction Move()
        {
            if (PreviousX.Count > 4)
                PreviousX.RemoveAt(PreviousX.Count - 1);
            PreviousX.Insert(0, PosX);
            if (PreviousY.Count > 4)
                PreviousY.RemoveAt(PreviousY.Count - 1);
            PreviousY.Insert(0, PosY);
            if (moveDir == Direction.Up)
                PosY -= 1; // Reversed for Scratch
            else if (moveDir == Direction.Right)
                PosX += 1;
            else if (moveDir == Direction.Down)
                PosY += 1; // Reversed for scratch
            else
                PosX -= 1;
            return new RoverAction(moveDir);
        }

        private void FindOpenSquare()
        {
            if (destination == Direction.Right || destination == Direction.Left)
            {
                if (adjacentSquares[0] != TerrainType.Impassable)
                {
                    moveDir = Direction.Up;
                }
                else if (adjacentSquares[2] != TerrainType.Impassable)
                {
                    moveDir = Direction.Down;
                }
                else
                {
                    if (destination == Direction.Right)
                    {
                        moveDir = Direction.Left;
                    }
                    else
                        moveDir = Direction.Right;
                }
            }
            if (destination == Direction.Up || destination == Direction.Down)
            {
                if (adjacentSquares[1] != TerrainType.Impassable)
                {
                    moveDir = Direction.Right;
                }
                else if (adjacentSquares[3] != TerrainType.Impassable)
                {
                    moveDir = Direction.Left;
                }
                else
                {
                    if (destination == Direction.Up)
                    {
                        moveDir = Direction.Down;
                    }
                    else
                        moveDir = Direction.Up;
                }
            }
        }

        private void CheckStuck()
        {
            if (PreviousY.Count > 4 && PreviousX.Count > 4)
            {
                if (PreviousX[1] == PosX && PreviousY[1] == PosY && PreviousX[3] == PosX && PreviousX[3] == PosY)
                {
                    SimNextMove();
                    if (PosXNext == PreviousX[0] && PosYNext == PreviousY[0])
                    {
                        SetDestination();
                    }
                }
            }
        }
    }
}
