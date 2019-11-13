using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace RoverSim.WinFormsClient
{
    public partial class WorkForm : Form
    {
        private Boolean running = false;
        private CompletedSimulation _renderSim;
        private IAiFactory _renderAiFactory;
        private Int32 _completedCount;

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly IReadOnlyList<IAiFactory> _aiFactories;
        private readonly IReadOnlyDictionary<String, ILevelGenerator> _levelGenerators;

        private WorkManager _workManager;

        public WorkForm(IReadOnlyList<IAiFactory> aiFactories, IReadOnlyDictionary<String, ILevelGenerator> levelGenerators)
        {
            _aiFactories = aiFactories ?? throw new ArgumentNullException(nameof(aiFactories));
            _levelGenerators = levelGenerators ?? throw new ArgumentNullException(nameof(levelGenerators));

            InitializeComponent();

            foreach (var ai in aiFactories)
                AiList.Items.Add(ai.Name, ai.Name, 0);
            AiList.SelectedIndices.Add(AiList.Items.Count - 1);

            foreach (var generator in _levelGenerators)
                LevelGeneratorName.Items.Add(generator.Key.ToString());
            LevelGeneratorName.SelectedIndex = 0;
        }

        private async void SimulateButton_Click(object sender, EventArgs e)
        {
            if (!running)
                running = true;
            else
                return;
            
            if (!Int32.TryParse(RunCount.Text, out Int32 runCount)) return;

            if (_workManager == null)
                _workManager = new WorkManager(SimulationParameters.Default);

            List<IAiFactory> selectedAis = _aiFactories.Where(t => AiList.SelectedItems.ContainsKey(t.Name)).ToList();
            ILevelGenerator selectedLevelGenerator = _levelGenerators[LevelGeneratorName.Text];

            var progress = new Progress<Int32>(UpdateProgress);
            _completedCount = 0;
            _stopwatch.Reset();
            UpdateStatus();

            Timer.Start();
            _stopwatch.Start();
            (var results, var worstSim, var worstAi) = await _workManager.Simulate(selectedAis, selectedLevelGenerator, runCount, progress);
            _stopwatch.Stop();
            Timer.Stop();

            UpdateStatus();

            _renderSim = worstSim;
            _renderAiFactory = worstAi;
            if (_renderSim != null && _renderSim.HasError)
                MessageBox.Show("ERROR");

            foreach (var aiFactory in results.Keys)
            {
                var stat = results[aiFactory];
                if (Results.Rows.Count == 0)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    Results.Rows.Add(row);
                }

                (var meanMoves, var meanPower, var meanSamples, var sampleStdDev) = stat;
                Results.Rows[0].SetValues(aiFactory.Name, meanMoves.ToString("F2"), meanPower.ToString("F2"), meanSamples.ToString("F2"), sampleStdDev.ToString("F2"));
            }
            running = false;
        }

        private void UpdateProgress(Int32 completionCount) => _completedCount += completionCount;

        private void UpdateStatus()
        {
            TimeUsed.Text = _stopwatch.Elapsed.TotalSeconds.ToString();
            Completed.Text = _completedCount.ToString();
        }

        private void OpenRender_Click(object sender, EventArgs e)
        {
            if (_renderSim == null)
                return;

#pragma warning disable IDE0067 // Dispose objects before losing scope
            RenderForm form = new RenderForm(_renderSim, _renderAiFactory);
#pragma warning restore IDE0067 // Dispose objects before losing scope
            form.Show();
        }

        private void Timer_Tick(Object sender, EventArgs e) => UpdateStatus();
    }
}
