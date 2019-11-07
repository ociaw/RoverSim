using System;
using BenchmarkDotNet.Attributes;
using RoverSim;

namespace Benchmarking
{
    [RPlotExporter, RankColumn]
    public class LevelGeneration
    {
        private MazeGenerator _mazeGenerator;

        private DefaultLevelGenerator _defaultGenerator;

        private OpenCheckingGenerator _checkedGenerator;

        private OpenCheckingGenerator _superCheckedGenerator;

        [Params]
        public Int32 Seed = 42;

        public SimulationParameters Parameters { get; }  = SimulationParameters.Default;

        [GlobalSetup]
        public void Setup()
        {
            _mazeGenerator = new MazeGenerator();
            _defaultGenerator = new DefaultLevelGenerator();
            _checkedGenerator = new OpenCheckingGenerator(new DefaultLevelGenerator(), 5);
            _superCheckedGenerator = new OpenCheckingGenerator(new DefaultLevelGenerator(), 15);
        }

        [Benchmark]
        public Level Maze() => _mazeGenerator.Generate(Parameters, Seed);

        [Benchmark]
        public Level Default() => _defaultGenerator.Generate(Parameters, Seed);

        [Benchmark]
        public Level CheckedDefault() => _checkedGenerator.Generate(Parameters, Seed);

        [Benchmark]
        public Level SuperCheckedDefault() => _superCheckedGenerator.Generate(Parameters, Seed);
    }
}
