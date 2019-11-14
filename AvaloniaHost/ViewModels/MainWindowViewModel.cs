using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace RoverSim.AvaloniaHost.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly WorkManager _workManager;

        private Int32 _runCount = 8;

        public MainWindowViewModel(LevelSettingsViewModel levelSettings, AiListViewModel aiList, SimulationListViewModel simulationList, WorkManager workManager, IScheduler scheduler)
        {
            LevelSettings = levelSettings ?? throw new ArgumentNullException(nameof(levelSettings));
            AiList = aiList ?? throw new ArgumentNullException(nameof(aiList));
            SimulationList = simulationList ?? throw new ArgumentNullException(nameof(simulationList));

            _workManager = workManager ?? throw new ArgumentNullException(nameof(workManager));
            Simulate = ReactiveCommand.CreateFromObservable(() => _workManager.Simulate(AiList.SelectedAis.ToList(), LevelSettings.SelectedGenerator.Value, RunCount), AiList.IsAnyAiSelected, scheduler);
        }

        public Int32 RunCount
        {
            get => _runCount;
            set => this.RaiseAndSetIfChanged(ref _runCount, value);
        }

        public ReactiveCommand<Unit, Unit> Simulate { get; }

        public LevelSettingsViewModel LevelSettings { get; }

        public AiListViewModel AiList { get; }

        public SimulationListViewModel SimulationList { get; }
    }
}
