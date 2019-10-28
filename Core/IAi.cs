using System;
using System.Collections.Generic;

namespace RoverSim
{
    public interface IAi
    {
        IEnumerable<RoverAction> Simulate(IRoverStatusAccessor rover);

        IAi CloneFresh();
    }
}
