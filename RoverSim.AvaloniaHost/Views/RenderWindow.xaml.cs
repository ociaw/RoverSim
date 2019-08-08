using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RoverSim.AvaloniaHost.Views
{
    public sealed class RenderWindow : Window
    {
        public RenderWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
