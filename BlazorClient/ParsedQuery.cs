using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace RoverSim.BlazorClient
{
    public sealed class ParsedQuery
    {
        private readonly AiProvider _aiProvider = new AiProvider();
        private readonly LevelProvider _levelProvider = new LevelProvider();
        private readonly Dictionary<String, StringValues> _query;

        private ParsedQuery(SimulationParameters parameters, Dictionary<String, StringValues> query)
        {
            Parameters = parameters;
            _query = query;
        }

        public SimulationParameters Parameters { get; }

        public IAi CreateAi() => _aiProvider.CreateAi(_query, Parameters);

        public ProtoLevel CreateLevel() => _levelProvider.CreateLevel(_query, Parameters);

        public static ParsedQuery FromDictionary(Dictionary<String, StringValues> query)
        {
            SimulationParameters parameters = ReadParameters(query);
            return new ParsedQuery(parameters, query);
        }

        private static SimulationParameters ReadParameters(Dictionary<String, StringValues> query)
        {
            SimulationParameters fallback = SimulationParameters.Default;
            Int32 width = query.GetFirstPositive("sim-width", fallback.BottomRight.X + 1);
            Int32 height = query.GetFirstPositive("sim-height", fallback.BottomRight.Y + 1);
            Position bottomRight = new Position(width - 1, height - 1);
            return new SimulationParameters(bottomRight);
        }
    }
}
