namespace RoverSim.WinFormsClient
{
    partial class RenderForm
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
            this.RenderControl = new RoverSim.WinFormsClient.RenderControl();
            this.beginRender = new System.Windows.Forms.Button();
            this.MovesLeftText = new System.Windows.Forms.TextBox();
            this.MovesLeftLabel = new System.Windows.Forms.Label();
            this.PowerLeftLabel = new System.Windows.Forms.Label();
            this.PowerLeftText = new System.Windows.Forms.TextBox();
            this.SamplesSentLabel = new System.Windows.Forms.Label();
            this.SamplesSentText = new System.Windows.Forms.TextBox();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.ExportButton = new System.Windows.Forms.Button();
            this.HopperText = new System.Windows.Forms.TextBox();
            this.ProcessedText = new System.Windows.Forms.TextBox();
            this.MovesText = new System.Windows.Forms.TextBox();
            this.MoveCallText = new System.Windows.Forms.TextBox();
            this.PowerCallText = new System.Windows.Forms.TextBox();
            this.SampleCallText = new System.Windows.Forms.TextBox();
            this.ProcessCallText = new System.Windows.Forms.TextBox();
            this.TransmitCallText = new System.Windows.Forms.TextBox();
            this.HopperLabel = new System.Windows.Forms.Label();
            this.ProcessedLabel = new System.Windows.Forms.Label();
            this.MovesLabel = new System.Windows.Forms.Label();
            this.MoveCallLabel = new System.Windows.Forms.Label();
            this.PowerCallLabel = new System.Windows.Forms.Label();
            this.SampleCallLabel = new System.Windows.Forms.Label();
            this.ProcessCallLabel = new System.Windows.Forms.Label();
            this.TransmitCallLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // RenderControl
            // 
            this.RenderControl.BackColor = System.Drawing.Color.White;
            this.RenderControl.Location = new System.Drawing.Point(12, 12);
            this.RenderControl.Name = "RenderControl";
            this.RenderControl.Size = new System.Drawing.Size(480, 345);
            this.RenderControl.TabIndex = 0;
            // 
            // beginRender
            // 
            this.beginRender.Location = new System.Drawing.Point(596, 334);
            this.beginRender.Name = "beginRender";
            this.beginRender.Size = new System.Drawing.Size(75, 23);
            this.beginRender.TabIndex = 1;
            this.beginRender.Text = "Begin";
            this.beginRender.UseVisualStyleBackColor = true;
            this.beginRender.Click += new System.EventHandler(this.BeginRender_Click);
            // 
            // MovesLeftText
            // 
            this.MovesLeftText.Enabled = false;
            this.MovesLeftText.Location = new System.Drawing.Point(571, 12);
            this.MovesLeftText.Name = "MovesLeftText";
            this.MovesLeftText.Size = new System.Drawing.Size(100, 20);
            this.MovesLeftText.TabIndex = 2;
            // 
            // MovesLeftLabel
            // 
            this.MovesLeftLabel.AutoSize = true;
            this.MovesLeftLabel.Location = new System.Drawing.Point(505, 15);
            this.MovesLeftLabel.Name = "MovesLeftLabel";
            this.MovesLeftLabel.Size = new System.Drawing.Size(60, 13);
            this.MovesLeftLabel.TabIndex = 3;
            this.MovesLeftLabel.Text = "Moves Left";
            // 
            // PowerLeftLabel
            // 
            this.PowerLeftLabel.AutoSize = true;
            this.PowerLeftLabel.Location = new System.Drawing.Point(507, 41);
            this.PowerLeftLabel.Name = "PowerLeftLabel";
            this.PowerLeftLabel.Size = new System.Drawing.Size(58, 13);
            this.PowerLeftLabel.TabIndex = 5;
            this.PowerLeftLabel.Text = "Power Left";
            // 
            // PowerLeftText
            // 
            this.PowerLeftText.Enabled = false;
            this.PowerLeftText.Location = new System.Drawing.Point(571, 38);
            this.PowerLeftText.Name = "PowerLeftText";
            this.PowerLeftText.Size = new System.Drawing.Size(100, 20);
            this.PowerLeftText.TabIndex = 4;
            // 
            // SamplesSentLabel
            // 
            this.SamplesSentLabel.AutoSize = true;
            this.SamplesSentLabel.Location = new System.Drawing.Point(493, 67);
            this.SamplesSentLabel.Name = "SamplesSentLabel";
            this.SamplesSentLabel.Size = new System.Drawing.Size(72, 13);
            this.SamplesSentLabel.TabIndex = 7;
            this.SamplesSentLabel.Text = "Samples Sent";
            // 
            // SamplesSentText
            // 
            this.SamplesSentText.Enabled = false;
            this.SamplesSentText.Location = new System.Drawing.Point(571, 64);
            this.SamplesSentText.Name = "SamplesSentText";
            this.SamplesSentText.Size = new System.Drawing.Size(100, 20);
            this.SamplesSentText.TabIndex = 6;
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(508, 334);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(82, 23);
            this.ExportButton.TabIndex = 8;
            this.ExportButton.Text = "Export Images";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // HopperText
            // 
            this.HopperText.Enabled = false;
            this.HopperText.Location = new System.Drawing.Point(571, 107);
            this.HopperText.Name = "HopperText";
            this.HopperText.Size = new System.Drawing.Size(100, 20);
            this.HopperText.TabIndex = 9;
            // 
            // ProcessedText
            // 
            this.ProcessedText.Enabled = false;
            this.ProcessedText.Location = new System.Drawing.Point(571, 133);
            this.ProcessedText.Name = "ProcessedText";
            this.ProcessedText.Size = new System.Drawing.Size(100, 20);
            this.ProcessedText.TabIndex = 10;
            // 
            // MovesText
            // 
            this.MovesText.Enabled = false;
            this.MovesText.Location = new System.Drawing.Point(571, 159);
            this.MovesText.Name = "MovesText";
            this.MovesText.Size = new System.Drawing.Size(100, 20);
            this.MovesText.TabIndex = 11;
            // 
            // MoveCallText
            // 
            this.MoveCallText.Enabled = false;
            this.MoveCallText.Location = new System.Drawing.Point(571, 204);
            this.MoveCallText.Name = "MoveCallText";
            this.MoveCallText.Size = new System.Drawing.Size(100, 20);
            this.MoveCallText.TabIndex = 12;
            // 
            // PowerCallText
            // 
            this.PowerCallText.Enabled = false;
            this.PowerCallText.Location = new System.Drawing.Point(571, 230);
            this.PowerCallText.Name = "PowerCallText";
            this.PowerCallText.Size = new System.Drawing.Size(100, 20);
            this.PowerCallText.TabIndex = 13;
            // 
            // SampleCallText
            // 
            this.SampleCallText.Enabled = false;
            this.SampleCallText.Location = new System.Drawing.Point(571, 256);
            this.SampleCallText.Name = "SampleCallText";
            this.SampleCallText.Size = new System.Drawing.Size(100, 20);
            this.SampleCallText.TabIndex = 14;
            // 
            // ProcessCallText
            // 
            this.ProcessCallText.Enabled = false;
            this.ProcessCallText.Location = new System.Drawing.Point(571, 282);
            this.ProcessCallText.Name = "ProcessCallText";
            this.ProcessCallText.Size = new System.Drawing.Size(100, 20);
            this.ProcessCallText.TabIndex = 15;
            // 
            // TransmitCallText
            // 
            this.TransmitCallText.Enabled = false;
            this.TransmitCallText.Location = new System.Drawing.Point(571, 308);
            this.TransmitCallText.Name = "TransmitCallText";
            this.TransmitCallText.Size = new System.Drawing.Size(100, 20);
            this.TransmitCallText.TabIndex = 16;
            // 
            // HopperLabel
            // 
            this.HopperLabel.AutoSize = true;
            this.HopperLabel.Location = new System.Drawing.Point(492, 110);
            this.HopperLabel.Name = "HopperLabel";
            this.HopperLabel.Size = new System.Drawing.Size(73, 13);
            this.HopperLabel.TabIndex = 17;
            this.HopperLabel.Text = "Hopper Count";
            // 
            // ProcessedLabel
            // 
            this.ProcessedLabel.AutoSize = true;
            this.ProcessedLabel.Location = new System.Drawing.Point(508, 136);
            this.ProcessedLabel.Name = "ProcessedLabel";
            this.ProcessedLabel.Size = new System.Drawing.Size(57, 13);
            this.ProcessedLabel.TabIndex = 18;
            this.ProcessedLabel.Text = "Processed";
            // 
            // MovesLabel
            // 
            this.MovesLabel.AutoSize = true;
            this.MovesLabel.Location = new System.Drawing.Point(526, 162);
            this.MovesLabel.Name = "MovesLabel";
            this.MovesLabel.Size = new System.Drawing.Size(39, 13);
            this.MovesLabel.TabIndex = 19;
            this.MovesLabel.Text = "Moves";
            // 
            // MoveCallLabel
            // 
            this.MoveCallLabel.AutoSize = true;
            this.MoveCallLabel.Location = new System.Drawing.Point(507, 207);
            this.MoveCallLabel.Name = "MoveCallLabel";
            this.MoveCallLabel.Size = new System.Drawing.Size(59, 13);
            this.MoveCallLabel.TabIndex = 20;
            this.MoveCallLabel.Text = "Move Calls";
            // 
            // PowerCallLabel
            // 
            this.PowerCallLabel.AutoSize = true;
            this.PowerCallLabel.Location = new System.Drawing.Point(503, 233);
            this.PowerCallLabel.Name = "PowerCallLabel";
            this.PowerCallLabel.Size = new System.Drawing.Size(62, 13);
            this.PowerCallLabel.TabIndex = 21;
            this.PowerCallLabel.Text = "Power Calls";
            // 
            // SampleCallLabel
            // 
            this.SampleCallLabel.AutoSize = true;
            this.SampleCallLabel.Location = new System.Drawing.Point(498, 259);
            this.SampleCallLabel.Name = "SampleCallLabel";
            this.SampleCallLabel.Size = new System.Drawing.Size(67, 13);
            this.SampleCallLabel.TabIndex = 22;
            this.SampleCallLabel.Text = "Sample Calls";
            // 
            // ProcessCallLabel
            // 
            this.ProcessCallLabel.AutoSize = true;
            this.ProcessCallLabel.Location = new System.Drawing.Point(495, 285);
            this.ProcessCallLabel.Name = "ProcessCallLabel";
            this.ProcessCallLabel.Size = new System.Drawing.Size(70, 13);
            this.ProcessCallLabel.TabIndex = 23;
            this.ProcessCallLabel.Text = "Process Calls";
            // 
            // TransmitCallLabel
            // 
            this.TransmitCallLabel.AutoSize = true;
            this.TransmitCallLabel.Location = new System.Drawing.Point(493, 311);
            this.TransmitCallLabel.Name = "TransmitCallLabel";
            this.TransmitCallLabel.Size = new System.Drawing.Size(72, 13);
            this.TransmitCallLabel.TabIndex = 24;
            this.TransmitCallLabel.Text = "Transmit Calls";
            // 
            // RenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 369);
            this.Controls.Add(this.TransmitCallLabel);
            this.Controls.Add(this.ProcessCallLabel);
            this.Controls.Add(this.SampleCallLabel);
            this.Controls.Add(this.PowerCallLabel);
            this.Controls.Add(this.MoveCallLabel);
            this.Controls.Add(this.MovesLabel);
            this.Controls.Add(this.ProcessedLabel);
            this.Controls.Add(this.HopperLabel);
            this.Controls.Add(this.TransmitCallText);
            this.Controls.Add(this.ProcessCallText);
            this.Controls.Add(this.SampleCallText);
            this.Controls.Add(this.PowerCallText);
            this.Controls.Add(this.MoveCallText);
            this.Controls.Add(this.MovesText);
            this.Controls.Add(this.ProcessedText);
            this.Controls.Add(this.HopperText);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.SamplesSentLabel);
            this.Controls.Add(this.SamplesSentText);
            this.Controls.Add(this.PowerLeftLabel);
            this.Controls.Add(this.PowerLeftText);
            this.Controls.Add(this.MovesLeftLabel);
            this.Controls.Add(this.MovesLeftText);
            this.Controls.Add(this.beginRender);
            this.Controls.Add(this.RenderControl);
            this.Name = "RenderForm";
            this.Text = "Simulation Render";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RoverSim.WinFormsClient.RenderControl RenderControl;
        private System.Windows.Forms.Button beginRender;
        private System.Windows.Forms.TextBox MovesLeftText;
        private System.Windows.Forms.Label MovesLeftLabel;
        private System.Windows.Forms.Label PowerLeftLabel;
        private System.Windows.Forms.TextBox PowerLeftText;
        private System.Windows.Forms.Label SamplesSentLabel;
        private System.Windows.Forms.TextBox SamplesSentText;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.TextBox HopperText;
        private System.Windows.Forms.TextBox ProcessedText;
        private System.Windows.Forms.TextBox MovesText;
        private System.Windows.Forms.TextBox MoveCallText;
        private System.Windows.Forms.TextBox PowerCallText;
        private System.Windows.Forms.TextBox SampleCallText;
        private System.Windows.Forms.TextBox ProcessCallText;
        private System.Windows.Forms.TextBox TransmitCallText;
        private System.Windows.Forms.Label HopperLabel;
        private System.Windows.Forms.Label ProcessedLabel;
        private System.Windows.Forms.Label MovesLabel;
        private System.Windows.Forms.Label MoveCallLabel;
        private System.Windows.Forms.Label PowerCallLabel;
        private System.Windows.Forms.Label SampleCallLabel;
        private System.Windows.Forms.Label ProcessCallLabel;
        private System.Windows.Forms.Label TransmitCallLabel;
    }
}
