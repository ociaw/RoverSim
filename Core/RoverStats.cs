using System;

namespace RoverSim
{
    public readonly struct RoverStats : IEquatable<RoverStats>
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

        public static RoverStats Create(SimulationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return new RoverStats(
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
        }

        public RoverStats Add(in RoverAction action, in Update update)
        {
            // Moves and power cannot go below 0
            Int32 moves = MovesLeft + update.MoveDelta;
            Int32 power = Power + update.PowerDelta;
            return new RoverStats(
                moves > 0 ? moves : 0,
                power > 0 ? power : 0,
                SamplesCollected + update.HopperDelta,
                SamplesProcessed + update.PendingTransmissionDelta,
                SamplesTransmitted + update.TransmittedDelta,
                CollectPowerCallCount + (action.Instruction == Instruction.CollectPower ? 1 : 0),
                PowerCumulative + (update.PowerDelta > 0 ? update.PowerDelta : 0),
                CollectSampleCallCount + (action.Instruction == Instruction.CollectSample ? 1 : 0),
                ProcessSamplesCallCount + (action.Instruction == Instruction.ProcessSamples ? 1 : 0),
                TransmitCallCount + (action.Instruction == Instruction.Transmit ? 1 : 0),
                MoveCallCount + (action.Instruction == Instruction.Move ? 1 : 0),
                MoveCount + (update.PositionDelta != default ? 1 : 0)
            );
        }

        public Boolean Equals(RoverStats other) =>
            MovesLeft == other.MovesLeft &&
            Power == other.Power &&

            SamplesCollected == other.SamplesCollected &&
            SamplesProcessed == other.SamplesProcessed &&
            SamplesTransmitted == other.SamplesTransmitted &&

            CollectPowerCallCount == other.CollectPowerCallCount &&
            PowerCumulative == other.PowerCumulative &&

            CollectSampleCallCount == other.CollectSampleCallCount &&

            ProcessSamplesCallCount == other.ProcessSamplesCallCount &&

            TransmitCallCount == other.TransmitCallCount &&

            MoveCallCount == other.MoveCallCount &&
            MoveCount == other.MoveCount;

        public override Boolean Equals(Object obj) => obj is RoverStats stats && Equals(stats);

        public override Int32 GetHashCode() => HashCode.Combine(
            HashCode.Combine(MovesLeft, Power, SamplesCollected, SamplesProcessed, SamplesTransmitted, CollectPowerCallCount, PowerCumulative),
            HashCode.Combine(CollectSampleCallCount, ProcessSamplesCallCount, TransmitCallCount, MoveCallCount, MoveCount)
            );

        public static Boolean operator ==(RoverStats left, RoverStats right) => left.Equals(right);

        public static Boolean operator !=(RoverStats left, RoverStats right) => !(left == right);
    }
}
