using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Logging.Serilog;
using DynamicData;
using RoverSim.Ais;
using RoverSim.AvaloniaHost.ViewModels;
using RoverSim.AvaloniaHost.Views;
using RoverSim.ScratchAis;

namespace RoverSim.AvaloniaHost
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseDataGrid()
                .UseReactiveUI();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            var schedule = Avalonia.Threading.AvaloniaScheduler.Instance;

            var levelSettings = new LevelSettingsViewModel(GetLevelGenerators());
            var ais = GetAvailableAis();
            var aiList = AiListViewModel.Create(ais);
            var workManager = new WorkManager();

            var renderCommand = ReactiveUI.ReactiveCommand.Create<SimulationRowViewModel>((simRow) =>
            {
                var renderVm = new RenderViewModel(simRow.Ai, simRow.Simulation);
                var renderWindow = new RenderWindow
                {
                    DataContext = new RenderWindowViewModel(renderVm)
                };
                renderWindow.Show();
            });

            var simulationList = new SimulationListViewModel(workManager.Simulations.AsObservableList(), renderCommand);

            var window = new MainWindow
            {
                DataContext = new MainWindowViewModel(levelSettings, aiList, simulationList, workManager, schedule),
                Width = 600,
                Height = 400
            };

            app.Run(window);
        }

        private static IEnumerable<IAiFactory> GetAvailableAis()
        {
            return new List<IAiFactory>
            {
                new FixedStateAiFactory(),
                new RandomAiFactory(),
                new IntelligentRandomAiFactory(),
                new MarkIFactory(),
                new MarkIIFactory(),
            };
        }

        private static IReadOnlyDictionary<String, ILevelGenerator> GetLevelGenerators()
            => new Dictionary<String, ILevelGenerator>
            {
                { "Default", new OpenCheckingGenerator(new DefaultLevelGenerator(), 6) },
                { "Maze", new MazeGenerator() }
            };
    }
}
