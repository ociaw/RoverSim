using System;

namespace MarsRoverScratch
{
    public interface IRover
    {
        Int32 MovesLeft { get; }

        Int32 Power { get; }

        Int32 SamplesCollected { get; }

        Int32 SamplesProcessed { get; }

        Int32 SamplesTransmitted { get; }

        Int32 NoBacktrack { get; }

        TerrainType Sense { get; }

        Boolean IsHalted { get; }

        void SenseSquare(Direction direction);

        Boolean Move(Direction direction);

        Boolean Transmit();

        Boolean CollectPower();

        Boolean CollectSample();

        Boolean ProcessSamples();
    }
}
