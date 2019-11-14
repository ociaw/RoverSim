using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace RoverSim.AvaloniaHost.ViewModels
{
    public sealed class LevelSettingsViewModel : ViewModelBase
    {
        private KeyValuePair<String, ILevelGenerator> _selectedGenerator;
        private Int32 _initialSeed;

        public LevelSettingsViewModel(IReadOnlyDictionary<String, ILevelGenerator> levelGenerators)
        {
            LevelGenerators = new ObservableCollection<KeyValuePair<string, ILevelGenerator>>(levelGenerators);
            _selectedGenerator = LevelGenerators.Count > 0 ? LevelGenerators[0] : default;
        }

        public ObservableCollection<KeyValuePair<String, ILevelGenerator>> LevelGenerators { get; }

        public KeyValuePair<String, ILevelGenerator> SelectedGenerator
        {
            get => _selectedGenerator;
            set => this.RaiseAndSetIfChanged(ref _selectedGenerator, value);
        }

        public Int32 InitialSeed
        {
            get => _initialSeed;
            set => this.RaiseAndSetIfChanged(ref _initialSeed, value);
        }
    }
}
