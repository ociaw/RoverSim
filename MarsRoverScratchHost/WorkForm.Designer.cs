namespace MarsRoverScratchHost
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.ActionButton2 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.openRender = new System.Windows.Forms.Button();
            this.timeUsed = new System.Windows.Forms.TextBox();
            this.aiType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.movesLeft = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.powerLeft = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.samplesSent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleStdDev = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 165);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(75, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "4";
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(341, 147);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // ActionButton2
            // 
            this.ActionButton2.Location = new System.Drawing.Point(93, 163);
            this.ActionButton2.Name = "ActionButton2";
            this.ActionButton2.Size = new System.Drawing.Size(101, 23);
            this.ActionButton2.TabIndex = 3;
            this.ActionButton2.Text = "Simulate";
            this.ActionButton2.UseVisualStyleBackColor = true;
            this.ActionButton2.Click += new System.EventHandler(this.ActionButton2_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.aiType,
            this.movesLeft,
            this.powerLeft,
            this.samplesSent,
            this.SampleStdDev});
            this.dataGridView1.Location = new System.Drawing.Point(12, 192);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(432, 150);
            this.dataGridView1.TabIndex = 4;
            // 
            // openRender
            // 
            this.openRender.Location = new System.Drawing.Point(201, 163);
            this.openRender.Name = "openRender";
            this.openRender.Size = new System.Drawing.Size(75, 23);
            this.openRender.TabIndex = 5;
            this.openRender.Text = "Open Renderer";
            this.openRender.UseVisualStyleBackColor = true;
            this.openRender.Click += new System.EventHandler(this.OpenRender_Click);
            // 
            // timeUsed
            // 
            this.timeUsed.Location = new System.Drawing.Point(369, 12);
            this.timeUsed.Name = "timeUsed";
            this.timeUsed.Size = new System.Drawing.Size(75, 20);
            this.timeUsed.TabIndex = 6;
            // 
            // aiType
            // 
            this.aiType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.aiType.HeaderText = "AI Type";
            this.aiType.MinimumWidth = 100;
            this.aiType.Name = "aiType";
            this.aiType.ReadOnly = true;
            // 
            // movesLeft
            // 
            this.movesLeft.HeaderText = "Moves Left";
            this.movesLeft.MinimumWidth = 55;
            this.movesLeft.Name = "movesLeft";
            this.movesLeft.ReadOnly = true;
            this.movesLeft.Width = 55;
            // 
            // powerLeft
            // 
            this.powerLeft.HeaderText = "Power Left";
            this.powerLeft.MinimumWidth = 55;
            this.powerLeft.Name = "powerLeft";
            this.powerLeft.ReadOnly = true;
            this.powerLeft.Width = 55;
            // 
            // samplesSent
            // 
            this.samplesSent.HeaderText = "Samples Sent";
            this.samplesSent.MinimumWidth = 55;
            this.samplesSent.Name = "samplesSent";
            this.samplesSent.ReadOnly = true;
            this.samplesSent.Width = 55;
            // 
            // SampleStdDev
            // 
            this.SampleStdDev.HeaderText = "Sample Std. Dev.";
            this.SampleStdDev.MinimumWidth = 75;
            this.SampleStdDev.Name = "SampleStdDev";
            this.SampleStdDev.ReadOnly = true;
            this.SampleStdDev.Width = 75;
            // 
            // WorkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 466);
            this.Controls.Add(this.timeUsed);
            this.Controls.Add(this.openRender);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.ActionButton2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.textBox1);
            this.Name = "WorkForm";
            this.Text = "Mars Rover Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button ActionButton2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button openRender;
        private System.Windows.Forms.TextBox timeUsed;
        private System.Windows.Forms.DataGridViewTextBoxColumn aiType;
        private System.Windows.Forms.DataGridViewTextBoxColumn movesLeft;
        private System.Windows.Forms.DataGridViewTextBoxColumn powerLeft;
        private System.Windows.Forms.DataGridViewTextBoxColumn samplesSent;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleStdDev;
    }
}

