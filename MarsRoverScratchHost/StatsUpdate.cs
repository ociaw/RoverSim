using System;

namespace RoverSim.WinFormsClient
{
    public readonly struct StatsUpdate
    {
        public StatsUpdate(
            Int32 moveCount,
            Int32 movesLeft,
            Int32 power,
            Int32 sampleCount,
            Int32 processedCount,
            Int32 transmittedCount,
            Int32 noBacktrack)
        {
            if (moveCount < 0)
                throw new ArgumentOutOfRangeException(nameof(moveCount), moveCount, "Must be non-negative.");
            if (movesLeft < 0)
                throw new ArgumentOutOfRangeException(nameof(movesLeft), movesLeft, "Must be non-negative.");
            if (power < 0)
                throw new ArgumentOutOfRangeException(nameof(power), power, "Must be non-negative.");
            if (sampleCount < 0)
                throw new ArgumentOutOfRangeException(nameof(sampleCount), sampleCount, "Must be non-negative.");
            if (processedCount < 0)
                throw new ArgumentOutOfRangeException(nameof(processedCount), processedCount, "Must be non-negative.");
            if (transmittedCount < 0)
                throw new ArgumentOutOfRangeException(nameof(transmittedCount), transmittedCount, "Must be non-negative.");
            if (noBacktrack < 0)
                throw new ArgumentOutOfRangeException(nameof(noBacktrack), noBacktrack, "Must be non-negative.");

            MoveCount = moveCount;
            MovesLeft = movesLeft;
            Power = power;
            SampleCount = sampleCount;
            ProcessedCount = processedCount;
            TransmitedCount = transmittedCount;
            NoBacktrack = noBacktrack;
        }

        public Int32 MoveCount { get; }

        public Int32 MovesLeft { get; }

        public Int32 Power { get; }

        public Int32 SampleCount { get; }

        public Int32 ProcessedCount { get; }

        public Int32 TransmitedCount { get; }

        public Int32 NoBacktrack { get; }
    }
}
