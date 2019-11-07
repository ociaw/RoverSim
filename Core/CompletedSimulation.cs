using System;

namespace RoverSim
{
    public sealed class CompletedSimulation
    {
        public CompletedSimulation(ProtoLevel protoLevel, RoverStats stats, Exception exception)
        {
            ProtoLevel = protoLevel ?? throw new ArgumentNullException(nameof(protoLevel));
            Stats = stats;
            Exception = exception;
        }

        public ProtoLevel ProtoLevel { get; }

        public SimulationParameters Parameters => ProtoLevel.Parameters;

        public RoverStats Stats { get; }

        public Exception Exception { get; }

        public Boolean HasError => Exception != null;
    }
}
