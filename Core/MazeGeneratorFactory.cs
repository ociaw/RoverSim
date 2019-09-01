using System;

namespace RoverSim
{
    public sealed class MazeGeneratorFactory : ILevelGeneratorFactory
    {
        public ILevelGenerator Create()
        {
            Int32 seed = Rando.Next(Int32.MinValue, Int32.MaxValue);
            Random random = new Random(seed);
            return new MazeGenerator(random);
        }
    }
}
