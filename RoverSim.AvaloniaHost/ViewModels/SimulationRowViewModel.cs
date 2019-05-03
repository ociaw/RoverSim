using System;
using ReactiveUI;

namespace RoverSim.AvaloniaHost.ViewModels
{
    public sealed class SimulationRowViewModel : ViewModelBase
    {
        public SimulationRowViewModel(IAiFactory ai, CompletedSimulation simulation)
        {
            Ai = ai ?? throw new ArgumentNullException(nameof(ai));
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
        }

        public IAiFactory Ai { get; }

        public CompletedSimulation Simulation { get; }

        public String AiName => Ai.Name;

        public Int32 MovesLeft => Simulation.Stats.MovesLeft;

        public Int32 Power => Simulation.Stats.Power;

        public Int32 SamplesTransmitted => Simulation.Stats.SamplesTransmitted;

        public String Error => Simulation.HasError ? Simulation.Exception.GetType().ToString() : null;

        public ReactiveCommand ViewDetails { get; }
    }
}
