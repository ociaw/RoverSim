using System;
using DynamicData;
using DynamicData.Binding;

namespace RoverSim.AvaloniaHost.ViewModels
{
    public sealed class SimulationListViewModel : ViewModelBase
    {
        public SimulationListViewModel(IObservableList<SimulationRowViewModel> simulations, ReactiveUI.ReactiveCommand renderCommand)
        {
            if (simulations == null)
                throw new ArgumentNullException(nameof(simulations));

            simulations
                .Connect()
                .Bind(Simulations)
                .Subscribe();

            RenderCommand = renderCommand ?? throw new ArgumentNullException(nameof(renderCommand));
        }

        public ObservableCollectionExtended<SimulationRowViewModel> Simulations { get; } = new ObservableCollectionExtended<SimulationRowViewModel>();

        public ReactiveUI.ReactiveCommand RenderCommand { get; }
    }
}
