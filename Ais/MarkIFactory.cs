using System;

namespace MarsRoverScratch.Ais
{
    public sealed class MarkIFactory : IAiFactory
    {
        public String Name => "Mark I";

        public IAi Create(Int32 identifier) => new MarkI(identifier);
    }
}
