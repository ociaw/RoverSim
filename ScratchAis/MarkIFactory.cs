using System;

namespace RoverSim.ScratchAis
{
    public sealed class MarkIFactory : IAiFactory
    {
        public String Name => "Mark I";

        public IAi Create(SimulationParameters parameters) => new ScratchAiWrapper(new MarkI());
    }
}
