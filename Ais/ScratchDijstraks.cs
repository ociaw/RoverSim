using System;
using System.Collections.Generic;

namespace MarsRoverScratch.Ais
{
    public sealed class ScratchDijstraks
    {
        private const Int32 WIDTH = 32;
        private const Int32 HEIGHT = 23;
        private const Int32 CELL_COUNT = WIDTH * HEIGHT;
        private const Int32 DISTANCE_HIGH_VALUE = 9999;
        private readonly List<TerrainType> _mappedTerrain;
        private Int32 _startIndex = 0;
        public Int32 destinationIndex = -1;
        private Int32 destinationDist = DISTANCE_HIGH_VALUE;
        private readonly Int32[] distances;
        private readonly Int32[] previousCell; // May not be correct type
        private Int32 _roughTerrainCost = 0;

        Int32 bestSmoothIndex = -1;
        Int32 bestSmoothDist = DISTANCE_HIGH_VALUE;
        Int32 bestRoughIndex = -1;
        Int32 bestRoughDist = DISTANCE_HIGH_VALUE;
        Int32 bestUnknownIndex = -1;
        Int32 bestUnknownDist = DISTANCE_HIGH_VALUE;

        public ScratchDijstraks(List<TerrainType> mappedTerrain)
        {
            _mappedTerrain = mappedTerrain;
            // Initialise Lists
            distances = new Int32[CELL_COUNT];
            previousCell = new Int32[CELL_COUNT];
        }

        public List<Direction> BeginSolve(Int32 startX, Int32 startY, Int32 destinationX, Int32 destinationY, Double roughTerrainDistMultiplier)
        {
            _startIndex = startY * WIDTH + startX;
            _roughTerrainCost = (Int32)(10 * roughTerrainDistMultiplier - 10);

            for (Int32 i = 0; i < CELL_COUNT; i++)
            {
                distances[i] = DISTANCE_HIGH_VALUE;
                previousCell[i] = -1;
            }

            bestSmoothIndex = -1;
            bestSmoothDist = DISTANCE_HIGH_VALUE;
            bestRoughIndex = -1;
            bestRoughDist = DISTANCE_HIGH_VALUE;
            bestUnknownIndex = -1;
            bestUnknownDist = DISTANCE_HIGH_VALUE;

            distances[_startIndex] = 0;
            previousCell[_startIndex] = -1;
            //distances[destinationIndex] = DISTANCE_HIGH_VALUE;
            //previousCell[destinationIndex] = -1;

            MapDistances(_startIndex);

            if (bestSmoothDist < destinationDist)
            {
                destinationIndex = bestSmoothIndex;
                destinationDist = bestSmoothDist;
            }
            if (bestUnknownDist < destinationDist)
            {
                destinationIndex = bestUnknownIndex;
                destinationDist = bestUnknownDist;
            }
            if (bestRoughDist * roughTerrainDistMultiplier < destinationDist)
            {
                destinationIndex = bestRoughIndex;
                destinationDist = bestRoughDist;
            }


             if (distances[destinationIndex] == DISTANCE_HIGH_VALUE)
            {
                return new List<Direction>();
            }
            else
            {
                return GenPath(destinationIndex);
            }
        }

        private void MapDistances(Int32 index)
        {
            Boolean n = false;
            Boolean s = false;
            Boolean e = false;
            Boolean w = false;
            //Int16 destinationIndex = 0;

            // TODO: Work out how to let this decide it's own destination
            Int32 column = index % WIDTH;
            Int32 row = index / WIDTH;
            if (column > 0) // column > 1 in scratch
            {
                w = SetNeighbor(distances[index] + 10, index - 1, index);
            }
            if (column < WIDTH - 1) // No -1 in scratch
            {
                e = SetNeighbor(distances[index] + 10, index + 1, index);
            }
            if (row > 0) // row > 1 in scratch
            {
                n = SetNeighbor(distances[index] + 10, index - WIDTH, index);
            }
            if (row < HEIGHT - 1) // No -1 in scratch
            {
                s = SetNeighbor(distances[index] + 10, index + WIDTH, index);
            }

            if (index != _startIndex)
            {
                switch (_mappedTerrain[index])
                {
                    case TerrainType.Smooth:
                        if (distances[index] < bestSmoothDist)
                        {
                            bestSmoothIndex = index;
                            bestSmoothDist = distances[index];
                        }
                        break;
                    case TerrainType.Rough:
                        if (distances[index] < bestRoughDist)
                        {
                            bestRoughIndex = index;
                            bestRoughDist = distances[index];
                        }
                        break;
                    case TerrainType.Unknown:
                        if (distances[index] < bestUnknownDist)
                        {
                            bestUnknownIndex = index;
                            bestUnknownDist = distances[index];
                        }
                        break;
                }
            }


            // C# Speed optimization - doesn't apply to scratch
            if (n)
                MapDistances(index - WIDTH);
            if (s)
                MapDistances(index + WIDTH);
            if (e)
                MapDistances(index + 1);
            if (w)
                MapDistances(index - 1);
        }

        private Boolean SetNeighbor(Int32 potentialDistance, Int32 neighborIndex, Int32 currentIndex)
        {
            if (_mappedTerrain[neighborIndex] != TerrainType.Impassable)
            {
                if (_mappedTerrain[neighborIndex] == TerrainType.Rough)
                {
                    potentialDistance += _roughTerrainCost;
                }
                if (distances[neighborIndex] > potentialDistance && potentialDistance < destinationDist)
                {
                    distances[neighborIndex] = potentialDistance;
                    previousCell[neighborIndex] = currentIndex;
                    return true;
                }
            }
            return false;
        }

        private List<Direction> GenPath(Int32 destinationIndex)
        {
            Int32 index = destinationIndex;
            List<Direction> path = new List<Direction>();
            
            while (index != _startIndex)
            {
                Int32 pathX = index % WIDTH;
                Int32 pathY = index / WIDTH;
                Int32 column = previousCell[index] % WIDTH;
                Int32 row = previousCell[index] / WIDTH;
                if (pathX - column > 0)
                    path.Add(Direction.Right);
                else if (pathX - column < 0)
                    path.Add(Direction.Left);
                else if (pathY - row < 0)
                    path.Add(Direction.Up); // Reversed for scratch
                else if (pathY - row > 0)
                    path.Add(Direction.Down); // Reversed for scratch
                index = previousCell[index];
            }

            path.Reverse();

            return path;
        }
    }
}
