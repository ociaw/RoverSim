using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using RoverSim.Rendering;

namespace RoverSim.WinFormsClient
{
    internal partial class RenderForm : Form, IDisposable
    {
        private delegate void UpdateUICallBack(VisibleState state, Int32 movesLeft, Int32 power, Int32 samplesSent);

        private VisibleState _state;

        public RenderForm(CompletedSimulation demoResult, IAiFactory demoAi)
        {
            DemoResult = demoResult ?? throw new ArgumentNullException(nameof(demoResult));
            DemoAi = demoAi ?? throw new ArgumentNullException(nameof(demoAi));
            InitializeComponent();
        }

        public CompletedSimulation DemoResult { get; }

        public IAiFactory DemoAi { get; }

        private void GlControl1_Load(object sender, EventArgs e)
        {
            beginRender.Enabled = true;
            GL.ClearColor(Color.SkyBlue);
            SetupViewport();
        }

        private void SetupViewport()
        {
            Int32 w = glControl1.Width;
            Int32 h = glControl1.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, h, 0, -1, 1); // Top-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        private async void BeginRender_Click(object sender, EventArgs e)
        {
            beginRender.Enabled = false;

            IAi ai = DemoAi.Create(DemoResult.AiIdentifier, DemoResult.Parameters);
            Level originalLevel = DemoResult.OriginalLevel;
            MutableLevel workingLevel = originalLevel.AsMutable();
            using (var source = new System.Threading.CancellationTokenSource())
            {
                IRover rover = new ReportingRover(
                    new Rover(workingLevel, DemoResult.Parameters),
                    new Progress<TerrainUpdate>(UpdateTerrain),
                    new Progress<PositionUpdate>(UpdateRoverPosition),
                    new Progress<StatsUpdate>(UpdateStats),
                    source.Token
                );
                Simulation sim = new Simulation(originalLevel, DemoResult.Parameters, ai, rover);
                _state = VisibleState.GenerateBlank(originalLevel.BottomRight, rover.Position);

                try
                {
                    await Task.Run(sim.Simulate, source.Token);
                }
                catch (OperationCanceledException)
                {
                    // Ignore this exception, since it'll only happen when we've already closed the form
                }
                catch (OutOfPowerOrMovesException)
                {
                    // This is to be expected if an AI doesn't keep track of their power or moves
                }
            }
        }

        private void UpdateTerrain(TerrainUpdate update)
        {
            if (IsDisposed)
                return;

            _state.UpdateTerrain(update);
            Render(_state);
        }

        private void UpdateRoverPosition(PositionUpdate update)
        {
            if (IsDisposed)
                return;

            _state.UpdateRoverPos(update);
            Render(_state);
        }

        private void UpdateStats(StatsUpdate update)
        {
            MovesLeftText.Text = update.MovesLeft.ToString();
            PowerLeftText.Text = update.Power.ToString();
            SamplesSentText.Text = update.TransmitedCount.ToString();
        }

        private void Render(VisibleState state)
        {
            Int32 roverX = state.RoverPosition.X;
            Int32 roverY = state.RoverPosition.Y;
            Int32 viewWidth = glControl1.Width;
            Int32 viewHeight = glControl1.Height;
            Int32 widthMultiplier = viewWidth / state.Width;
            Int32 heightMultiplier = viewHeight / state.Height;
            for (Int16 i = 0; i < state.Width; i++)
            {
                for (Int16 j = 0; j < state.Height; j++)
                {
                    DrawTile(i, j, state[i, j], widthMultiplier, heightMultiplier);
                }
            }

            DrawRover(roverX, roverY, widthMultiplier, heightMultiplier);

            glControl1.SwapBuffers();
        }

        private void DrawTile(Int32 x, Int32 y, TerrainType terrain, Int32 tileWidth, Int32 tileHeight)
        {
            GL.Color3(GetColorForTerrain(terrain));
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x * tileWidth, y * tileHeight);
            GL.Vertex2(x * tileWidth, y * tileHeight + tileHeight);
            GL.Vertex2(x * tileWidth + tileWidth, y * tileHeight + tileHeight);
            GL.Vertex2(x * tileWidth + tileWidth, y * tileHeight);
            GL.End();
        }

        private void DrawRover(Int32 roverX, Int32 roverY, Int32 tileWidth, Int32 tileHeight)
        {
            GL.Color3(Color.LightGreen);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(roverX * tileWidth + 2, roverY * tileHeight + 2);
            GL.Vertex2(roverX * tileWidth + 2, roverY * tileHeight + tileHeight - 2);
            GL.Vertex2(roverX * tileWidth + tileWidth - 2, roverY * tileHeight + tileHeight - 2);
            GL.Vertex2(roverX * tileWidth + tileWidth - 2, roverY * tileHeight + 2);
            GL.End();
        }

        private static Color GetColorForTerrain(TerrainType terrain)
        {
            switch (terrain)
            {
                case TerrainType.Impassable:
                    return Color.Black;
                case TerrainType.Rough:
                    return Color.Red;
                case TerrainType.SampledRough:
                    return Color.Brown;
                case TerrainType.SampledSmooth:
                    return Color.DarkGray;
                case TerrainType.Smooth:
                    return Color.LightGray;
                case TerrainType.Unknown:
                    return Color.LightGoldenrodYellow;
                default:
                    return Color.Blue;
            }
        }

        private void RenderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
