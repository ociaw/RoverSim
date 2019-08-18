using System;

namespace RoverSim.AvaloniaHost.ViewModels
{
    public sealed class RenderWindowViewModel : ViewModelBase
    {
        public RenderWindowViewModel(RenderViewModel renderViewModel)
        {
            Render = renderViewModel ?? throw new ArgumentNullException(nameof(renderViewModel));
        }

        public RenderViewModel Render { get; }
    }
}
