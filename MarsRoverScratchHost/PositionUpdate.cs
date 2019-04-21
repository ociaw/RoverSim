using System;

namespace MarsRoverScratchHost
{
    public readonly struct PositionUpdate
    {
        public PositionUpdate(Int32 previousX, Int32 previousY, Int32 newX, Int32 newY)
        {
            if (previousX < 0)
                throw new ArgumentOutOfRangeException(nameof(previousX), previousX, "Must be non-negative.");
            if (previousY < 0)
                throw new ArgumentOutOfRangeException(nameof(previousY), previousY, "Must be non-negative.");
            if (newX < 0)
                throw new ArgumentOutOfRangeException(nameof(newX), newX, "Must be non-negative.");
            if (newY < 0)
                throw new ArgumentOutOfRangeException(nameof(newY), newY, "Must be non-negative.");

            PreviousX = previousX;
            PreviousY = previousY;
            NewX = newX;
            NewY = newY;
        }

        public Int32 PreviousX { get; }

        public Int32 PreviousY { get; }

        public Int32 NewX { get; }

        public Int32 NewY { get; }
    }
}
