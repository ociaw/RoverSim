using System;

using RoverSim;

namespace MarsRoverScratchHost
{
    public readonly struct PositionUpdate
    {
        public PositionUpdate(Position previous, Position newPosition)
        {
            if (previous.IsNegative)
                throw new ArgumentOutOfRangeException(nameof(previous), previous, "Must be non-negative.");
            if (newPosition.IsNegative)
                throw new ArgumentOutOfRangeException(nameof(newPosition), newPosition, "Must be non-negative.");

            Previous = previous;
            New = newPosition;
        }

        public Position Previous { get; }

        public Position New { get; }

        public Int32 PreviousX => Previous.X;

        public Int32 PreviousY => Previous.Y;

        public Int32 NewX => New.X;

        public Int32 NewY => New.Y;
    }
}
