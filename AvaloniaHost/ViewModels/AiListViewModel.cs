using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData;

namespace RoverSim.AvaloniaHost.ViewModels
{
    internal sealed class AiListViewModel : ViewModelBase
    {
        private AiListViewModel(ObservableCollection<AiViewModel> ais)
        {
            Ais = ais ?? throw new ArgumentNullException(nameof(ais));
            var selectedAis = Ais
                .ToObservableChangeSet()
                .AutoRefresh(ai => ai.IsChecked)
                .Filter(ai => ai.IsChecked)
                .Transform(ai => ai.Ai)
                .Distinct()
                .ToCollection();
            IsAnyAiSelected = selectedAis.Select(list => list.Any());
        }

        public ObservableCollection<AiViewModel> Ais { get; }

        public IEnumerable<IAiFactory> SelectedAis => Ais.Where(ai => ai.IsChecked).Select(ai => ai.Ai);

        public IObservable<Boolean> IsAnyAiSelected { get; }

        public static AiListViewModel Create(IEnumerable<IAiFactory> ais)
        {
            var observable = new ObservableCollection<AiViewModel>(ais.Where(ai => ai != null).Select(ai => new AiViewModel(ai)));
            return new AiListViewModel(observable);
        }
    }
}
