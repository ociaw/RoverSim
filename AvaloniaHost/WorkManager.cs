using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using RoverSim.AvaloniaHost.ViewModels;

namespace RoverSim.AvaloniaHost
{
    internal class WorkManager
    {
        public Int32 LevelSeed { get; set; } = 42;

        public SourceList<SimulationRowViewModel> Simulations { get; } = new SourceList<SimulationRowViewModel>();

        internal IObservable<Unit> Simulate(IReadOnlyList<IAiFactory> aiFactories, Int32 runCount)
        {
            return Observable.StartAsync(() =>
            {
                return SimulateAsync(aiFactories, runCount);
            });
        }

        internal async Task SimulateAsync(IReadOnlyList<IAiFactory> aiFactories, Int32 runCount)
        {
            var aiSimulations = new List<Task<List<CompletedSimulation>>>(aiFactories.Count);
            foreach (var aiFactory in aiFactories)
            {
                var levelRand = new Random(LevelSeed);
                var levelGenerator = new DefaultLevelGenerator(levelRand);
                var roverFactory = new DefaultRoverFactory();

                var simulator = new Simulator(levelGenerator, roverFactory, aiFactory);
                aiSimulations.Add(simulator.SimulateAsync(runCount));
            }

            var completed = await Task.WhenAll(aiSimulations);
            for (Int32 i = 0; i < completed.Length; i++)
            {
                IAiFactory ai = aiFactories[i];
                Simulations.AddRange(completed[i].Select(sim => new SimulationRowViewModel(ai, sim)));
            }
        }
    }
}
