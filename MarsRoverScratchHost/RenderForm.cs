using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using MarsRoverScratch;

namespace MarsRoverScratchHost
{
    internal partial class RenderForm : Form
    {
        private delegate void UpdateUICallBack(RenderData renderData, Int32 movesLeft, Int32 power, Int32 samplesSent);

        private readonly System.Threading.CancellationTokenSource _source = new System.Threading.CancellationTokenSource();

        private RenderData _renderData;

        public RenderForm(SimulationResult demoResult, IAiFactory demoAi)
        {
            DemoResult = demoResult ?? throw new ArgumentNullException(nameof(demoResult));
            DemoAi = demoAi ?? throw new ArgumentNullException(nameof(demoAi));
            InitializeComponent();
        }

        public SimulationResult DemoResult { get; }

        public IAiFactory DemoAi { get; }

        private void GlControl1_Load(object sender, EventArgs e)
        {
            beginRender.Enabled = true;
            GL.ClearColor(Color.SkyBlue);
            SetupViewport();
        }

        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, h, 0, -1, 1); // Top-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        private async void BeginRender_Click(object sender, EventArgs e)
        {
            beginRender.Enabled = false;

            IAi ai = DemoAi.Create(DemoResult.Ai.Identifier);
            Level originalLevel = DemoResult.Simulation.OriginalLevel.Clone();
            Level _workingLevel = originalLevel.Clone();
            IRover rover = new ReportingRover(
                new Rover(_workingLevel),
                new Progress<TerrainUpdate>(UpdateTerrain),
                new Progress<PositionUpdate>(UpdateRoverPosition),
                new Progress<StatsUpdate>(UpdateStats),
                _source.Token
            );
            Simulation sim = new Simulation(originalLevel, ai, rover);
            _renderData = RenderData.GenerateBlank(originalLevel.Width, originalLevel.Height, rover.PosX, rover.PosY);

            try
            {
                await Task.Run
                (() =>
                    {
                        sim.Simulate();
                    }
                , _source.Token);
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

        private void UpdateTerrain(TerrainUpdate update)
        {
            if (IsDisposed)
                return;

            _renderData.UpdateTerrain(update);
            Render(_renderData);
        }

        private void UpdateRoverPosition(PositionUpdate update)
        {
            if (IsDisposed)
                return;

            _renderData.UpdateRoverPos(update);
            Render(_renderData);
        }

        private void UpdateStats(StatsUpdate update)
        {
            MovesLeftText.Text = update.MovesLeft.ToString();
            PowerLeftText.Text = update.Power.ToString();
            SamplesSentText.Text = update.TransmitedCount.ToString();
        }

        private void Render(RenderData renderData)
        {
            var terrain = renderData.Terrain;
            Int32 roverX = renderData.RoverX;
            Int32 roverY = renderData.RoverY;
            int viewWidth = glControl1.Width;
            int viewHeight = glControl1.Height;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Int32 widthMultiplier = viewWidth / renderData.Width;
            Int32 heightMultiplier = viewHeight / renderData.Height;
            for (Int16 i = 0; i < renderData.Width; i++)
            {
                for (Int16 j = 0; j < renderData.Height; j++)
                {
                    switch (terrain[i, j])
                    {
                        case TerrainType.Impassable:
                            GL.Color3(Color.Black);
                            break;
                        case TerrainType.Rough:
                            GL.Color3(Color.Red);
                            break;
                        case TerrainType.SampledRough:
                            GL.Color3(Color.Brown);
                            break;
                        case TerrainType.SampledSmooth:
                            GL.Color3(Color.DarkGray);
                            break;
                        case TerrainType.Smooth:
                            GL.Color3(Color.LightGray);
                            break;
                        case TerrainType.Unknown:
                            GL.Color3(Color.LightGoldenrodYellow);
                            break;
                        default:
                            GL.Color3(Color.Blue);
                            break;
                    }
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex2(i * widthMultiplier, j * heightMultiplier);
                    GL.Vertex2(i * widthMultiplier, j * heightMultiplier + heightMultiplier);
                    GL.Vertex2(i * widthMultiplier + widthMultiplier, j * heightMultiplier + heightMultiplier);
                    GL.Vertex2(i * widthMultiplier + widthMultiplier, j * heightMultiplier);
                    GL.End();
                }
            }

            GL.Color3(Color.LightGreen);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(roverX * widthMultiplier + 2, roverY * heightMultiplier + 2);
            GL.Vertex2(roverX * widthMultiplier + 2, roverY * heightMultiplier + heightMultiplier - 2);
            GL.Vertex2(roverX * widthMultiplier + widthMultiplier - 2, roverY * heightMultiplier + heightMultiplier - 2);
            GL.Vertex2(roverX * widthMultiplier + widthMultiplier - 2, roverY * heightMultiplier + 2);
            GL.End();

            glControl1.SwapBuffers();
        }

        private void RenderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _source.Cancel();
        }
    }
}
