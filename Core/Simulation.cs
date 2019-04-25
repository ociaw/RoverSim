using System;

namespace RoverSim
{
    public sealed class Simulation
    {
        public Simulation(Level originalLevel, IAi ai, IRover rover)
        {
            OriginalLevel = originalLevel ?? throw new ArgumentNullException(nameof(originalLevel));
            Ai = ai ?? throw new ArgumentNullException(nameof(ai));
            Rover = rover ?? throw new ArgumentNullException(nameof(rover));
        }

        public Level OriginalLevel { get; }

        public IAi Ai { get; }

        public IRover Rover { get; }

        public void Simulate() => Ai.Simulate(Rover);
    }
}
