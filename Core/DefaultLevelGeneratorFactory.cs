namespace RoverSim
{
    public sealed class DefaultLevelGeneratorFactory : ILevelGeneratorFactory
    {
        public ILevelGenerator Create() => new DefaultLevelGenerator();
    }
}
