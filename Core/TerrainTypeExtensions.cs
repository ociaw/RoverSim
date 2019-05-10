using System;

namespace RoverSim
{
    public static class TerrainTypeExtensions
    {
        public static Boolean IsSampleable(this TerrainType terrain) => terrain == TerrainType.Smooth || terrain == TerrainType.Rough;
    }
}
