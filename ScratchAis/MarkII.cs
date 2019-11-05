using System;
using System.Collections.Generic;

namespace RoverSim.ScratchAis
{
    public sealed class MarkII : IScratchAi
    {
        private const Int32 Width = 32;
        private const Int32 Height = 23;

        private readonly List<TerrainType> _mappedTerrain = new List<TerrainType>(Width * Height);
        private readonly List<TerrainType> _adjacentSquares = new List<TerrainType>(5);
        private readonly Int32[] _potentialPower = new Int32[4];
        private Boolean _lowPower = false;

        private Int32 _posX = 16;
        private Int32 _posY = 11;

        Double roughTerrainDistMultiplier = 1.0;

        private Boolean _mapDirty = true;
        private List<Direction> path = new List<Direction>();

        private Direction _moveDir;

        public MarkII()
        {
            for (Int32 i = 0; i < Width * Height; i++)
            {
                _mappedTerrain.Add(TerrainType.Unknown);
            }
        }

        public IScratchAi CloneFresh() => new MarkII();

        public IEnumerable<RoverAction> Simulate(ScratchRover rover)
        {
            while (true)
            {
                // Sense nearby and find a path and anything else free
                SenseAdjacentSquares(rover);
                if (_mapDirty == true || path.Count == 0)
                {
                    do
                    {
                        roughTerrainDistMultiplier = 70.0 * rover.MovesLeft / rover.Power; // Optimization target
                        if (roughTerrainDistMultiplier > 3)
                            roughTerrainDistMultiplier = 3; // Optimization target
                        else if (roughTerrainDistMultiplier < 1)
                            roughTerrainDistMultiplier = 1;

                        ScratchDijkstras pathFinder = new ScratchDijkstras(_mappedTerrain);
                        path = pathFinder.BeginSolve(_posX, _posY, roughTerrainDistMultiplier);
                        if (path.Count < 1)
                        {
                            if (pathFinder.destinationIndex == -1)
                                yield break; // We have no more squares to go to.
                            // Found an unreachable square
                            _mappedTerrain[pathFinder.destinationIndex] = TerrainType.Impassable;
                        }
                    } while (path.Count == 0);
                }
                _moveDir = path[0];
                path.RemoveAt(0);

                // Collect samples and power, move, process, and transmit - anything needing power or moves
                if (rover.MovesLeft < 5)
                {
                    if (rover.MovesLeft == 4)
                    {
                        yield return RoverAction.CollectPower;
                        if (rover.Power > 40 && _adjacentSquares[4] == TerrainType.Smooth)
                        {
                            yield return RoverAction.CollectSample;;
                            yield return RoverAction.ProcessSamples;
                        }
                        else if (rover.Power > 30)
                        {
                            yield return RoverAction.ProcessSamples;
                        }
                    }
                    else if (rover.MovesLeft == 3)
                    {
                        if (rover.Power > 40 && _adjacentSquares[4] == TerrainType.Smooth)
                        {
                            yield return RoverAction.CollectSample;
                        }
                        else
                        {
                            yield return RoverAction.CollectPower;
                        }
                        if (rover.Power > 30)
                        {
                            yield return RoverAction.ProcessSamples;
                        }
                    }
                    else if (rover.MovesLeft == 2)
                    {
                        if (rover.Power > 30)
                        {
                            yield return RoverAction.ProcessSamples;
                        }
                    }
                    yield return RoverAction.Transmit;
                    yield break; // Out of moves, end simulation
                }

                if (rover.Power < 101)
                {
                    if (rover.Power < 11)
                    {
                        if (_potentialPower[0] + rover.Power < 11)
                        {
                            yield return RoverAction.Transmit;
                        }
                        else
                        {
                            yield return RoverAction.CollectPower;
                        }
                    }

                    if (rover.Power < 101)
                    {
                        _lowPower = true;
                    }


                    if (rover.Power < 51)
                    {
                        if (rover.Power > 10 && _adjacentSquares[4] == TerrainType.Smooth)
                        {
                            yield return RoverAction.CollectPower;
                        }
                        if (rover.Power < 51)
                        {
                            yield return RoverAction.Transmit;
                        }
                    }
                }

                if ((rover.Power + _potentialPower[1]) / rover.MovesLeft > 70)
                {
                    _lowPower = false;
                }

                if (_potentialPower[0] > 0)
                {
                    if (rover.Power < 30 || _potentialPower[1] == 0)
                    {
                        if (rover.Power / rover.MovesLeft < 51)
                        {
                            yield return RoverAction.CollectPower;
                        }
                    }
                }

                if (_adjacentSquares[4] == TerrainType.Smooth || _adjacentSquares[4] == TerrainType.Rough)
                {
                    if (!(_lowPower && _potentialPower[1] > 0))
                    {
                        yield return RoverAction.CollectSample;
                        if (rover.SamplesCollected >= 3 && rover.Power > 40)
                        {
                            yield return RoverAction.ProcessSamples;
                        }
                    }
                }

                yield return Move();

                if (rover.MovesLeft == 0 || rover.Power == 0)
                    yield break;
            }
        }

