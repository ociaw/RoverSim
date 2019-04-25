using System;

namespace RoverSim
{
    public sealed class DefaultRoverFactory : IRoverFactory
    {
        public IRover Create(Level level) => new Rover(level);
    }
}
