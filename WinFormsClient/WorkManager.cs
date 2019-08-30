using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RoverSim.Ais;
using RoverSim.ScratchAis;

namespace RoverSim.WinFormsClient
{
    internal class WorkManager
    {
        internal async Task<Dictionary<IAiFactory, List<CompletedSimulation>>> Simulate(IList<IAiFactory> aiFactories, Int32 runCount)
        {
            var tasks = new List<Task>(aiFactories.Count);
            var results = new Dictionary<IAiFactory, List<CompletedSimulation>>(aiFactories.Count);
            Int32 levelSeed = Rando.Next(Int32.MinValue, Int32.MaxValue);
            foreach (var aiFactory in aiFactories)
            {
                var levelRand = new Random(levelSeed);
                var levelGenerator = new OpenCheckingGenerator(new DefaultLevelGenerator(levelRand), 6);
                var roverFactory = new DefaultRoverFactory();
                var simulator = new Simulator(levelGenerator, roverFactory, aiFactory);

                results[aiFactory] = new List<CompletedSimulation>(runCount);
                tasks.Add(simulator.SimulateAsync(runCount, sim => results[aiFactory].Add(sim)));
            }

            await Task.WhenAll(tasks);
            return results;
        }
        
        public static IEnumerable<IAiFactory> GetAIs()
            => new List<IAiFactory>
            {
                new RandomAiFactory(),
                new IntelligentRandomAiFactory(),
                new MarkIFactory(),
                new MarkIIFactory(),
                new FixedStateAiFactory(),
            };
    }
}
