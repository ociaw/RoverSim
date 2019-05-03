using System;
using ReactiveUI;

namespace RoverSim.AvaloniaHost.ViewModels
{
    internal sealed class AiViewModel : ViewModelBase
    {
        private Boolean _isChecked;

        public AiViewModel(IAiFactory ai)
        {
            Ai = ai ?? throw new ArgumentNullException(nameof(ai));
        }

        public Boolean IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public String Name => Ai.Name;

        public IAiFactory Ai { get; }
    }
}
