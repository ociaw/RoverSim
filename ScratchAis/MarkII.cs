using System;
using System.Collections.Generic;

namespace RoverSim.ScratchAis
{
    public sealed class MarkII : IScratchAi
    {
        private const Int32 Width = 32;
        private const Int32 Height = 23;

        private readonly List<TerrainType> _mappedTerrain = new List<TerrainType>(Width * Height);
        private readonly Int32[] _weightedTerrain = new Int32[Width * Height];
        private Int32[] _reducedWeightMap = new Int32[(Width / 3) * (Height / 3)];
        private readonly List<TerrainType> _adjacentSquares = new List<TerrainType>(5);
        private readonly Int32[] _potentialPower = new Int32[4];
        private Boolean _lowPower = false;

        private Int32 _posX = 16;
        private Int32 _posY = 11;

        Int32 destinationX = 0;
        Int32 destinationY = 0;
        Int32 destinationDist = 0;

        Double roughTerrainDistMultiplier = 1.0;

        private Int32 squareWeight = 0;

        private Boolean _mapDirty = true;
        private List<Direction> path = new List<Direction>();

        private Direction _moveDir;
        private Int32 searchAdjusted;

        public MarkII()
        {
            for (Int32 i = 0; i < Width * Height; i++)
            {
                _mappedTerrain.Add(TerrainType.Unknown);
            }
        }

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
                        AnalyzeTerrain(_posX, _posY);

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

        private void AnalyzeTerrain(Int32 x, Int32 y)
        {
            Int32 bestSmoothX = 9999;
            Int32 bestSmoothY = 9999;
            Int32 bestSmoothAdjusted = 19998;
            Int32 bestRoughX = 9999;
            Int32 bestRoughY = 9999;
            Int32 bestRoughAdjusted = 19998;
            Int32 bestUnknownX = 9999;
            Int32 bestUnknownY = 9999;
            Int32 bestUnknownAdjusted = 19998;
            _reducedWeightMap = new Int32[Width / 3 * (Height / 3)];

            for (Int32 i = 0; i < Width * Height; i++)
            {
                WeightSquare(i);
                Int32 searchX = i % Width;
                Int32 searchY = i / Width;
                Int32 searchDistance = Math.Abs(y - searchY) + Math.Abs(x - searchX);
                searchAdjusted = searchDistance - _weightedTerrain[i] / 1;
                if (searchX != _posX || searchY != _posY)
                {
                    if (_mappedTerrain[i] == TerrainType.Smooth)
                    {
                        if (searchAdjusted < bestSmoothAdjusted)
                        {
                            bestSmoothY = searchY;
                            bestSmoothX = searchX;
                            bestSmoothAdjusted = searchAdjusted;
                        }
                    }
                    else if (_mappedTerrain[i] == TerrainType.Rough)
                    {
                        if (searchAdjusted < bestRoughAdjusted)
                        {
                            bestRoughY = searchY;
                            bestRoughX = searchX;
                            bestRoughAdjusted = searchAdjusted;
                        }
                    }
                    else if (_mappedTerrain[i] == TerrainType.Unknown)
                    {
                        if (searchAdjusted < bestUnknownAdjusted)
                        {
                            bestUnknownY = searchY;
                            bestUnknownX = searchX;
                            bestUnknownAdjusted = searchAdjusted;
                        }
                    }
                }
            }

            if (bestSmoothAdjusted > bestUnknownAdjusted)
            {
                destinationX = bestUnknownX;
                destinationY = bestUnknownY;
                destinationDist = bestUnknownAdjusted;
            }
            else
            {
                destinationX = bestSmoothX;
                destinationY = bestSmoothY;
                destinationDist = bestSmoothAdjusted;
            }
            if (destinationDist > bestRoughAdjusted * roughTerrainDistMultiplier)
            {
                destinationX = bestRoughX;
                destinationY = bestRoughY;
                destinationDist = bestRoughAdjusted;
            }
            if (destinationX >= Width || destinationY >= Height)
            {

            }
        }

        private void WeightSquare(Int32 index)
        {
            squareWeight = 0;
            // Look up
            if (index >= Width)
            {
                if (_mappedTerrain[index - Width] == TerrainType.Smooth) // Reversed for Scratch
                {
                    squareWeight += 10;
                }
                else if (_mappedTerrain[index - Width] == TerrainType.Unknown)
                {
                    squareWeight += 6; // An unknown square has a 6/10 chance of being smooth
                }
                else if (_mappedTerrain[index - Width] == TerrainType.Impassable)
                {
                    squareWeight -= 5; // Impassable squares are quite undesireable
                }
            }

            // Look right
            if ((index + 1) % Width != 0)
            {
                if (_mappedTerrain[index + 1] == TerrainType.Smooth)
                {
                    squareWeight += 10;
                }
                else if (_mappedTerrain[index + 1] == TerrainType.Unknown)
                {
                    squareWeight += 6; // An unknown square has a 6/10 chance of being smooth
                }
                else if (_mappedTerrain[index + 1] == TerrainType.Impassable)
                {
                    squareWeight -= 5; // Impassable squares are quite undesireable
                }
            }

            // Look down
            if (index < Height * Width - Width)
            {
                if (_mappedTerrain[index + Width] == TerrainType.Smooth)
                {
                    squareWeight += 10;
                }
                else if (_mappedTerrain[index + Width] == TerrainType.Unknown)
                {
                    squareWeight += 6; // An unknown square has a 6/10 chance of being smooth
                }
                else if (_mappedTerrain[index + Width] == TerrainType.Impassable)
                {
                    squareWeight -= 5; // Impassable squares are quite undesireable
                }
            }

            // Look left
            if (index % Width != 0)
            {
                if (_mappedTerrain[index - 1] == TerrainType.Smooth)
                {
                    squareWeight += 10;
                }
                else if (_mappedTerrain[index - 1] == TerrainType.Unknown)
                {
                    squareWeight += 6; // An unknown square has a 6/10 chance of being smooth
                }
                else if (_mappedTerrain[index - 1] == TerrainType.Impassable)
                {
                    squareWeight -= 5; // Impassable squares are quite undesireable
                }
            }

            // Look self
            if (_mappedTerrain[index] == TerrainType.Smooth)
            {
                squareWeight += 10;
            }
            else if (_mappedTerrain[index] == TerrainType.Unknown)
            {
                squareWeight += 6; // An unknown square has a 6/10 chance of being smooth
            }
            else if (_mappedTerrain[index] == TerrainType.Impassable)
            {
                squareWeight -= 5; // Impassable squares are quite undesireable
            }
            _weightedTerrain[index] = squareWeight;

            if (index >= Width && (index + 1) % Width != 0 && index < Height * Width - Width && index % Width != 0)
            {
                Int32 j = ((index % Width) - 1) / 3;
                Int32 k = ((index / Width) - 1) / 3;
                if (_mappedTerrain[index] == TerrainType.Smooth)
                {
                    _reducedWeightMap[(Width / 3) * k + j] += 10;
                }
                else if (_mappedTerrain[index] == TerrainType.Unknown)
                {
                    _reducedWeightMap[(Width / 3) * k + j] += 6; // An unknown square has a 6/10 chance of being smooth
                }
                else if (_mappedTerrain[index] == TerrainType.Impassable)
                {
                    _reducedWeightMap[(Width / 3) * k + j] -= 5; // Impassable squares are quite undesireable
                }

            }
        }
    }
}
