using System;

namespace RoverSim
{
    public sealed class SimulationParameters
    {
        public Int32 LevelWidth { get; }

        public Int32 LevelHeight { get; }

        public Int32 InitialX { get; }

        public Int32 InitialY { get; }

        public Int32 InitialMovesLeft { get; } = 1000;

        public Int32 InitialPower { get; } = 500;

        public Int32 SampleCost { get; } = 10;

        public Int32 ProcessCost { get; } = 30;

        public Int32 TransmitCost { get; } = 50;

        public Int32 MoveSmoothCost { get; } = 10;

        public Int32 MoveRoughCost { get; } = 50;

        public static SimulationParameters Default { get; } = new SimulationParameters(32, 23);

        public SimulationParameters(Int32 levelWidth, Int32 levelHeight)
            : this(levelWidth, levelHeight, levelWidth / 2, levelHeight / 2)
        { }

        public SimulationParameters(Int32 levelWidth, Int32 levelHeight, Int32 initialX, Int32 initialY)
        {
            if (levelWidth < 1)
                throw new ArgumentOutOfRangeException(nameof(levelWidth), levelWidth, "Must be positive.");
            if (levelHeight < 1)
                throw new ArgumentOutOfRangeException(nameof(levelHeight), levelHeight, "Must be positive.");
            if (initialX < 1)
                throw new ArgumentOutOfRangeException(nameof(initialX), initialX, "Must be positive.");
            if (initialY < 1)
                throw new ArgumentOutOfRangeException(nameof(initialY), initialY, "Must be positive.");
            if (initialX >= levelWidth)
                throw new ArgumentOutOfRangeException(nameof(initialX), initialX, $"Must be less than {levelWidth}.");
            if (initialY >= levelHeight)
                throw new ArgumentOutOfRangeException(nameof(initialY), initialY, $"Must be less than {levelHeight}.");

            LevelWidth = levelWidth;
            LevelHeight = levelHeight;
            InitialX = initialX;
            InitialY = initialY;
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
