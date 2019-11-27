using System;

namespace RoverSim.Ais
{
    public sealed class PathfindingAiFactory : IAiFactory
    {
        public String Name => "Pathfinding AI";

        public IAi Create(SimulationParameters parameters) => new PathfindingAi(parameters, Map.Create(parameters));
    }
}
