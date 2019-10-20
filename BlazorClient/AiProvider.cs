using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using RoverSim.Ais;
using RoverSim.ScratchAis;

namespace RoverSim.BlazorClient
{
    public sealed class AiProvider
    {
        private readonly Dictionary<String, Func<Dictionary<String, StringValues>, SimulationParameters, IAi>> _functions;

        private static readonly String FallbackKey = "FixedState";
        private static readonly Func<Dictionary<String, StringValues>, SimulationParameters, IAi> FallbackAi = CreateFixedState;

        public AiProvider()
        {
            // Add new AIs here
            _functions = new Dictionary<String, Func<Dictionary<String, StringValues>, SimulationParameters, IAi>>(StringComparer.OrdinalIgnoreCase)
            {
                { "FixedState", CreateFixedState },
                { "Random", CreateRandom },
                { "IntelligentRandom", CreateIntelligentRandom },
                { "MarkI", CreateMarkI },
                { "MarkII", CreateMarkII }
            };
        }

        public IAi CreateAi(Dictionary<String, StringValues> query, SimulationParameters parameters)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            String aiKey = query.GetFirstValue("ai-key", FallbackKey);

            if (!_functions.TryGetValue(aiKey, out var func))
                func = FallbackAi;

            return func(query, parameters);
        }

        private static IAi CreateFixedState(Dictionary<String, StringValues> query, SimulationParameters parameters)
        {
            Int32 memory = query.GetFirstNonNegative("ai-memory", 5);

            return new FixedStateAi(1, SimulationParameters.Default, memory);
        }

        private static IAi CreateRandom(Dictionary<String, StringValues> query, SimulationParameters parameters)
        {
            Int32 aiSeed = query.GetFirstValue("ai-seed", 1);
            var aiFactory = new RandomAiFactory();
            return aiFactory.Create(aiSeed, parameters);
        }

        private static IAi CreateIntelligentRandom(Dictionary<String, StringValues> query, SimulationParameters parameters)
        {
            Int32 aiSeed = query.GetFirstValue("ai-seed", 1);
            var aiFactory = new IntelligentRandomAiFactory();
            return aiFactory.Create(aiSeed, parameters);
        }

        private static IAi CreateMarkI(Dictionary<String, StringValues> query, SimulationParameters parameters)
        {
            Int32 aiSeed = query.GetFirstValue("ai-seed", 1);
            var aiFactory = new MarkIFactory();
            return aiFactory.Create(aiSeed, parameters);
        }

        private static IAi CreateMarkII(Dictionary<String, StringValues> query, SimulationParameters parameters)
        {
            Int32 aiSeed = query.GetFirstValue("ai-seed", 1);
            var aiFactory = new MarkIIFactory();
            return aiFactory.Create(aiSeed, parameters);
        }
    }
}
