using System;

namespace RoverSim
{
    public sealed class SimulationParameters
    {
        public Int32 LevelWidth { get; }

        public Int32 LevelHeight { get; }

        public Position InitialPosition { get; }

        public Int32 InitialMovesLeft { get; } = 1000;

        public Int32 InitialPower { get; } = 500;

        public Int32 SampleCost { get; } = 10;

        public Int32 ProcessCost { get; } = 30;

        public Int32 TransmitCost { get; } = 50;

        public Int32 MoveSmoothCost { get; } = 10;

        public Int32 MoveRoughCost { get; } = 50;

        public Int32 HopperSize { get; } = 10;

        public Int32 SamplesPerProcess { get; } = 3;

        public static SimulationParameters Default { get; } = new SimulationParameters(32, 23);

        public SimulationParameters(Int32 levelWidth, Int32 levelHeight)
            : this(levelWidth, levelHeight, new Position(levelWidth / 2, levelHeight / 2))
        { }

        public SimulationParameters(Int32 levelWidth, Int32 levelHeight, Position initialPosition)
        {
            if (levelWidth < 1)
                throw new ArgumentOutOfRangeException(nameof(levelWidth), levelWidth, "Must be positive.");
            if (levelHeight < 1)
                throw new ArgumentOutOfRangeException(nameof(levelHeight), levelHeight, "Must be positive.");
            if (initialPosition.X >= levelWidth)
                throw new ArgumentOutOfRangeException(nameof(initialPosition), initialPosition, $"X must be less than {levelWidth}.");
            if (initialPosition.Y >= levelHeight)
                throw new ArgumentOutOfRangeException(nameof(initialPosition), initialPosition, $"Y must be less than {levelHeight}.");

            LevelWidth = levelWidth;
            LevelHeight = levelHeight;
            InitialPosition = initialPosition;
        }

        public Int32 GetMovementPowerCost(TerrainType destinationTerrain)
        {
            switch (destinationTerrain)
            {
                case TerrainType.Rough:
                    return MoveRoughCost;
                case TerrainType.SampledRough:
                case TerrainType.Smooth:
                case TerrainType.SampledSmooth:
                    return MoveSmoothCost;
            }

            throw new ArgumentOutOfRangeException(nameof(destinationTerrain), destinationTerrain, "Invalid terrain type.");
        }
    }
}
