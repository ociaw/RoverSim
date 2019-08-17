using System;

using RoverSim;

namespace WinFormsClient
{
    public readonly struct PositionUpdate
    {
        public PositionUpdate(Position previous, Position newPosition)
        {
            Previous = previous;
            New = newPosition;
        }

        public Position Previous { get; }

        public Position New { get; }
    }
}
