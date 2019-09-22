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
            var tasks = new List<Task>(aiFactories.Count);
            var results = new Dictionary<IAiFactory, List<CompletedSimulation>>(aiFactories.Count);
            foreach (var aiFactory in aiFactories)
            {
                var levelRand = new Random(LevelSeed);
                var levelGeneratorFactory = new OpenCheckingGeneratorFactory(new DefaultLevelGeneratorFactory(), 6);
                var roverFactory = new DefaultRoverFactory();
                var simulator = new Simulator(levelGeneratorFactory, roverFactory, aiFactory);

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