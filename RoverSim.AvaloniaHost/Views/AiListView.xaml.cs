using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RoverSim.AvaloniaHost.Views
{
    public class AiListView : UserControl
    {
        public AiListView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
