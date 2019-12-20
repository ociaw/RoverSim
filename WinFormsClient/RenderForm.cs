using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using RoverSim.Rendering;

namespace RoverSim.WinFormsClient
{
    internal partial class RenderForm : Form, IDisposable
    {
        private delegate void UpdateUICallBack(VisibleState state, Int32 movesLeft, Int32 power, Int32 samplesSent);

        private IEnumerator<RoverAction> _actionEnumerator;

        private Rover _rover;

        private RoverStats _stats;

        private VisibleState _state;

        public RenderForm(CompletedSimulation demoResult, IAiFactory demoAi)
        {
            DemoResult = demoResult ?? throw new ArgumentNullException(nameof(demoResult));
            DemoAi = demoAi ?? throw new ArgumentNullException(nameof(demoAi));
            OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), DemoAi.Name);
            InitializeComponent();
        }

        public CompletedSimulation DemoResult { get; }

        public IAiFactory DemoAi { get; }

        public String OutputDirectory { get; }

        public String OutputBase => Path.Combine(OutputDirectory, $"frame-{DemoResult.ProtoLevel.Seed}-");

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

        private void BeginRender_Click(object sender, EventArgs e)
        {
            IAi ai = DemoAi.Create(DemoResult.Parameters);
            Level level = DemoResult.ProtoLevel.Generate();

            _rover = new Rover(level, DemoResult.Parameters);
            _stats = RoverStats.Create(DemoResult.Parameters);
            _state = VisibleState.GenerateBlank(level.BottomRight, _rover.Position);
            _actionEnumerator = ai.Simulate(_rover.Accessor).GetEnumerator();
            beginRender.Enabled = false;

            UpdateTimer.Interval = 100;
            _state.Apply(new Update(terrain: _rover.Adjacent));
            Render();
            UpdateTimer.Start();
        }

        private void UpdateTimer_Tick(Object sender, EventArgs e)
        {
            Int32 delay;
            do
            {
                if (!_actionEnumerator.MoveNext() || !_rover.Perform(_actionEnumerator.Current, out Update update))
                {
                    UpdateTimer.Stop();
                    UpdateStats();
                    Render();
                    beginRender.Enabled = true;
                    _actionEnumerator.Dispose();
                    return;
                }

                _stats = _stats.Add(_actionEnumerator.Current, update);
                _state.Apply(update);

                delay = _actionEnumerator.Current.Instruction switch
                {
                    Instruction.Move => 75,
                    Instruction.CollectSample => 50,
                    _ => 0
                };
            }
            while (delay == 0);
            UpdateTimer.Interval = delay;

            UpdateStats();
            Render();
        }

        private void UpdateStats()
        {
            MovesLeftText.Text = _stats.MovesLeft.ToString();
            PowerLeftText.Text = _stats.Power.ToString();
            SamplesSentText.Text = _stats.SamplesTransmitted.ToString();
        }

        private void Render()
        {
            Int32 roverX = _state.RoverPosition.X;
            Int32 roverY = _state.RoverPosition.Y;
            Int32 viewWidth = glControl1.Width;
            Int32 viewHeight = glControl1.Height;
            Int32 widthMultiplier = viewWidth / _state.Width;
            Int32 heightMultiplier = viewHeight / _state.Height;
            for (Int16 i = 0; i < _state.Width; i++)
            {
                for (Int16 j = 0; j < _state.Height; j++)
                {
                    DrawTile(i, j, _state[i, j], widthMultiplier, heightMultiplier);
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
            return terrain switch
            {
                TerrainType.Impassable => Color.Black,
                TerrainType.Rough => Color.Red,
                TerrainType.SampledRough => Color.Brown,
                TerrainType.SampledSmooth => Color.DarkGray,
                TerrainType.Smooth => Color.LightGray,
                TerrainType.Unknown => Color.LightGoldenrodYellow,
                _ => Color.Blue,
            };
        }

        void IDisposable.Dispose() => _actionEnumerator.Dispose();

        private async void ExportButton_Click(Object sender, EventArgs e)
        {
            ExportButton.Enabled = false;
            await Task.Run(() => ExportFrames(DemoResult, DemoAi, OutputDirectory));
            ExportButton.Enabled = true;
        }

        private static void ExportFrames(CompletedSimulation sim, IAiFactory aiFactory, String outputDir)
        {
            const Int32 tileSize = 10;
            
            IAi ai = aiFactory.Create(sim.Parameters);
            Level level = sim.ProtoLevel.Generate();
            Rover rover = new Rover(level, sim.Parameters);
            VisibleState state = VisibleState.GenerateBlank(level.BottomRight, rover.Position);
            using var actionEnumerator = ai.Simulate(rover.Accessor).GetEnumerator();

            Int32 maxFrameCount = sim.Parameters.InitialMovesLeft;
            Int32 filenameDigits = (Int32)Math.Ceiling(Math.Log10(maxFrameCount)) + 1;
            
            String fileBase = Path.Combine(outputDir, $"frame-{sim.ProtoLevel.Seed}-");
            DirectoryInfo dir = new DirectoryInfo(outputDir);
            if (dir.Exists)
            {
                foreach (var file in dir.EnumerateFiles())
                    file.Delete();
            }
            else
            {
                dir.Create();
            }

            Int32 width = level.Width * tileSize;
            Int32 height = level.Height * tileSize;
            
            GdiRenderer gdiRenderer = new GdiRenderer(width, height);
            using Bitmap bitmap = new Bitmap(width, height);

            Update update = new Update(terrain: rover.Adjacent);
            Int32 frameIndex = 0;
            do
            {
                if (!state.Apply(update))
                    continue;
                
                using Graphics surface = Graphics.FromImage(bitmap);
                gdiRenderer.Draw(surface, state);
                String suffix = frameIndex.ToString().PadLeft(filenameDigits, '0');
                String filename = fileBase + suffix + ".png";
                bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                frameIndex++;
            }
            while (actionEnumerator.MoveNext() && rover.Perform(actionEnumerator.Current, out update));
        }
    }
}
