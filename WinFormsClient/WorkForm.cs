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
        private ILevelGenerator _renderLevelGenerator;

        private readonly IEnumerable<IAiFactory> aiFactories;

        public WorkForm()
        {
            InitializeComponent();

            aiFactories = WorkManager.GetAIs();

            foreach (var ai in aiFactories)
            {
                AiList.Items.Add(ai.Name, ai.Name, 0);
            }
        }

        private async void SimulateButton_Click(object sender, EventArgs e)
        {
            if (!running)
                running = true;
            else
                return;
            
            if (!Int32.TryParse(RunCount.Text, out Int32 runCount)) return;

            WorkManager manager = new WorkManager();
            
            List<IAiFactory> selectedAis = aiFactories.Where(t => AiList.SelectedItems.ContainsKey(t.Name)).ToList();
            ILevelGenerator selectedLevelGenerator = new OpenCheckingGenerator(new DefaultLevelGenerator(), 6);
            TimeUsed.Text = "Working...";
            var stopwatch = Stopwatch.StartNew();
            (var results, var worstSim, var worstAi) = await manager.Simulate(selectedAis, selectedLevelGenerator, runCount);
            stopwatch.Stop();
            TimeUsed.Text = stopwatch.Elapsed.TotalSeconds.ToString();

            _renderSim = worstSim;
            _renderAiFactory = worstAi;
            _renderLevelGenerator = selectedLevelGenerator;
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

        private void OpenRender_Click(object sender, EventArgs e)
        {
            if (_renderSim == null)
                return;

#pragma warning disable IDE0067 // Dispose objects before losing scope
            RenderForm form = new RenderForm(_renderSim, _renderAiFactory, _renderLevelGenerator);
#pragma warning restore IDE0067 // Dispose objects before losing scope
            form.Show();
        }
    }
}
