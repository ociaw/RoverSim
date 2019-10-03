using System;
using System.Collections.Generic;

namespace RoverSim
{
    public interface IAi
    {
        Int32 Identifier { get; }

        IEnumerable<RoverAction> Simulate(IRoverStatusAccessor rover);
    }
}
