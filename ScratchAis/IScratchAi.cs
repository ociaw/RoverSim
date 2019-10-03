using System.Collections.Generic;

namespace RoverSim.ScratchAis
{
    public interface IScratchAi
    {
        IEnumerable<RoverAction> Simulate(ScratchRover rover);
    }
}
