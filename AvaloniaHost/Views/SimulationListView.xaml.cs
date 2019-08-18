using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RoverSim.AvaloniaHost.Views
{
    public class SimulationListView : UserControl
    {
        public SimulationListView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
