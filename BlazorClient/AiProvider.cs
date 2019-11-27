using System;
using System.Collections.Generic;
using RoverSim.Ais;
using RoverSim.ScratchAis;

namespace RoverSim.BlazorClient
{
    public sealed class AiProvider
    {
        private readonly Dictionary<String, Func<Dictionary<String, String>, IAiFactory>> _functions;

        private static readonly String FallbackKey = "LimitedState";
        private static readonly Func<Dictionary<String, String>, IAiFactory> FallbackAiFactory = CreateLimitedState;

        public AiProvider()
        {
            // Add new AIs here
            _functions = new Dictionary<String, Func<Dictionary<String, String>, IAiFactory>>(StringComparer.OrdinalIgnoreCase)
            {
                { "FixedState", CreateLimitedState }, // TODO: This name is obsolete, remove later.
                { "LimitedState", CreateLimitedState },
                { "Pathfinding", CreatePathfinding },
                { "Random", CreateRandom },
                { "IntelligentRandom", CreateIntelligentRandom },
                { "MarkI", CreateMarkI },
                { "MarkII", CreateMarkII }
            };
        }

        public IAi CreateAi(Dictionary<String, String> query, SimulationParameters parameters)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            String aiKey = query.GetValue("ai-key", FallbackKey);

            if (!_functions.TryGetValue(aiKey, out var factory))
                factory = FallbackAiFactory;
            return factory(query).Create(parameters);
        }

        private static IAiFactory CreateLimitedState(Dictionary<String, String> query)
        {
            Int32 memory = query.GetNonNegative("ai-memory", 5);
            return new LimitedStateAiFactory(memory);
        }

        private static IAiFactory CreatePathfinding(Dictionary<String, String> query) => new PathfindingAiFactory();

        private static IAiFactory CreateRandom(Dictionary<String, String> query)
        {
            Int32 aiSeed = query.GetValue("ai-seed", 1);
            return new RandomAiFactory() { Seed = aiSeed };
        }

        private static IAiFactory CreateIntelligentRandom(Dictionary<String, String> query)
        {
            Int32 aiSeed = query.GetValue("ai-seed", 1);
            return new IntelligentRandomAiFactory() { Seed = aiSeed };
        }

        private static IAiFactory CreateMarkI(Dictionary<String, String> query) => new MarkIFactory();

        private static IAiFactory CreateMarkII(Dictionary<String, String> query) => new MarkIIFactory();
    }
}
