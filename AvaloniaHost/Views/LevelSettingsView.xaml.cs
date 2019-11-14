using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RoverSim.AvaloniaHost.Views
{
    public class LevelSettingsView : UserControl
    {
        public LevelSettingsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
