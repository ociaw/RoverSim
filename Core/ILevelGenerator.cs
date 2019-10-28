using System;

namespace RoverSim
{
    public interface ILevelGenerator
    {
        Level Generate(Int32 rngSeed);

        SimulationParameters Parameters { get; }
    }
}
