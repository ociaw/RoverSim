using System;

namespace RoverSim.AvaloniaHost.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(AiListViewModel aiList, SimulatorSettingsViewModel simulatorSettings, SimulationListViewModel simulationList)
        {
            AiList = aiList ?? throw new ArgumentNullException(nameof(aiList));
            SimulatorSettings = simulatorSettings ?? throw new ArgumentNullException(nameof(simulatorSettings));
            SimulationList = simulationList ?? throw new ArgumentNullException(nameof(simulationList));
        }

        public AiListViewModel AiList { get; }

        public SimulatorSettingsViewModel SimulatorSettings { get; }

        public SimulationListViewModel SimulationList { get; }
    }
}
