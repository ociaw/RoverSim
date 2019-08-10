using System;

namespace RoverSim.Ais
{
    public sealed class MinimalStateAiFactory : IAiFactory
    {
        public String Name => "Minimal State AI";

        public IAi Create(Int32 identifier, SimulationParameters parameters) => new MinimalStateAi(identifier, parameters, 5);
    }
}
