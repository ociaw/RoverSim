using System;

namespace RoverSim
{
    public interface IAi
    {
        Int32 Identifier { get; }

        void Simulate(IRover rover);
    }
}
