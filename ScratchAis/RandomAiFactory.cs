using System;

namespace RoverSim.ScratchAis
{
    public class RandomAiFactory : IAiFactory
    {
        public String Name => "Random";

        public Int32 Seed { get; set; }

        public IAi Create(SimulationParameters parameters) => new ScratchAiWrapper(new RandomAi(Seed));
    }
}
