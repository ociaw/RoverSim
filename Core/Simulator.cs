using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace RoverSim
{
    public sealed class Simulator
    {
        private Int32 _simId = 0;

        public Simulator(ILevelGeneratorFactory levelGeneratorFactory, IRoverFactory roverFactory, IAiFactory aiFactory)
        {
            LevelGeneratorFactory = levelGeneratorFactory ?? throw new ArgumentNullException(nameof(levelGeneratorFactory));
            RoverFactory = roverFactory ?? throw new ArgumentNullException(nameof(roverFactory));
            AiFactory = aiFactory ?? throw new ArgumentNullException(nameof(aiFactory));
        }

        public ILevelGeneratorFactory LevelGeneratorFactory { get; }

        public IRoverFactory RoverFactory { get; }

        public IAiFactory AiFactory { get; }

        public SimulationParameters Parameters { get; } = SimulationParameters.Default;

        public String AiName => AiFactory.Name;

        public Task SimulateAsync(Int32 runCount, Action<CompletedSimulation> completionAction)
        {
            var factoryOptions = new DataflowBlockOptions { BoundedCapacity = 64 };
            var factory = new BufferBlock<(ILevelGenerator generator, Int32 count)>(factoryOptions);

            var generatorOptions = new DataflowBlockOptions();
            var generator = new TransformManyBlock<(ILevelGenerator generator, Int32 count), Level>(CreateLevels);

            var simCreatorOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 };
            var simCreator = new TransformBlock<Level, (Simulation simulation, StatsRover statsRover)>(CreateSim, simCreatorOptions);

            var performerOptions = new ExecutionDataflowBlockOptions { BoundedCapacity = 64, MaxDegreeOfParallelism = 16 };
            var performer = new TransformBlock<(Simulation simulation, StatsRover statsRover), CompletedSimulation>
            (
                PerformSim,
                performerOptions
            );
            var completerOptions = new ExecutionDataflowBlockOptions { SingleProducerConstrained = true, BoundedCapacity = 32 };
            var completer = new ActionBlock<CompletedSimulation>(completionAction, completerOptions);

            factory.LinkTo(generator, new DataflowLinkOptions { PropagateCompletion = true });
            generator.LinkTo(simCreator, new DataflowLinkOptions { PropagateCompletion = true });
            simCreator.LinkTo(performer, new DataflowLinkOptions { PropagateCompletion = true });
            performer.LinkTo(completer, new DataflowLinkOptions { PropagateCompletion = true });

            CreateGenerators(LevelGeneratorFactory, 4, runCount, factory);

            return Task.WhenAll(generator.Completion, simCreator.Completion, performer.Completion, completer.Completion);
        }

        private void CreateGenerators(ILevelGeneratorFactory generatorFactory, Int32 generatorCount, Int32 levelCount,
            ITargetBlock<(ILevelGenerator factory, Int32 count)> target)
        {
            Int32 levelsPerGenerator = levelCount / generatorCount;
            Int32 extraLevels = levelCount % generatorCount;
            for (Int32 i = 0; i < generatorCount; i++)
            {
                Int32 count = levelsPerGenerator + (i < extraLevels ? 1 : 0);
                target.Post((generatorFactory.Create(), count));
            }

            target.Complete();
        }

        private IEnumerable<Level> CreateLevels((ILevelGenerator generator, Int32 count) gen)
        {
            for (Int32 i = 0; i < gen.count; i++)
                yield return gen.generator.Generate(Parameters);
        }

        private (Simulation simulation, StatsRover statsRover) CreateSim(Level level)
        {
            IRover rover = RoverFactory.Create(level.AsMutable(), Parameters);
            StatsRover statsRover = new StatsRover(rover);
            IAi ai = AiFactory.Create(_simId, Parameters);
            _simId++;
            Simulation simulation = new Simulation(level, Parameters, ai, statsRover);
            return (simulation, statsRover);
        }

        private static CompletedSimulation PerformSim((Simulation simulation, StatsRover statsRover) sim)
        {
            (Simulation simulation, StatsRover statsRover) = sim;
            try
            {
                simulation.Simulate();
            }
            catch (Exception ex)
            {
                return new CompletedSimulation(simulation.OriginalLevel, simulation.Parameters, simulation.Ai.Identifier, statsRover.GetStats(), ex);
            }
            return new CompletedSimulation(simulation.OriginalLevel, simulation.Parameters, simulation.Ai.Identifier, statsRover.GetStats(), null);
        }
    }
}
