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
            this.glControl1 = new OpenTK.GLControl();
            this.beginRender = new System.Windows.Forms.Button();
            this.MovesLeftText = new System.Windows.Forms.TextBox();
            this.MovesLeftLabel = new System.Windows.Forms.Label();
            this.PowerLeftLabel = new System.Windows.Forms.Label();
            this.PowerLeftText = new System.Windows.Forms.TextBox();
            this.SamplesSentLabel = new System.Windows.Forms.Label();
            this.SamplesSentText = new System.Windows.Forms.TextBox();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.ExportButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(12, 12);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(480, 345);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.GlControl1_Load);
            // 
            // beginRender
            // 
            this.beginRender.Enabled = false;
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
            // RenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 369);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.SamplesSentLabel);
            this.Controls.Add(this.SamplesSentText);
            this.Controls.Add(this.PowerLeftLabel);
            this.Controls.Add(this.PowerLeftText);
            this.Controls.Add(this.MovesLeftLabel);
            this.Controls.Add(this.MovesLeftText);
            this.Controls.Add(this.beginRender);
            this.Controls.Add(this.glControl1);
            this.Name = "RenderForm";
            this.Text = "Simulation Render";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.Button beginRender;
        private System.Windows.Forms.TextBox MovesLeftText;
        private System.Windows.Forms.Label MovesLeftLabel;
        private System.Windows.Forms.Label PowerLeftLabel;
        private System.Windows.Forms.TextBox PowerLeftText;
        private System.Windows.Forms.Label SamplesSentLabel;
        private System.Windows.Forms.TextBox SamplesSentText;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.Button ExportButton;
    }
}
