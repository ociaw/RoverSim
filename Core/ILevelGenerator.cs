using System;

namespace RoverSim
{
    public interface ILevelGenerator
    {
        Level Generate(SimulationParameters parameters, Int32 rngSeed);
    }
}
