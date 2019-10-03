using System;
using System.Collections.Generic;

namespace RoverSim.ScratchAis
{
    internal sealed class ScratchAiWrapper : IAi
    {
        public ScratchAiWrapper(Int32 identifier, IScratchAi scratchAi)
        {
            Ai = scratchAi ?? throw new ArgumentNullException(nameof(scratchAi));
            Identifier = identifier;
        }

        private IScratchAi Ai { get; }

        public Int32 Identifier { get; }

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
