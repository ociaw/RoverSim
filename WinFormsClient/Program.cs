using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RoverSim.Ais;
using RoverSim.ScratchAis;

namespace RoverSim.WinFormsClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WorkForm(GetAIs(), GetLevelGenerators()));
        }

        private static IReadOnlyList<IAiFactory> GetAIs()
            => new List<IAiFactory>
            {
                new RandomAiFactory(),
                new IntelligentRandomAiFactory(),
                new MarkIFactory(),
                new MarkIIFactory(),
                new LimitedStateAiFactory(),
                new PathfindingAiFactory()
            };

        private static IReadOnlyDictionary<String, ILevelGenerator> GetLevelGenerators()
            => new Dictionary<String, ILevelGenerator>
            {
                { "Default", new OpenCheckingGenerator(new DefaultLevelGenerator(), 6) },
                { "Maze", new MazeGenerator() }
            };
    }
}
