using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using CsvHelper;
using RoverSim;

namespace MarsRoverScratchHost
{
    public partial class WorkForm : Form
    {
        delegate void SetTextCallback(String text);

        private Boolean running = false;
        private Int64 startTick = 0;
        private Int64 endTick = 0;
        IList<SimulationResult> _results = new List<SimulationResult>();

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

        private void TasksCompleted(IList<SimulationResult> results)
        {
            endTick = DateTime.Now.Ticks;
            _results = results;
            SaveResults();
            if (results.Any(r => r.Error == true))
            {
                MessageBox.Show("ERROR");
            }
            running = false;
        }

        private void ActionButton2_Click(object sender, EventArgs e)
        {
            if (!running)
                running = true;
            else
                return;
            
            if (!Int32.TryParse(textBox1.Text, out Int32 runCount)) return;

            WorkManager manager = new WorkManager(TasksCompleted);
            
            List<IAiFactory> selectedAis = aiFactories.Where(t => listView1.SelectedItems.ContainsKey(t.Name)).ToList();
            startTick = DateTime.Now.Ticks;
            manager.StartTasks(selectedAis, runCount);
            timeUsed.Text = "Working...";
        }

        private void SaveResults()
        {
            String documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            TextWriter writer = new StreamWriter(documentsFolder + "\\MarsRoverScratch.csv");
            CsvWriter csv = new CsvWriter(writer);

            csv.WriteField("AI Type");
            csv.WriteField("Moves Left");
            csv.WriteField("Power Left");
            csv.WriteField("Samples Collected");
            csv.WriteField("Samples Processed");
            csv.WriteField("Samples Transmitted");
            csv.NextRecord();

            Dictionary<String, (Int32 moves, Int32 power, Int32 samples)> stats = new Dictionary<String, (Int32 moves, Int32 power, Int32 samples)>();
            foreach (SimulationResult result in _results)
            {
                csv.WriteField(result.AiName);
                csv.WriteField(result.Rover.MovesLeft.ToString());
                csv.WriteField(result.Rover.Power.ToString());
                csv.WriteField(result.Rover.SamplesCollected.ToString());
                csv.WriteField(result.Rover.SamplesProcessed.ToString());
                csv.WriteField(result.Rover.SamplesTransmitted.ToString());
                csv.NextRecord();
                if (stats.ContainsKey(result.AiName))
                {
                    var (moves, power, samples) = stats[result.AiName];
                    stats[result.AiName] = (moves + result.Rover.MovesLeft, power + result.Rover.Power, samples + result.Rover.SamplesTransmitted);
                }
                else
                {
                    stats[result.AiName] = (result.Rover.MovesLeft, result.Rover.Power, result.Rover.SamplesTransmitted);
                }
            }

            if (timeUsed.InvokeRequired)
            {
                SetTextCallback d = SetText;
                Invoke(d, (endTick - startTick).ToString());
            }
            else
            {
                timeUsed.Text = (endTick - startTick).ToString();
            }

            writer.Close();

            Double runCount = _results.Count;

            foreach (KeyValuePair<String, (Int32 moves, Int32 power, Int32 samples)> stat in stats)
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    dataGridView1.Rows.Add(row);
                }
                dataGridView1.Rows[0].SetValues(stat.Key.ToString(), (stat.Value.moves / runCount).ToString(), (stat.Value.power / runCount).ToString(), (stat.Value.samples / runCount).ToString());
            }
        }

        private void SetText(String text)
        {
            timeUsed.Text = text;
        }


        private void OpenRender_Click(object sender, EventArgs e)
        {
            if (_results.Count > 0)
            {
                SimulationResult result;

                var erroredResults = _results.Where(i => i.Error == true);
                if (erroredResults.Any())
                {
                    result = erroredResults.First();
                }
                else
                {
                    result = _results.OrderBy(r => r.Rover.SamplesTransmitted).First();
                }

                RenderForm form = new RenderForm(result, aiFactories.Single(f => f.Name == result.AiName));
                
                form.Show();
            }

        }
    }
}
