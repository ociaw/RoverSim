﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;

namespace RoverSim.WinFormsClient
{
    internal class WorkManager
    {
        public WorkManager(SimulationParameters parameters)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public SimulationParameters Parameters { get; }

        public String OutputDirectory { get; } = Directory.GetCurrentDirectory();

        public Int32 NextLevelSeed { get; set; } = 0;

        internal async Task<(Dictionary<IAiFactory, (Double meanMoves, Double meanPower, Double meanSamples, Double sampleStdDev)> aggregates, CompletedSimulation worstSim, IAiFactory worstAi)> Simulate(IList<IAiFactory> aiFactories, ILevelGenerator levelGenerator, Int32 runCount, IProgress<Int32> progress)
        {
            var aggregates = new Dictionary<IAiFactory, (Double meanMoves, Double meanPower, Double meanSamples, Double sampleStdDev)>();
            Int32 initialLevelSeed = NextLevelSeed;
            CompletedSimulation worstSim = null;
            IAiFactory worstAi = null;
            foreach (var aiFactory in aiFactories)
            {
                var simulator = new Simulator(Parameters, levelGenerator, aiFactory, initialLevelSeed);

                using Completer completer = Completer.Create(Path.Combine(OutputDirectory, $"RoverSim-{aiFactory.Name}.csv"), progress);

                await simulator.SimulateAsync(runCount, completer.Consume);
                NextLevelSeed = simulator.NextLevelSeed;

                aggregates[aiFactory] = completer.GetAggregates();
                worstSim = Completer.ChooseWorst(worstSim, completer.WorstSim);
                if (worstSim == completer.WorstSim)
                    worstAi = aiFactory;
            }

            return (aggregates, worstSim, worstAi);
        }

        private sealed class Completer : IDisposable
        {
            private readonly CsvWriter _csv;
            private readonly IProgress<Int32> _progress;
            private Int64 _moves = 0;
            private Int64 _power = 0;

            // Used for calculating the standard deviation
            private Int32 _count = 0;
            private Double _meanSamples = 0;
            private Double _mean2Samples = 0;

            private Completer(CsvWriter csvWriter, IProgress<Int32> progress)
            {
                _csv = csvWriter ?? throw new ArgumentNullException(nameof(csvWriter));
                _progress = progress;
            }

            public CompletedSimulation WorstSim { get; private set; }

            public static Completer Create(String path, IProgress<Int32> progress)
            {
                TextWriter writer = new StreamWriter(path);
                CsvWriter csv = new CsvWriter(writer);
                csv.WriteField("Moves Left");
                csv.WriteField("Power Left");
                csv.WriteField("Cumulative Power");
                csv.WriteField("Samples Collected");
                csv.WriteField("Samples Processed");
                csv.WriteField("Samples Transmitted");
                csv.WriteField("Move Count");
                csv.WriteField("Move Calls");
                csv.WriteField("Power Calls");
                csv.WriteField("Sample Calls");
                csv.WriteField("Process Calls");
                csv.WriteField("Transmit Calls");
                csv.WriteField("Level Seed");
                csv.NextRecord();
                return new Completer(csv, progress);
            }

            public (Double meanMoves, Double meanPower, Double meanSamples, Double sampleStdDev) GetAggregates()
            {
                Double sampleVariance = _mean2Samples / (_count - 1);
                Double sampleStdDev = Math.Sqrt(sampleVariance);

                return (_moves / (Double)_count, _power / (Double)_count, _meanSamples, sampleStdDev);
            }

            public void Consume(CompletedSimulation sim)
            {
                RoverStats stats = sim.Stats;

                // Write CSV
                _csv.WriteField(stats.MovesLeft.ToString());
                _csv.WriteField(stats.Power.ToString());
                _csv.WriteField(stats.PowerCumulative.ToString());
                _csv.WriteField(stats.SamplesCollected.ToString());
                _csv.WriteField(stats.SamplesProcessed.ToString());
                _csv.WriteField(stats.SamplesTransmitted.ToString());
                _csv.WriteField(stats.MoveCount.ToString());
                _csv.WriteField(stats.MoveCallCount.ToString());
                _csv.WriteField(stats.CollectPowerCallCount.ToString());
                _csv.WriteField(stats.CollectSampleCallCount.ToString());
                _csv.WriteField(stats.ProcessSamplesCallCount.ToString());
                _csv.WriteField(stats.TransmitCallCount.ToString());
                _csv.WriteField(sim.ProtoLevel.Seed);
                _csv.NextRecord();

                // Update stats
                _moves += stats.MovesLeft;
                _power += stats.Power;

                _count++;
                Double delta = stats.SamplesTransmitted - _meanSamples;
                _meanSamples += delta / _count;
                Double delta2 = stats.SamplesTransmitted - _meanSamples;
                _mean2Samples += delta * delta2;

                // Update Worst Sim
                WorstSim = ChooseWorst(WorstSim, sim);
                _progress.Report(1);
            }

            public void Dispose() => _csv.Dispose();

            public static CompletedSimulation ChooseWorst(CompletedSimulation first, CompletedSimulation second)
            {
                if (first == null)
                    return second;
                if (second == null)
                    return first;
                if (first.HasError)
                    return first;
                if (second.HasError)
                    return second;

                if (first.Stats.SamplesTransmitted <= second.Stats.SamplesTransmitted)
                    return first;

                return second;
            }
        }
    }
}
