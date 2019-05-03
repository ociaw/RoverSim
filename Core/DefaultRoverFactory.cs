namespace RoverSim
{
    public sealed class DefaultRoverFactory : IRoverFactory
    {
        public IRover Create(MutableLevel level, SimulationParameters parameters) => new Rover(level, parameters);
    }
}
