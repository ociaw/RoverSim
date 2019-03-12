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
        private delegate void UpdateUICallBack(TerrainType[,] terrain, Int32 roverX, Int32 roverY, Int32 movesLeft, Int32 power, Int32 samplesSent);
        private delegate void ToggleUICallBack(System.Boolean state);

        private Task workingTask;
        private readonly System.Threading.CancellationTokenSource source = new System.Threading.CancellationTokenSource();
        private System.Threading.CancellationToken token;

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

        private void BeginRender_Click(object sender, EventArgs e)
        {
            beginRender.Enabled = false;

            IAi ai = DemoAi.Create(DemoResult.Ai.Identifier);
            Simulation cleanSim = DemoResult.Simulation.CloneClean(ai);
            TerrainType[,] visible = GenerateBlank(cleanSim.Level.Width, cleanSim.Level.Height);

            token = source.Token;
            workingTask = Task.Factory.StartNew(() =>
            {
                token.ThrowIfCancellationRequested();

                UpdateUICallBack uiUpdate = UpdateUI;
                ToggleUICallBack method = ToggleUI;
                while (true)
                {
                    try
                    {
                        bool isHalted = cleanSim.Step();
                        UpdateVisible(visible, cleanSim.Level, cleanSim.Rover);

                        if (isHalted)
                            break;
                    }
                    catch (OutOfPowerOrMovesException)
                    {
                        break;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                    token.ThrowIfCancellationRequested();
                    // TODO: Handle premature window close
                    Invoke(uiUpdate, new Object[] { visible, cleanSim.Rover.PosX, cleanSim.Rover.PosY, cleanSim.Rover.MovesLeft, cleanSim.Rover.Power, cleanSim.Rover.SamplesTransmitted });
                }
                token.ThrowIfCancellationRequested();
                Invoke(uiUpdate, new Object[] { visible, cleanSim.Rover.PosX, cleanSim.Rover.PosY, cleanSim.Rover.MovesLeft, cleanSim.Rover.Power, cleanSim.Rover.SamplesTransmitted });
                Invoke(method, new Object[] { true });
            }, source.Token);
        }

        private void ToggleUI(System.Boolean state)
        {
            beginRender.Enabled = state;
        }

        private void UpdateUI(TerrainType[,] terrain, Int32 roverX, Int32 roverY, Int32 movesLeft, Int32 power, Int32 samplesSent)
        {
            MovesLeftText.Text = movesLeft.ToString();
            PowerLeftText.Text = power.ToString();
            SamplesSentText.Text = samplesSent.ToString();

            Render(terrain, roverX, roverY);
        }

        private void Render(TerrainType[,] terrain, Int32 roverX, Int32 roverY)
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Int32 terrainWidth = terrain.GetLength(0);
            Int32 terrainHeight = terrain.GetLength(1);
            Int32 widthMultiplier = w / terrainWidth;
            Int32 heightMultiplier = h / terrainHeight;
            for (Int16 i = 0; i < terrainWidth; i++)
            {
                for (Int16 j = 0; j < terrainHeight; j++)
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
            source.Cancel();
        }

        private TerrainType[,] GenerateBlank(Int32 width, Int32 height)
        {
            TerrainType[,] terrain = new TerrainType[width, height];
            for (Byte i = 1; i < width - 1; i++)
            {
                for (Byte j = 1; j < height - 1; j++)
                {
                    terrain[i, j] = TerrainType.Unknown;
                }
            }
            return terrain;
        }

        private void UpdateVisible(TerrainType[,] visible, Level level, Rover rover)
        {
            Int32 roverX = rover.PosX;
            Int32 roverY = rover.PosY;
            TerrainType[,] actual = level.Terrain;

            visible[roverX, roverY] = actual[roverX, roverY];
            visible[roverX - 1, roverY] = actual[roverX - 1, roverY];
            visible[roverX + 1, roverY] = actual[roverX + 1, roverY];
            visible[roverX, roverY - 1] = actual[roverX, roverY - 1];
            visible[roverX, roverY + 1] = actual[roverX, roverY + 1];
        }
    }
}