        private RoverAction Move()
        {
            if (_moveDir == Direction.Up)
            {
                if (_adjacentSquares[0] != TerrainType.Impassable)
                    _posY -= 1; // Reversed for Scratch
                else
                    throw new InvalidOperationException();
            }
            else if (_moveDir == Direction.Right)
            {
                if (_adjacentSquares[1] != TerrainType.Impassable)
                    _posX += 1;
                else
                    throw new InvalidOperationException();
            }
            else if (_moveDir == Direction.Down)
            {
                if (_adjacentSquares[2] != TerrainType.Impassable)
                    _posY += 1; // Reversed for scratch
                else
                    throw new InvalidOperationException();
            }
            else if (_moveDir == Direction.Left)
            {
                if (_adjacentSquares[3] != TerrainType.Impassable)
                    _posX -= 1;
                else
                    throw new InvalidOperationException();
            }
            return new RoverAction(_moveDir);
        }

        private void SenseAdjacentSquares(ScratchRover rover)
        {
            _mapDirty = false;
            _adjacentSquares.Clear();
            rover.SenseSquare(Direction.Up);
            if (_mappedTerrain[(_posY - 1) * Width + _posX] != rover.Sense) // Reversed for Scratch
                _mapDirty = true;
            _mappedTerrain[(_posY - 1) * Width + _posX] = rover.Sense; // Reversed for Scratch
            _adjacentSquares.Add(rover.Sense);

            rover.SenseSquare(Direction.Right);
            if (_mappedTerrain[_posY * Width + _posX + 1] != rover.Sense)
                _mapDirty = true;
            _mappedTerrain[_posY * Width + _posX + 1] = rover.Sense;
            _adjacentSquares.Add(rover.Sense);

            rover.SenseSquare(Direction.Down);
            if (_mappedTerrain[(_posY + 1) * Width + _posX] != rover.Sense) // Reversed for Scratch
                _mapDirty = true;
            _mappedTerrain[(_posY + 1) * Width + _posX] = rover.Sense; // Reversed for Scratch
            _adjacentSquares.Add(rover.Sense);

            rover.SenseSquare(Direction.Left);
            if (_mappedTerrain[_posY * Width + _posX - 1] != rover.Sense)
                _mapDirty = true;
            _mappedTerrain[_posY * Width + _posX - 1] = rover.Sense;
            _adjacentSquares.Add(rover.Sense);

            rover.SenseSquare(Direction.None);
            _mappedTerrain[_posY * Width + _posX] = rover.Sense;
            _adjacentSquares.Add(rover.Sense);

            _potentialPower[0] = 0;
            _potentialPower[1] = 0;

            if (_adjacentSquares[4] == TerrainType.Smooth)
            {
                _potentialPower[0] = rover.NoBacktrack * rover.NoBacktrack * rover.NoBacktrack;
            }

            for (Int32 i = 0; i < 4; i++)
            {
                if (_adjacentSquares[i] == TerrainType.Smooth)
                {
                    _potentialPower[1] = (rover.NoBacktrack + 1) * (rover.NoBacktrack + 1) * (rover.NoBacktrack + 1);
                    break;
                }
            }
        }
    }
}
