using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace RoverSim.BlazorClient
{
    public sealed class LevelProvider
    {
        private readonly Dictionary<String, Func<Dictionary<String, StringValues>, SimulationParameters, ILevelGenerator>> _functions;

        private static readonly String FallbackKey = "Default";
        private static readonly Func<Dictionary<String, StringValues>, SimulationParameters, ILevelGenerator> FallbackGenerator = CreateDefault;

        public LevelProvider()
        {
            // Add new AIs here
            _functions = new Dictionary<String, Func<Dictionary<String, StringValues>, SimulationParameters, ILevelGenerator>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Default", CreateDefault },
                { "Maze", CreateMaze }
            };
        }

        public Level CreateLevel(Dictionary<String, StringValues> query, SimulationParameters parameters)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            String aiKey = query.GetFirstValue("level-key", FallbackKey);
            Int32 seed = query.GetFirstNonNegative("level-seed", 123);

            if (!_functions.TryGetValue(aiKey, out var func))
                func = FallbackGenerator;

            var generator = func(query, parameters);

            Level level;
            do
            {
                level = generator.Generate(parameters, seed);
            }
            while (level == null);
            return level;
        }

        private static ILevelGenerator CreateDefault(Dictionary<String, StringValues> query, SimulationParameters parameters) => new DefaultLevelGenerator();

        private static ILevelGenerator CreateMaze(Dictionary<String, StringValues> query, SimulationParameters parameters) => new MazeGenerator();
    }
}
