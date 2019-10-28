using System.Collections.Generic;

namespace RoverSim.ScratchAis
{
    public interface IScratchAi
    {
        IScratchAi CloneFresh();

        IEnumerable<RoverAction> Simulate(ScratchRover rover);
    }
}
