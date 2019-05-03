using System;
using DynamicData;
using DynamicData.Binding;

namespace RoverSim.AvaloniaHost.ViewModels
{
    public sealed class SimulationListViewModel : ViewModelBase
    {
        public SimulationListViewModel(IObservableList<SimulationRowViewModel> simulations)
        {
            if (simulations == null)
                throw new ArgumentNullException(nameof(simulations));

            simulations
                .Connect()
                .Bind(Simulations)
                .Subscribe();
        }

        public ObservableCollectionExtended<SimulationRowViewModel> Simulations { get; } = new ObservableCollectionExtended<SimulationRowViewModel>();
    }
}
