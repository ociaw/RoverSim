using System;
using MarsRoverScratch;

namespace MarsRoverScratchHost
{
    public readonly struct TerrainUpdate
    {
        public TerrainUpdate(Int32 posX, Int32 posY, TerrainType newTerrain)
        {
            if (posX < 0)
                throw new ArgumentOutOfRangeException(nameof(posX), posX, "Must be non-negative.");
            if (posY < 0)
                throw new ArgumentOutOfRangeException(nameof(posY), posY, "Must be non-negative.");
            if (newTerrain < TerrainType.Impassable || newTerrain > TerrainType.Unknown)
                throw new ArgumentOutOfRangeException(nameof(newTerrain), newTerrain, "Must be a defined enum value.");

            PosX = posX;
            PosY = posY;
            NewTerrain = newTerrain;
        }

        public Int32 PosX { get; }

        public Int32 PosY { get; }

        public TerrainType NewTerrain { get; }
    }
}
