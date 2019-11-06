using System;

namespace RoverSim
{
    public interface IRoverStatusAccessor
    {
        Position Position { get; }

        Int32 MovesLeft { get; }

        Int32 Power { get; }

        Int32 SamplesCollected { get; }

        Int32 SamplesProcessed { get; }

        Int32 SamplesTransmitted { get; }

        Int32 NoBacktrack { get; }

        Int32 CollectablePower { get; }

        Boolean IsHalted { get; }

        AdjacentTerrain Adjacent { get; }
    }
}
