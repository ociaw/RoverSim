using System;

namespace MarsRoverScratch
{
    public interface IAi
    {
        Int32 Identifier { get; }

        Boolean Step(Rover rover);
    }
}
