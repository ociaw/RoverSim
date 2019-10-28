using System;
using System.Collections.Generic;

namespace RoverSim.ScratchAis
{
    internal sealed class ScratchAiWrapper : IAi
    {
        public ScratchAiWrapper(IScratchAi scratchAi)
        {
            Ai = scratchAi ?? throw new ArgumentNullException(nameof(scratchAi));
        }

        private IScratchAi Ai { get; }

        public IAi CloneFresh() => new ScratchAiWrapper(Ai.CloneFresh());

        public void Simulate(IRoverStatusAccessor rover)
        {
            var scratchRover = new ScratchRover(rover);
            Ai.Simulate(scratchRover);
        }

        IEnumerable<RoverAction> IAi.Simulate(IRoverStatusAccessor rover)
        {
            var scratchRover = new ScratchRover(rover);

            return Ai.Simulate(scratchRover);
        }
    }
}
