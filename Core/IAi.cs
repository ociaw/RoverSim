using System;

namespace MarsRoverScratch
{
    public interface IAi
    {
        Int32 Identifier { get; }

        void Simulate(IRover rover);
    }
}
