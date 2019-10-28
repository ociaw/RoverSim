using System;

namespace RoverSim.ScratchAis
{
    public sealed class IntelligentRandomAiFactory : IAiFactory
    {
        public String Name => "Intelligent Random";

        public Int32 Seed { get; set; }

        public IAi Create(SimulationParameters parameters) => new ScratchAiWrapper(new IntelligentRandomAi(Seed));
    }
}
