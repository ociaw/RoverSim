using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RoverSim;
using RoverSim.Ais;
using RoverSim.ScratchAis;

namespace MarsRoverScratchHost
{
    internal class WorkManager
    {
        internal async Task<Dictionary<IAiFactory, List<CompletedSimulation>>> Simulate(IList<IAiFactory> aiFactories, Int32 runCount)
        {
            var aiSimulations = new List<Task<List<CompletedSimulation>>>(aiFactories.Count);
            Int32 levelSeed = Rando.Next(Int32.MinValue, Int32.MaxValue);
            foreach (var aiFactory in aiFactories)
            {
                var levelRand = new Random(levelSeed);
                var levelGenerator = new DefaultLevelGenerator(levelRand);
                var roverFactory = new DefaultRoverFactory();

                var simulator = new Simulator(levelGenerator, roverFactory, aiFactory);
                aiSimulations.Add(simulator.SimulateAsync(runCount));
            }

            var completed = await Task.WhenAll(aiSimulations);
            var results = new Dictionary<IAiFactory, List<CompletedSimulation>>(aiFactories.Count);
            for (Int32 i = 0; i < completed.Length; i++)
            {
                results.Add(aiFactories[i], completed[i]);
            }
            return results;
        }
        
        public static IEnumerable<IAiFactory> GetAIs()
            => new List<IAiFactory>
            {
                new RandomAiFactory(),
                new IntelligentRandomAiFactory(),
                new MarkIFactory(),
                new MarkIIFactory(),
                new MinimalStateAiFactory(),
            };
    }
}
