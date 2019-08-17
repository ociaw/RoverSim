using System;

namespace RoverSim.Ais
{
    public sealed class FixedStateAiFactory : IAiFactory
    {
        public String Name => "Fixed State AI";

        public IAi Create(Int32 identifier, SimulationParameters parameters) => new FixedStateAi(identifier, parameters, 5);
    }
}
