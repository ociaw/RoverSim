using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using CsvHelper;

namespace RoverSim.WinFormsClient
{
    public partial class WorkForm : Form
    {
        private Boolean running = false;
        private CompletedSimulation _renderSim;
        private IAiFactory _renderAiFactory;

        private readonly IEnumerable<IAiFactory> aiFactories;

        public WorkForm()
        {
            InitializeComponent();

            aiFactories = WorkManager.GetAIs();

            foreach (var ai in aiFactories)
            {
                listView1.Items.Add(ai.Name, ai.Name, 0);
            }
        }

        private async void ActionButton2_Click(object sender, EventArgs e)
        {
            if (!running)
                running = true;
            else
                return;
            
            if (!Int32.TryParse(textBox1.Text, out Int32 runCount)) return;

            WorkManager manager = new WorkManager();
            
            List<IAiFactory> selectedAis = aiFactories.Where(t => listView1.SelectedItems.ContainsKey(t.Name)).ToList();
            timeUsed.Text = "Working...";
            var stopwatch = Stopwatch.StartNew();
            var results = await manager.Simulate(selectedAis, runCount);
            stopwatch.Stop();
            timeUsed.Text = stopwatch.Elapsed.TotalSeconds.ToString();

            (_renderSim, _renderAiFactory) = FindWorstSim(results);
            if (_renderSim != null && _renderSim.HasError)
                MessageBox.Show("ERROR");

            SaveResults(results);
            running = false;
        }

        private static (CompletedSimulation worstSim, IAiFactory worstAi) FindWorstSim(Dictionary<IAiFactory, List<CompletedSimulation>> results)
        {
            CompletedSimulation overallWorst = null;
            IAiFactory worstAi = null;

            foreach (var kvp in results)
            {
                var errored = kvp.Value.FirstOrDefault(sim => sim.HasError);
                if (errored != null)
                {
                    return (errored, kvp.Key);
                }

                CompletedSimulation worst = kvp.Value.OrderBy(sim => sim.Stats.SamplesTransmitted).FirstOrDefault();
                if (overallWorst == null || worst != null && worst.Stats.SamplesTransmitted < overallWorst.Stats.SamplesTransmitted)
                {
                    overallWorst = worst;
                    worstAi = kvp.Key;
                }
            }

            return (overallWorst, worstAi);
        }

        private void SaveResults(Dictionary<IAiFactory, List<CompletedSimulation>> results)
        {
            var aggregates = new Dictionary<String, (Double meanMoves, Double meanPower, Double meanSamples, Double sampleVariance)>();

            String documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            TextWriter writer = new StreamWriter(documentsFolder + "\\MarsRoverScratch.csv");
            using (CsvWriter csv = new CsvWriter(writer))
            {
                csv.WriteField("AI Type");
                csv.WriteField("Moves Left");
                csv.WriteField("Power Left");
                csv.WriteField("Samples Collected");
                csv.WriteField("Samples Processed");
                csv.WriteField("Samples Transmitted");
                csv.NextRecord();

                foreach (var kvp in results)
                {
                    String aiName = kvp.Key.Name;
                    Int64 moves = 0;
                    Int64 power = 0;
                    Int64 samples = 0;

                    // Used for calculating the standard deviation
                    Int32 count = 0;
                    Double meanSamples = 0;
                    Double mean2Samples = 0;

                    foreach (var sim in kvp.Value)
                    {
                        RoverStats stats = sim.Stats;

                        csv.WriteField(kvp.Key.Name);
                        csv.WriteField(stats.MovesLeft.ToString());
                        csv.WriteField(stats.Power.ToString());
                        csv.WriteField(stats.SamplesCollected.ToString());
                        csv.WriteField(stats.SamplesProcessed.ToString());
                        csv.WriteField(stats.SamplesTransmitted.ToString());
                        csv.NextRecord();

                        moves += stats.MovesLeft;
                        power += stats.Power;
                        samples += stats.SamplesTransmitted;

                        count++;
                        Double delta = stats.SamplesTransmitted - meanSamples;
                        meanSamples += delta / count;
                        Double delta2 = stats.SamplesTransmitted - meanSamples;
                        mean2Samples += delta * delta2;
                    }

                    Double sampleVariance = mean2Samples / (count - 1);
                    Double sampleStdDev = Math.Sqrt(sampleVariance);

                    aggregates[aiName] = (moves / (Double)count, power / (Double)count, meanSamples, sampleStdDev);
                }
            }

            foreach (KeyValuePair<String, (Double meanMoves, Double meanPower, Double meanSamples, Double sampleStdDev)> stat in aggregates)
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    dataGridView1.Rows.Add(row);
                }

                (var meanMoves, var meanPower, var meanSamples, var sampleStdDev) = stat.Value;
                dataGridView1.Rows[0].SetValues(stat.Key, meanMoves.ToString("F2"), meanPower.ToString("F2"), meanSamples.ToString("F2"), sampleStdDev.ToString("F2"));
            }
        }

        private void OpenRender_Click(object sender, EventArgs e)
        {
            if (_renderSim == null)
                return;

            RenderForm form = new RenderForm(_renderSim, _renderAiFactory);
            form.Show();
        }
    }
}
