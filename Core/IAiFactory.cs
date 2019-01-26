using System;

namespace MarsRoverScratch
{
    public interface IAiFactory
    {
        String Name { get; }

        IAi Create(Int32 identifier);
    }
}
