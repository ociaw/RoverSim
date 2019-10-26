using System;

namespace RoverSim
{
    public sealed class MazeGeneratorFactory : ILevelGeneratorFactory
    {
        public ILevelGenerator Create() => new MazeGenerator();
    }
}
