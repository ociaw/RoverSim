using System;
using System.Collections.Generic;

namespace RoverSim.BlazorClient
{
    public sealed class LevelProvider
    {
        private readonly Dictionary<String, Func<Dictionary<String, String>, ILevelGenerator>> _functions;

        private static readonly String FallbackKey = "Default";
        private static readonly Func<Dictionary<String, String>, ILevelGenerator> FallbackGenerator = CreateDefault;

        public LevelProvider()
        {
            // Add new AIs here
            _functions = new Dictionary<String, Func<Dictionary<String, String>, ILevelGenerator>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Default", CreateDefault },
                { "Maze", CreateMaze }
            };
        }

        public ProtoLevel CreateLevel(Dictionary<String, String> query, SimulationParameters parameters)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            String aiKey = query.GetValue("level-key", FallbackKey);
            Int32 seed = query.GetNonNegative("level-seed", 123);

            if (!_functions.TryGetValue(aiKey, out var func))
                func = FallbackGenerator;

            var generator = func(query);
            return new ProtoLevel(parameters, generator, seed);
        }

        private static ILevelGenerator CreateDefault(Dictionary<String, String> query) => new DefaultLevelGenerator();

        private static ILevelGenerator CreateMaze(Dictionary<String, String> query) => new MazeGenerator();
    }
}
