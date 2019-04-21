using System;

namespace MarsRoverScratch
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

        public static Simulation Create(Level original, IAi ai, IRoverFactory roverFactory)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));
            if (ai == null)
                throw new ArgumentNullException(nameof(ai));
            if (roverFactory == null)
                throw new ArgumentNullException(nameof(roverFactory));

            var level = original.Clone();
            var rover = roverFactory.Create(level);
            return new Simulation(original, ai, rover);
        }
    }
}
