using System;

namespace RoverSim
{
    public sealed class SimulationParameters
    {
        public Position BottomRight { get; }

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

        public static SimulationParameters Default { get; } = new SimulationParameters(new Position(31, 22));

        public SimulationParameters(Position bottomRight)
            : this(bottomRight, bottomRight / 2)
        { }

        public SimulationParameters(Position bottomRight, Position initialPosition)
        {
            if (!bottomRight.Contains(initialPosition))
                throw new ArgumentOutOfRangeException(nameof(initialPosition), initialPosition, "Must lie within bottom right position.");

            BottomRight = bottomRight;
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
