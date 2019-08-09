using System;
using RoverSim;

namespace MarsRoverScratchHost
{
    public readonly struct TerrainUpdate
    {
        public TerrainUpdate(Position position, TerrainType newTerrain)
        {
            if (position.IsNegative)
                throw new ArgumentOutOfRangeException(nameof(position), position, "Must be non-negative.");
            if (newTerrain < TerrainType.Impassable || newTerrain > TerrainType.Unknown)
                throw new ArgumentOutOfRangeException(nameof(newTerrain), newTerrain, "Must be a defined enum value.");

            Position = position;
            NewTerrain = newTerrain;
        }

        public Position Position { get; }

        public Int32 PosX => Position.X;

        public Int32 PosY => Position.Y;

        public TerrainType NewTerrain { get; }
    }
}
