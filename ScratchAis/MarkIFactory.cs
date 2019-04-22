using System;

namespace RoverSim.ScratchAis
{
    public sealed class MarkIFactory : IAiFactory
    {
        public String Name => "Mark I";

        public IAi Create(Int32 identifier) => new ScratchAiWrapper(identifier, new MarkI(identifier));
    }
}
