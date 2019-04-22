using System;

namespace RoverSim
{
    public interface IAiFactory
    {
        String Name { get; }

        IAi Create(Int32 identifier);
    }
}
