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

        [GlobalSetup]
        public void Setup()
        {
            const Int32 seed = 42;
            _mazeGenerator = new MazeGenerator(new Random(seed));
            _defaultGenerator = new DefaultLevelGenerator(new Random(seed));
            _checkedGenerator = new OpenCheckingGenerator(new DefaultLevelGenerator(new Random(seed)), 5);
            _superCheckedGenerator = new OpenCheckingGenerator(new DefaultLevelGenerator(new Random(seed)), 15);
        }

        [Benchmark]
        public Level Maze() => _mazeGenerator.Generate(SimulationParameters.Default);

        [Benchmark]
        public Level Default() => _defaultGenerator.Generate(SimulationParameters.Default);

        [Benchmark]
        public Level CheckedDefault() => _checkedGenerator.Generate(SimulationParameters.Default);

        [Benchmark]
        public Level SuperCheckedDefault() => _superCheckedGenerator.Generate(SimulationParameters.Default);
    }
}
