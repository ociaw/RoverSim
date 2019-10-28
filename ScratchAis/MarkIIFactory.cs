using System;

namespace RoverSim.ScratchAis
{
    public sealed class MarkIIFactory : IAiFactory
    {
        public String Name => "Mark II";

        public IAi Create(SimulationParameters parameters) => new ScratchAiWrapper(new MarkII());
    }
}
