using System;

namespace RoverSim.Ais
{
    public sealed class FixedStateAiFactory : IAiFactory
    {
        public FixedStateAiFactory() 
            : this(5) 
        { }

        public FixedStateAiFactory(Int32 memoryCount)
        {
            if (memoryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(memoryCount), "Must be non-negative.");

            MemoryCount = memoryCount;
        }

        public String Name => "Fixed State AI";

        public Int32 MemoryCount { get; }

        public IAi Create(SimulationParameters parameters) => new FixedStateAi(parameters, MemoryCount);
    }
}
