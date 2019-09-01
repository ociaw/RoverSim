using System;

namespace RoverSim
{
    public sealed class OpenCheckingGeneratorFactory : ILevelGeneratorFactory
    {
        private readonly ILevelGeneratorFactory _wrappedFactory;

        public OpenCheckingGeneratorFactory(ILevelGeneratorFactory wrappedFactory, Int32 minimumContiguousTiles)
        {
            _wrappedFactory = wrappedFactory ?? throw new ArgumentNullException(nameof(wrappedFactory));
            MinimumContiguousTiles = minimumContiguousTiles >= 0 ? minimumContiguousTiles : throw new ArgumentOutOfRangeException(nameof(minimumContiguousTiles));
        }

        public Int32 MinimumContiguousTiles { get; }

        public ILevelGenerator Create()
        {
            var wrapped = _wrappedFactory.Create();
            return new OpenCheckingGenerator(wrapped, MinimumContiguousTiles);
        }
    }
}
