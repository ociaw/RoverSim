using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace RoverSim.BlazorClient
{
    public sealed class GeneratorProvider
    {
        private readonly Dictionary<String, Func<Dictionary<String, StringValues>, SimulationParameters, ILevelGenerator>> _functions;

        private static readonly String FallbackKey = "Default";
        private static readonly Func<Dictionary<String, StringValues>, SimulationParameters, ILevelGenerator> FallbackGenerator = CreateDefault;

        public GeneratorProvider()
        {
            // Add new AIs here
            _functions = new Dictionary<String, Func<Dictionary<String, StringValues>, SimulationParameters, ILevelGenerator>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Default", CreateDefault },
                { "Maze", CreateMaze }
            };
        }

        public ILevelGenerator CreateGenerator(Dictionary<String, StringValues> query, SimulationParameters parameters)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            String aiKey = query.GetFirstValue("level-key", FallbackKey);

            if (!_functions.TryGetValue(aiKey, out var func))
                func = FallbackGenerator;

            return func(query, parameters);
        }

        private static ILevelGenerator CreateDefault(Dictionary<String, StringValues> query, SimulationParameters parameters)
        {
            Int32 seed = query.GetFirstNonNegative("level-seed", 123);
            var random = new Random(seed);
            var generator = new DefaultLevelGenerator(random);

            return generator;
        }

        private static ILevelGenerator CreateMaze(Dictionary<String, StringValues> query, SimulationParameters parameters)
        {
            Int32 seed = query.GetFirstNonNegative("level-seed", 123);
            var random = new Random(seed);
            var generator = new MazeGenerator(random);

            return generator;
        }
    }
}
