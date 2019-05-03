using System;

namespace RoverSim
{
    public sealed class Simulation
    {
        public Simulation(Level originalLevel, SimulationParameters parameters, IAi ai, IRover rover)
        {
            OriginalLevel = originalLevel ?? throw new ArgumentNullException(nameof(originalLevel));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Ai = ai ?? throw new ArgumentNullException(nameof(ai));
            Rover = rover ?? throw new ArgumentNullException(nameof(rover));
        }

        public Level OriginalLevel { get; }

        public SimulationParameters Parameters { get; }

        public IAi Ai { get; }

        public IRover Rover { get; }

        public void Simulate() => Ai.Simulate(Rover);
    }
}
