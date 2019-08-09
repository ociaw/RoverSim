using System;

namespace RoverSim
{
    public interface IRover
    {
        Position Position { get; }

        Int32 MovesLeft { get; }

        Int32 Power { get; }

        Int32 SamplesCollected { get; }

        Int32 SamplesProcessed { get; }

        Int32 SamplesTransmitted { get; }

        Int32 NoBacktrack { get; }

        Boolean IsHalted { get; }

        TerrainType SenseSquare(Direction direction);

        Boolean Move(Direction direction);

        Int32 Transmit();

        Int32 CollectPower();

        (Boolean isSuccess, TerrainType newTerrain) CollectSample();

        Int32 ProcessSamples();
    }
}
