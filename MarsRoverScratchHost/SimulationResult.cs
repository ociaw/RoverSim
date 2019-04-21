using System;
using MarsRoverScratch;

namespace MarsRoverScratchHost
{
    internal sealed class SimulationResult
    {
        public SimulationResult(Simulation simulation, UInt16 run, Boolean error, String aiName)
        {
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
            Run = run;
            Error = error;
            AiName = aiName ?? throw new ArgumentNullException(nameof(aiName));
        }

        public Simulation Simulation { get; }

        public IRover Rover => Simulation.Rover;

        public IAi Ai => Simulation.Ai;

        public UInt16 Run { get; }

        public Boolean Error { get; }

        public String AiName { get; }
    }
}