using System;
using System.Windows.Forms;
using RoverSim.Rendering;

namespace RoverSim.WinFormsClient
{
    public sealed class RenderControl : Control
    {
        private VisibleState _state;

        public RenderControl()
        {
            VisibleState = VisibleState.GenerateBlank(default, default);
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer,
                          true
            );
        }

        public VisibleState VisibleState
        {
            get => _state;
            set => _state = value ?? throw new ArgumentNullException(nameof(value));
        }

        protected override void OnPaint(PaintEventArgs e) => GdiRenderer.Draw(e.Graphics, Width, Height, VisibleState);
    }
}
