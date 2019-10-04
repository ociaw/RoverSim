using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace RoverSim
{
    public sealed class Simulator
    {
        private Int32 _simId = 0;
        private Int32 _taskCount = Environment.ProcessorCount;

        public Simulator(ILevelGeneratorFactory levelGeneratorFactory, IAiFactory aiFactory)
        {
            LevelGeneratorFactory = levelGeneratorFactory ?? throw new ArgumentNullException(nameof(levelGeneratorFactory));
            AiFactory = aiFactory ?? throw new ArgumentNullException(nameof(aiFactory));
        }

        public ILevelGeneratorFactory LevelGeneratorFactory { get; }

        public IAiFactory AiFactory { get; }

        public SimulationParameters Parameters { get; } = SimulationParameters.Default;

        public String AiName => AiFactory.Name;

        public Int32 TaskCount
        {
            get => _taskCount;
            set => _taskCount = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Must be positive.");
        }

        public Task SimulateAsync(Int32 runCount, Action<CompletedSimulation> completionAction)
        {
            Int32 capacity = TaskCount * 4;
            var completerOptions = new ExecutionDataflowBlockOptions { BoundedCapacity = capacity, MaxDegreeOfParallelism = 1 };
            var completer = new ActionBlock<CompletedSimulation>(completionAction, completerOptions);

            return SimulateAsync(runCount, completer);
        }

        public Task SimulateAsync(Int32 runCount, ActionBlock<CompletedSimulation> completer)
        {
            if (completer == null)
                throw new ArgumentNullException(nameof(completer));

            Int32 capacity = TaskCount * 4;

            var simCreatorOptions = new ExecutionDataflowBlockOptions { BoundedCapacity = capacity, MaxDegreeOfParallelism = 1 };
            var simCreator = new TransformBlock<Level, Simulation>(CreateSim, simCreatorOptions);

            var performerOptions = new ExecutionDataflowBlockOptions { BoundedCapacity = capacity, MaxDegreeOfParallelism = TaskCount };
            var performer = new TransformBlock<Simulation, CompletedSimulation>
            (
                PerformSim,
                performerOptions
            );

            simCreator.LinkTo(performer, new DataflowLinkOptions { PropagateCompletion = true });
            performer.LinkTo(completer, new DataflowLinkOptions { PropagateCompletion = true });

            Int32 genCount = TaskCount / 2;
            var genTask = StartGenerators(LevelGeneratorFactory, genCount, runCount, simCreator);

            return Task.WhenAll(genTask, simCreator.Completion, performer.Completion, completer.Completion);
        }

        private async Task StartGenerators(ILevelGeneratorFactory generatorFactory, Int32 generatorCount, Int32 levelCount, ITargetBlock<Level> target)
        {
            Int32 levelsPerGenerator = levelCount / generatorCount;
            Int32 extraLevels = levelCount % generatorCount;
            List<Task> list = new List<Task>();
            for (Int32 i = 0; i < generatorCount; i++)
            {
                Int32 count = levelsPerGenerator + (i < extraLevels ? 1 : 0);
                list.Add(RunGenerator(generatorFactory.Create(), count, target));
            }

            await Task.WhenAll(list);
            target.Complete();
        }

        private Task RunGenerator(ILevelGenerator generator, Int32 count, ITargetBlock<Level> target)
        {
            return Task.Run(async () =>
            {
                for (Int32 i = 0; i < count; i++)
                    await target.SendAsync(generator.Generate(Parameters));
            });
        }

        private Simulation CreateSim(Level level)
        {
            Rover rover = new Rover(level.AsMutable(), Parameters);
            IAi ai = AiFactory.Create(_simId, Parameters);
            _simId++;
            Simulation simulation = new Simulation(level, Parameters, ai, rover);
            return simulation;
        }

        private static CompletedSimulation PerformSim(Simulation simulation)
        {
            RoverStats stats;
            try
            {
                stats = simulation.Simulate();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // Catching this is fine, as we want to be able to report it later.
                return new CompletedSimulation(simulation.OriginalLevel, simulation.Parameters, simulation.Ai.Identifier, default, ex);
            }
            return new CompletedSimulation(simulation.OriginalLevel, simulation.Parameters, simulation.Ai.Identifier, stats, null);
        }
    }
}
