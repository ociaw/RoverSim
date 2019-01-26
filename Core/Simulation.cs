using System;

namespace MarsRoverScratch
{
    public sealed class Simulation
    {
        private Simulation(Level originalLevel, Level level, IAi ai, Rover rover)
        {
            OriginalLevel = originalLevel ?? throw new ArgumentNullException(nameof(originalLevel));
            Level = level ?? throw new ArgumentNullException(nameof(level));
            Ai = ai ?? throw new ArgumentNullException(nameof(ai));
            Rover = rover ?? throw new ArgumentNullException(nameof(rover));
        }

        private Level OriginalLevel { get; }

        public Level Level { get; }

        public IAi Ai { get; }

        public Rover Rover { get; }

        public Boolean IsHalted => Rover.IsHalted;

        public Boolean Step() => Ai.Step(Rover);

        public Simulation CloneClean(IAi ai)
        {
            if (ai == null)
                throw new ArgumentNullException(nameof(ai));

            return Create(OriginalLevel, ai);
        }

        public static Simulation Create(Level original, IAi ai)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            var level = original.Clone();
            return new Simulation(original, level, ai, new Rover(level));
        }
    }
}
