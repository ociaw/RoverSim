using System;

namespace RoverSim
{
    public struct TerrainSquare
    {
        public TerrainType Type { get; }

        public TerrainSquare(TerrainType type)
        {
            Type = type;
        }
    }
}
