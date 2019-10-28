using System;

namespace RoverSim
{
    public sealed class CompletedSimulation
    {
        public CompletedSimulation(Int32 levelSeed, ILevelGenerator levelGenerator, SimulationParameters parameters, RoverStats stats, Exception exception)
        {
            LevelSeed = levelSeed;
            LevelGenerator = levelGenerator ?? throw new ArgumentNullException(nameof(levelGenerator));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Stats = stats;
            Exception = exception;
        }

        public Int32 LevelSeed { get; }

        public ILevelGenerator LevelGenerator { get; }

        public SimulationParameters Parameters { get; }

        public RoverStats Stats { get; }

        public Exception Exception { get; }

        public Boolean HasError => Exception != null;
    }
}
