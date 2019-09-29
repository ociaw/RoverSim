using System;
using System.Collections;
using System.Collections.Generic;

namespace RoverSim.BlazorClient
{
    public sealed class AiProvider : IEnumerable<IAiFactory>
    {
        private readonly Dictionary<String, IAiFactory> _ais = new Dictionary<String, IAiFactory>();

        public void Add(IAiFactory ai)
        {
            if (ai == null)
                throw new ArgumentNullException(nameof(ai));

            _ais.Add(ai.Name, ai);
        }

        public IAiFactory this[String name] => _ais[name];

        public IEnumerable<IAiFactory> GetAis() => _ais.Values;

        IEnumerator<IAiFactory> IEnumerable<IAiFactory>.GetEnumerator() => _ais.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _ais.Values.GetEnumerator();
    }
}
