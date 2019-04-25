using System;

namespace RoverSim
{
    public sealed class CompletedSimulation
    {
        public CompletedSimulation(Level originalLevel, Int32 aiIdentifier, RoverStats stats, Exception exception)
        {
            OriginalLevel = originalLevel ?? throw new ArgumentNullException(nameof(originalLevel));
            AiIdentifier = aiIdentifier;
            Stats = stats;
            Exception = exception;
        }

        public Level OriginalLevel { get; }

        public Int32 AiIdentifier { get; }

        public RoverStats Stats { get; }

        public Exception Exception { get; }

        public Boolean HasError => Exception != null && !(Exception is OutOfPowerOrMovesException);
    }
}
