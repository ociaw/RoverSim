using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using RoverSim;
using RoverSim.ScratchAis;

namespace MarsRoverScratchHost
{
    internal class WorkManager
    {
        private static readonly Int32 threadCount = 16;
        private static ConcurrentBag<SimulationResult> results = new ConcurrentBag<SimulationResult>();
        public delegate void CompleteDelegate(IList<SimulationResult> results);
        private event CompleteDelegate TasksCompleted;

        internal WorkManager(CompleteDelegate action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            TasksCompleted += action;
        }

        internal void StartTasks(List<IAiFactory> aiFactories, Int32 runCount)
        {
            results = new ConcurrentBag<SimulationResult>();
            Task.Factory.StartNew(() =>
            {
                StartTasksInternal(aiFactories, runCount);
            });
        }

        private void StartTasksInternal(List<IAiFactory> aiFactories, Int32 runCount)
        {
            foreach (var factory in aiFactories)
            {
                ParallelOptions options = new ParallelOptions();
                options.MaxDegreeOfParallelism = threadCount;
                Parallel.For(0, runCount, options, j =>
                {
                    Random random = new Random(Rando.Next(0, Int32.MaxValue));
                    var generator = new DefaultLevelGenerator(random, 32, 23);

                    var terrain = generator.Generate();

                    var ai = factory.Create(j);
                    var simulation = new Simulation(terrain, ai, new Rover(terrain.Clone()));
                    Simulate(simulation, factory.Name);
                });
            }
            TasksCompleted.Invoke(results.ToList());
        }

        private static void Simulate(Simulation simulation, String name)
        {
            Boolean error = false;
            try
            {
                simulation.Simulate();
            }
            catch (OutOfMovesException)
            {

            }
            catch (OutOfPowerException)
            {

            }
            catch (IndexOutOfRangeException)
            {
                error = true;
            }
            catch (DivideByZeroException)
            {
                error = true;
            }
            catch (Exception)
            {
                error = true;
            }
            var result = new SimulationResult(simulation, 0, error, name);
            results.Add(result);
        }

        public static IEnumerable<IAiFactory> GetAIs()
            => new List<IAiFactory>
            {
                new RandomAiFactory(),
                new IntelligentRandomAiFactory(),
                new MarkIFactory(),
                new MarkIIFactory()
            };
    }
}
