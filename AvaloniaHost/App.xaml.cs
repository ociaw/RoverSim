﻿using Avalonia;
using Avalonia.Markup.Xaml;

namespace RoverSim.AvaloniaHost
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
