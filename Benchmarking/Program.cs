using BenchmarkDotNet.Running;

namespace Benchmarking
{
    public class Program
    {
        static void Main()
        {
            var summary = BenchmarkRunner.Run<LevelGeneration>();
        }
    }
}
