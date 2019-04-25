using System;

namespace RoverSim
{
    public sealed class DefaultRoverFactory : IRoverFactory
    {
        public IRover Create(MutableLevel level) => new Rover(level);
    }
}
