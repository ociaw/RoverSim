using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            RenderControl.VisibleState = VisibleState.GenerateBlank(DemoResult.Parameters);
        }

        public CompletedSimulation DemoResult { get; }

        public IAiFactory DemoAi { get; }

        public String OutputDirectory { get; }

        public String OutputBase => Path.Combine(OutputDirectory, $"frame-{DemoResult.ProtoLevel.Seed}-");

        private void BeginRender_Click(object sender, EventArgs e)
        {
            IAi ai = DemoAi.Create(DemoResult.Parameters);
            Level level = DemoResult.ProtoLevel.Generate();

            _rover = new Rover(level, DemoResult.Parameters);
            _stats = RoverStats.Create(DemoResult.Parameters);
            _state = VisibleState.GenerateBlank(level.BottomRight, _rover.Position);
            _actionEnumerator = ai.Simulate(_rover.Accessor).GetEnumerator();
            beginRender.Enabled = false;

            _state.Apply(new Update(terrain: _rover.Adjacent));
            RenderControl.VisibleState = _state;
            Render();
            UpdateTimer.Interval = 100;
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

        private void Render() => RenderControl.Invalidate();

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
            
            using Bitmap bitmap = new Bitmap(width, height);
            using Graphics surface = Graphics.FromImage(bitmap);

            Update update = new Update(terrain: rover.Adjacent);
            Int32 frameIndex = 0;
            do
            {
                if (!state.Apply(update))
                    continue;
                
                GdiRenderer.Draw(surface, width, height, state);
                String suffix = frameIndex.ToString().PadLeft(filenameDigits, '0');
                String filename = fileBase + suffix + ".png";
                bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                frameIndex++;
            }
            while (actionEnumerator.MoveNext() && rover.Perform(actionEnumerator.Current, out update));
        }
    }
}
