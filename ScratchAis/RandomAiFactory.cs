using System;

namespace RoverSim.ScratchAis
{
    public class RandomAiFactory : IAiFactory
    {
        public String Name => "Random";

        public Int32 Seed { get; }

        public IAi Create(Int32 identifier, SimulationParameters parameters) => new ScratchAiWrapper(identifier, new RandomAi(new Random(Seed + identifier)));
    }
}
