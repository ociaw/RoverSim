using System;

namespace RoverSim.ScratchAis
{
    public sealed class IntelligentRandomAiFactory : IAiFactory
    {
        public String Name => "Intelligent Random";

        public Int32 Seed { get; }

        public IAi Create(Int32 identifier, SimulationParameters parameters) => new ScratchAiWrapper(identifier, new IntelligentRandomAi(new Random(Seed + identifier)));
    }
}
