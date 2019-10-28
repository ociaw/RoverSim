﻿using System;
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
            SimulationParameters parameters = SimulationParameters.Default;
            var tasks = new List<Task>(aiFactories.Count);
            var results = new Dictionary<IAiFactory, List<CompletedSimulation>>(aiFactories.Count);
            foreach (var aiFactory in aiFactories)
            {
                var levelRand = new Random(LevelSeed);
                var levelGenerator = new OpenCheckingGenerator(new DefaultLevelGenerator(parameters), 6);
                var simulator = new Simulator(levelGenerator, aiFactory);

                results[aiFactory] = new List<CompletedSimulation>(runCount);
                tasks.Add(simulator.SimulateAsync(runCount, sim => results[aiFactory].Add(sim)));
            }

            await Task.WhenAll(tasks);
            foreach (var ai in aiFactories)
            {
                Simulations.AddRange(results[ai].Select(sim => new SimulationRowViewModel(ai, sim)));
            }
        }
    }
}
