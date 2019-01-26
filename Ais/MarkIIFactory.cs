using System;

namespace MarsRoverScratch.Ais
{
    public sealed class MarkIIFactory : IAiFactory
    {
        public String Name => "Mark II";

        public IAi Create(Int32 identifier) => new MarkII(identifier);
    }
}
