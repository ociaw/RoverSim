using System;

namespace RoverSim.Ais
{
    public sealed class FixedStateAiFactory : IAiFactory
    {
        public String Name => "Fixed State AI";

        public IAi Create(SimulationParameters parameters) => new FixedStateAi(parameters, 5);
    }
}
