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

        public Int32 PowerNeeded
        {
            get
            {
                switch (Type)
                {
                    case TerrainType.Rough:
                        return Rover.RoughCost;
                    case TerrainType.SampledRough:
                    case TerrainType.Smooth:
                    case TerrainType.SampledSmooth:
                        return Rover.SmoothCost;
                    default:
                        return Int32.MaxValue;
                }
            }
        }
    }
}
