using System;

namespace RoverSim.Ais
{
    public sealed class LimitedStateAiFactory : IAiFactory
    {
        public LimitedStateAiFactory() 
            : this(5) 
        { }

        public LimitedStateAiFactory(Int32 memoryCount)
        {
            if (memoryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(memoryCount), "Must be non-negative.");

            MemoryCount = memoryCount;
        }

        public String Name => "Limited State AI";

        public Int32 MemoryCount { get; }

        public IAi Create(SimulationParameters parameters) => new LimitedStateAi(parameters, MemoryCount);
    }
}
