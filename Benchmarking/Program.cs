using BenchmarkDotNet.Running;

namespace Benchmarking
{
    public class Program
    {
        static void Main()
        {
            var _ = BenchmarkRunner.Run<LevelGeneration>();
        }
    }
}
