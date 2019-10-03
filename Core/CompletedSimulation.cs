using System;

namespace RoverSim
{
    public sealed class CompletedSimulation
    {
        public CompletedSimulation(Level originalLevel, SimulationParameters parameters, Int32 aiIdentifier, RoverStats stats, Exception exception)
        {
            OriginalLevel = originalLevel ?? throw new ArgumentNullException(nameof(originalLevel));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            AiIdentifier = aiIdentifier;
            Stats = stats;
            Exception = exception;
        }

        public Level OriginalLevel { get; }

        public SimulationParameters Parameters { get; }

        public Int32 AiIdentifier { get; }

        public RoverStats Stats { get; }

        public Exception Exception { get; }

        public Boolean HasError => Exception != null;
    }
}
