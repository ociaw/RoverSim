using System;

namespace RoverSim.AvaloniaHost.ViewModels
{
    public sealed class RenderViewModel : ViewModelBase
    {
        public RenderViewModel(IAiFactory ai, CompletedSimulation simulation)
        {
            Ai = ai ?? throw new ArgumentNullException(nameof(ai));
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
            Tiles = Simulation.OriginalLevel.AsMutable().Terrain;
            RoverX = Simulation.Parameters.InitialX;
            RoverY = Simulation.Parameters.InitialY;
        }

        public IAiFactory Ai { get; }

        public CompletedSimulation Simulation { get; }

        public TerrainType[,] Tiles { get; }

        public Int32 RoverX { get; }

        public Int32 RoverY { get; }
    }
}
