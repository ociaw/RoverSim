using System;

namespace RoverSim
{
    public readonly struct RoverStats
    {
        public RoverStats
        (
            Int32 movesLeft,
            Int32 power,

            Int32 samplesCollected,
            Int32 samplesProcessed,
            Int32 samplesTransmitted,

            Int32 collectPowerCallCount,
            Int32 powerCumulative,

            Int32 collectSampleCallCount,
            Int32 smoothSampleCumulative,
            Int32 roughSampleCumulative,

            Int32 processSamplesCallCount,

            Int32 transmitCallCount,

            Int32 moveCallCount,
            Int32 moveCount,

            Int32 senseCallCount
        )
        {
            MovesLeft = movesLeft;
            Power = power;

            SamplesCollected = samplesCollected;
            SamplesProcessed = samplesProcessed;
            SamplesTransmitted = samplesTransmitted;

            CollectPowerCallCount = collectPowerCallCount;
            PowerCumulative = powerCumulative;

            CollectSampleCallCount = collectSampleCallCount;
            SmoothSampleCumulative = smoothSampleCumulative;
            RoughSampleCumulative = roughSampleCumulative;

            ProcessSamplesCallCount = processSamplesCallCount;

            TransmitCallCount = transmitCallCount;

            MoveCallCount = moveCallCount;
            MoveCount = moveCount;

            SenseCallCount = senseCallCount;
        }

        public Int32 MovesLeft { get; }

        public Int32 Power { get; }

        public Int32 SamplesCollected { get; }

        public Int32 SamplesProcessed { get; }

        public Int32 SamplesTransmitted { get; }


        public Int32 CollectPowerCallCount { get; }
        public Int32 PowerCumulative { get; }

        public Int32 CollectSampleCallCount { get; }
        public Int32 SmoothSampleCumulative { get; }
        public Int32 RoughSampleCumulative { get; }

        public Int32 ProcessSamplesCallCount { get; }

        public Int32 TransmitCallCount { get; }

        public Int32 MoveCallCount { get; }
        public Int32 MoveCount { get; }

        public Int32 SenseCallCount { get; }
    }
}
