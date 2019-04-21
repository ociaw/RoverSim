using System;

namespace MarsRoverScratch.Ais
{
    public sealed class IntelligentRandomAiFactory : IAiFactory
    {
        public String Name => "Intelligent Random";

        public Int32 Seed { get; }

        public IAi Create(Int32 identifier) => new ScratchAiWrapper(identifier, new IntelligentRandomAi(new Random(Seed + identifier)));
    }
}
