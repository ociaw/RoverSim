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

            Int32 processSamplesCallCount,

            Int32 transmitCallCount,

            Int32 moveCallCount,
            Int32 moveCount
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

            ProcessSamplesCallCount = processSamplesCallCount;

            TransmitCallCount = transmitCallCount;

            MoveCallCount = moveCallCount;
            MoveCount = moveCount;
        }

        public Int32 MovesLeft { get; }

        public Int32 Power { get; }

        public Int32 SamplesCollected { get; }

        public Int32 SamplesProcessed { get; }

        public Int32 SamplesTransmitted { get; }


        public Int32 CollectPowerCallCount { get; }
        public Int32 PowerCumulative { get; }

        public Int32 CollectSampleCallCount { get; }

        public Int32 ProcessSamplesCallCount { get; }

        public Int32 TransmitCallCount { get; }

        public Int32 MoveCallCount { get; }
        public Int32 MoveCount { get; }

        public static RoverStats Create(SimulationParameters parameters) =>
            new RoverStats(
                parameters.InitialMovesLeft,
                parameters.InitialPower,
                0,
                0,
                0,
                0,
                parameters.InitialPower,
                0,
                0,
                0,
                0,
                0
            );

        public RoverStats Add(in RoverAction action, in Update update) =>
            new RoverStats(
                MovesLeft + update.MoveDelta,
                Power + update.PowerDelta,
                SamplesCollected + update.HopperDelta,
                SamplesProcessed + update.PendingTransmissionDelta,
                SamplesTransmitted + update.TransmittedDelta,
                CollectPowerCallCount + action.Instruction == Instruction.CollectPower ? 1 : 0,
                PowerCumulative + update.PowerDelta > 0 ? update.PowerDelta : 0,
                CollectSampleCallCount + action.Instruction == Instruction.CollectSample ? 1 : 0,
                ProcessSamplesCallCount + action.Instruction == Instruction.ProcessSamples ? 1 : 0,
                TransmitCallCount + action.Instruction == Instruction.Transmit ? 1 : 0,
                MoveCallCount + action.Instruction == Instruction.Move ? 1 : 0,
                MoveCount + (update.PositionDelta != default ? 1 : 0)
            );
    }
}
