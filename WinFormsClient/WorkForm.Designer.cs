﻿namespace RoverSim.WinFormsClient
{
    partial class WorkForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Button OpenRenderer;
            this.RunCount = new System.Windows.Forms.TextBox();
            this.AiList = new System.Windows.Forms.ListView();
            this.Simulate = new System.Windows.Forms.Button();
            this.Results = new System.Windows.Forms.DataGridView();
            this.AiType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MovesLeft = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PowerLeft = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SamplesSent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleStdDev = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeUsed = new System.Windows.Forms.TextBox();
            this.TimeUsedLabel = new System.Windows.Forms.Label();
            this.CompletedLabel = new System.Windows.Forms.Label();
            this.Completed = new System.Windows.Forms.TextBox();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.RunCountLabel = new System.Windows.Forms.Label();
            this.LevelGeneratorName = new System.Windows.Forms.ComboBox();
            OpenRenderer = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Results)).BeginInit();
            this.SuspendLayout();
            // 
            // OpenRenderer
            // 
            OpenRenderer.Location = new System.Drawing.Point(369, 167);
            OpenRenderer.Name = "OpenRenderer";
            OpenRenderer.Size = new System.Drawing.Size(75, 23);
            OpenRenderer.TabIndex = 4;
            OpenRenderer.Text = "View";
            OpenRenderer.UseVisualStyleBackColor = true;
            OpenRenderer.Click += new System.EventHandler(this.OpenRender_Click);
            // 
            // RunCount
            // 
            this.RunCount.Location = new System.Drawing.Point(171, 169);
            this.RunCount.Name = "RunCount";
            this.RunCount.Size = new System.Drawing.Size(75, 20);
            this.RunCount.TabIndex = 1;
            this.RunCount.Text = "100000";
            // 
            // AiList
            // 
            this.AiList.HideSelection = false;
            this.AiList.LabelWrap = false;
            this.AiList.Location = new System.Drawing.Point(12, 12);
            this.AiList.Name = "AiList";
            this.AiList.Size = new System.Drawing.Size(341, 147);
            this.AiList.TabIndex = 2;
            this.AiList.UseCompatibleStateImageBehavior = false;
            this.AiList.View = System.Windows.Forms.View.List;
            // 
            // Simulate
            // 
            this.Simulate.Location = new System.Drawing.Point(252, 167);
            this.Simulate.Name = "Simulate";
            this.Simulate.Size = new System.Drawing.Size(101, 23);
            this.Simulate.TabIndex = 3;
            this.Simulate.Text = "Simulate";
            this.Simulate.UseVisualStyleBackColor = true;
            this.Simulate.Click += new System.EventHandler(this.SimulateButton_Click);
            // 
            // Results
            // 
            this.Results.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Results.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AiType,
            this.MovesLeft,
            this.PowerLeft,
            this.SamplesSent,
            this.SampleStdDev});
            this.Results.Location = new System.Drawing.Point(12, 192);
            this.Results.Name = "Results";
            this.Results.Size = new System.Drawing.Size(432, 152);
            this.Results.TabIndex = 5;
            // 
            // AiType
            // 
            this.AiType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.AiType.HeaderText = "AI Type";
            this.AiType.MinimumWidth = 100;
            this.AiType.Name = "AiType";
            this.AiType.ReadOnly = true;
            // 
            // MovesLeft
            // 
            this.MovesLeft.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.MovesLeft.HeaderText = "Moves Left";
            this.MovesLeft.MinimumWidth = 55;
            this.MovesLeft.Name = "MovesLeft";
            this.MovesLeft.ReadOnly = true;
            this.MovesLeft.Width = 55;
            // 
            // PowerLeft
            // 
            this.PowerLeft.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.PowerLeft.HeaderText = "Power Left";
            this.PowerLeft.MinimumWidth = 55;
            this.PowerLeft.Name = "PowerLeft";
            this.PowerLeft.ReadOnly = true;
            this.PowerLeft.Width = 55;
            // 
            // SamplesSent
            // 
            this.SamplesSent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.SamplesSent.HeaderText = "Samples Sent";
            this.SamplesSent.MinimumWidth = 55;
            this.SamplesSent.Name = "SamplesSent";
            this.SamplesSent.ReadOnly = true;
            this.SamplesSent.Width = 55;
            // 
            // SampleStdDev
            // 
            this.SampleStdDev.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.SampleStdDev.HeaderText = "Sample Std. Dev.";
            this.SampleStdDev.MinimumWidth = 75;
            this.SampleStdDev.Name = "SampleStdDev";
            this.SampleStdDev.ReadOnly = true;
            this.SampleStdDev.Width = 75;
            // 
            // TimeUsed
            // 
            this.TimeUsed.Location = new System.Drawing.Point(369, 28);
            this.TimeUsed.Name = "TimeUsed";
            this.TimeUsed.ReadOnly = true;
            this.TimeUsed.Size = new System.Drawing.Size(75, 20);
            this.TimeUsed.TabIndex = 6;
            // 
            // TimeUsedLabel
            // 
            this.TimeUsedLabel.AutoSize = true;
            this.TimeUsedLabel.Location = new System.Drawing.Point(366, 12);
            this.TimeUsedLabel.Name = "TimeUsedLabel";
            this.TimeUsedLabel.Size = new System.Drawing.Size(74, 13);
            this.TimeUsedLabel.TabIndex = 7;
            this.TimeUsedLabel.Text = "Elapsed (sec):";
            // 
            // CompletedLabel
            // 
            this.CompletedLabel.AutoSize = true;
            this.CompletedLabel.Location = new System.Drawing.Point(366, 56);
            this.CompletedLabel.Name = "CompletedLabel";
            this.CompletedLabel.Size = new System.Drawing.Size(60, 13);
            this.CompletedLabel.TabIndex = 9;
            this.CompletedLabel.Text = "Completed:";
            // 
            // Completed
            // 
            this.Completed.Location = new System.Drawing.Point(369, 72);
            this.Completed.Name = "Completed";
            this.Completed.ReadOnly = true;
            this.Completed.Size = new System.Drawing.Size(75, 20);
            this.Completed.TabIndex = 8;
            // 
            // Timer
            // 
            this.Timer.Interval = 500;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // RunCountLabel
            // 
            this.RunCountLabel.AutoSize = true;
            this.RunCountLabel.Location = new System.Drawing.Point(107, 172);
            this.RunCountLabel.Name = "RunCountLabel";
            this.RunCountLabel.Size = new System.Drawing.Size(58, 13);
            this.RunCountLabel.TabIndex = 10;
            this.RunCountLabel.Text = "Run Count";
            // 
            // LevelGeneratorName
            // 
            this.LevelGeneratorName.FormattingEnabled = true;
            this.LevelGeneratorName.Location = new System.Drawing.Point(12, 169);
            this.LevelGeneratorName.Name = "LevelGeneratorName";
            this.LevelGeneratorName.Size = new System.Drawing.Size(89, 21);
            this.LevelGeneratorName.TabIndex = 11;
            // 
            // WorkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 356);
            this.Controls.Add(this.LevelGeneratorName);
            this.Controls.Add(this.RunCountLabel);
            this.Controls.Add(this.CompletedLabel);
            this.Controls.Add(this.Completed);
            this.Controls.Add(this.TimeUsedLabel);
            this.Controls.Add(this.TimeUsed);
            this.Controls.Add(OpenRenderer);
            this.Controls.Add(this.Results);
            this.Controls.Add(this.Simulate);
            this.Controls.Add(this.AiList);
            this.Controls.Add(this.RunCount);
            this.Name = "WorkForm";
            this.Text = "Mars Rover Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.Results)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox RunCount;
        private System.Windows.Forms.Button Simulate;
        private System.Windows.Forms.DataGridView Results;
        private System.Windows.Forms.TextBox TimeUsed;
        private System.Windows.Forms.DataGridViewTextBoxColumn AiType;
        private System.Windows.Forms.DataGridViewTextBoxColumn MovesLeft;
        private System.Windows.Forms.DataGridViewTextBoxColumn PowerLeft;
        private System.Windows.Forms.DataGridViewTextBoxColumn SamplesSent;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleStdDev;
        private System.Windows.Forms.ListView AiList;
        private System.Windows.Forms.Label TimeUsedLabel;
        private System.Windows.Forms.Label CompletedLabel;
        private System.Windows.Forms.TextBox Completed;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.Label RunCountLabel;
        private System.Windows.Forms.ComboBox LevelGeneratorName;
    }
}

