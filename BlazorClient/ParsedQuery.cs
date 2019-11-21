using System;
using System.Collections.Generic;

namespace RoverSim.BlazorClient
{
    public sealed class ParsedQuery
    {
        private readonly AiProvider _aiProvider = new AiProvider();
        private readonly LevelProvider _levelProvider = new LevelProvider();
        private readonly Dictionary<String, String> _query;

        private ParsedQuery(SimulationParameters parameters, Dictionary<String, String> query)
        {
            Parameters = parameters;
            _query = query;
        }

        public SimulationParameters Parameters { get; }

        public IAi CreateAi() => _aiProvider.CreateAi(_query, Parameters);

        public ProtoLevel CreateLevel() => _levelProvider.CreateLevel(_query, Parameters);

        public static ParsedQuery FromDictionary(Dictionary<String, String> query)
        {
            SimulationParameters parameters = ReadParameters(query);
            return new ParsedQuery(parameters, query);
        }

        private static SimulationParameters ReadParameters(Dictionary<String, String> query)
        {
            SimulationParameters fallback = SimulationParameters.Default;
            Int32 width = query.GetPositive("sim-width", fallback.BottomRight.X + 1);
            Int32 height = query.GetPositive("sim-height", fallback.BottomRight.Y + 1);
            Position bottomRight = new Position(width - 1, height - 1);
            return new SimulationParameters(bottomRight);
        }
    }
}
