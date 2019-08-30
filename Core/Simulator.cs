﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace RoverSim
{
    public sealed class Simulator
    {
        public Simulator(ILevelGenerator levelGenerator, IRoverFactory roverFactory, IAiFactory aiFactory)
        {
            LevelGenerator = levelGenerator ?? throw new ArgumentNullException(nameof(levelGenerator));
            RoverFactory = roverFactory ?? throw new ArgumentNullException(nameof(roverFactory));
            AiFactory = aiFactory ?? throw new ArgumentNullException(nameof(aiFactory));
        }

        public ILevelGenerator LevelGenerator { get; }

        public IRoverFactory RoverFactory { get; }

        public IAiFactory AiFactory { get; }

        public SimulationParameters Parameters { get; } = SimulationParameters.Default;

        public String AiName => AiFactory.Name;

        public Task SimulateAsync(Int32 runCount, Action<CompletedSimulation> completionAction)
        {
            var productionQueue = new BufferBlock<(Simulation simulation, StatsRover statsRover)>();

            var consumerOptions = new ExecutionDataflowBlockOptions { BoundedCapacity = 64, MaxDegreeOfParallelism = 16 };
            var simConsumer = new TransformBlock<(Simulation simulation, StatsRover statsRover), CompletedSimulation>
            (
                DoSimulation,
                consumerOptions
            );
            var completerOptions = new ExecutionDataflowBlockOptions { SingleProducerConstrained = true, BoundedCapacity = 32 };
            var completer = new ActionBlock<CompletedSimulation>(completionAction, completerOptions);

            productionQueue.LinkTo(simConsumer, new DataflowLinkOptions { PropagateCompletion = true });
            simConsumer.LinkTo(completer, new DataflowLinkOptions { PropagateCompletion = true });

            var simProducer = ProduceSimulations(productionQueue, Parameters, runCount);

            return Task.WhenAll(simProducer, simConsumer.Completion, completer.Completion);
        }

        private Task ProduceSimulations(ITargetBlock<(Simulation simulation, StatsRover statsRover)> target, SimulationParameters parameters, Int32 count)
        {
            return Task.Run(async () =>
            {
                for (Int32 i = 0; i < count; i++)
                {
                    Level originalLevel = LevelGenerator.Generate(parameters);
                    IRover rover = RoverFactory.Create(originalLevel.AsMutable(), parameters);
                    StatsRover statsRover = new StatsRover(rover);
                    IAi ai = AiFactory.Create(i, parameters);
                    Simulation simulation = new Simulation(originalLevel, Parameters, ai, statsRover);
                    await target.SendAsync((simulation, statsRover));
                }
                target.Complete();
            });
        }

        private static CompletedSimulation DoSimulation((Simulation simulation, StatsRover statsRover) sim)
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
