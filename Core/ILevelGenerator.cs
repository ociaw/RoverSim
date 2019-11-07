using System;

namespace RoverSim
{
    public interface ILevelGenerator
    {
        Level Generate(SimulationParameters paramters, Int32 rngSeed);
    }
}
