using System;

namespace RoverSim.ScratchAis
{
    public sealed class MarkIIFactory : IAiFactory
    {
        public String Name => "Mark II";

        public IAi Create(Int32 identifier, SimulationParameters parameters) => new ScratchAiWrapper(identifier, new MarkII());
    }
}
