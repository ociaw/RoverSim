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

        [GlobalSetup]
        public void Setup()
        {
            var parameters = SimulationParameters.Default;
            _mazeGenerator = new MazeGenerator(parameters);
            _defaultGenerator = new DefaultLevelGenerator(parameters);
            _checkedGenerator = new OpenCheckingGenerator(new DefaultLevelGenerator(parameters), 5);
            _superCheckedGenerator = new OpenCheckingGenerator(new DefaultLevelGenerator(parameters), 15);
        }

        [Benchmark]
        public Level Maze() => _mazeGenerator.Generate(Seed);

        [Benchmark]
        public Level Default() => _defaultGenerator.Generate(Seed);

        [Benchmark]
        public Level CheckedDefault() => _checkedGenerator.Generate(Seed);

        [Benchmark]
        public Level SuperCheckedDefault() => _superCheckedGenerator.Generate(Seed);
    }
}
