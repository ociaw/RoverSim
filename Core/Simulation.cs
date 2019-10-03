using System;

namespace RoverSim
{
    public sealed class Simulation
    {
        public Simulation(Level originalLevel, SimulationParameters parameters, IAi ai, Rover rover)
        {
            OriginalLevel = originalLevel ?? throw new ArgumentNullException(nameof(originalLevel));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Ai = ai ?? throw new ArgumentNullException(nameof(ai));
            Rover = rover ?? throw new ArgumentNullException(nameof(rover));
        }

        public Level OriginalLevel { get; }

        public SimulationParameters Parameters { get; }

        public IAi Ai { get; }

        public Rover Rover { get; }

        public RoverStats Simulate()
        {
            RoverStats stats = RoverStats.Create(Parameters);
            foreach (var action in Ai.Simulate(Rover.Accessor))
            {
                if (!Rover.Perform(action, out Update update))
                    break;

                stats = stats.Add(action, update);
            }

            return stats;
        }
    }
}
