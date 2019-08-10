using System;
using RoverSim;

namespace MarsRoverScratchHost
{
    public readonly struct TerrainUpdate
    {
        public TerrainUpdate(Position position, TerrainType newTerrain)
        {
            if (newTerrain < TerrainType.Impassable || newTerrain > TerrainType.Unknown)
                throw new ArgumentOutOfRangeException(nameof(newTerrain), newTerrain, "Must be a defined enum value.");

            Position = position;
            NewTerrain = newTerrain;
        }

        public Position Position { get; }

        public TerrainType NewTerrain { get; }
    }
}
