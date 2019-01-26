using System;

namespace MarsRoverScratch.Ais
{
    public class RandomAiFactory : IAiFactory
    {
        public String Name => "Random";

        public Int32 Seed { get; }

        public IAi Create(Int32 identifier) => new RandomAi(identifier, new Random(Seed + identifier));
    }
}
