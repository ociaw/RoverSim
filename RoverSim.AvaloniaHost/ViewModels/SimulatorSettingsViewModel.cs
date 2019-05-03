using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace RoverSim.AvaloniaHost.ViewModels
{
    internal sealed class SimulatorSettingsViewModel : ViewModelBase
    {
        private readonly WorkManager _workManager;

        private Int32 _runCount = 8;

        public SimulatorSettingsViewModel(WorkManager workManager, IObservable<Boolean> canSimulate, IEnumerable<IAiFactory> selectedAis, IScheduler scheduler)
        {
            _workManager = workManager ?? throw new ArgumentNullException(nameof(workManager));
            Simulate = ReactiveCommand.CreateFromObservable(() => _workManager.Simulate(selectedAis.ToList(), RunCount), canSimulate, scheduler);
        }

        public Int32 RunCount
        {
            get => _runCount;
            set => this.RaiseAndSetIfChanged(ref _runCount, value);
        }

        public ReactiveCommand<Unit, Unit> Simulate { get; }
    }
}
