using System;
using System.Drawing;
using RoverSim.Rendering;

namespace RoverSim.WinFormsClient
{
    internal sealed class GdiRenderer
    {
        public GdiRenderer(Int32 width, Int32 height)
        {
            Width = width;
            Height = height;
        }

        public Int32 Width { get; }

        public Int32 Height { get; }

        public void Draw(Graphics surface, VisibleState state)
        {
            Int32 widthMultiplier = Width / state.Width;
            Int32 heightMultiplier = Height / state.Height;

            Point point = new Point();
            Size size = new Size(widthMultiplier, heightMultiplier);
            for (Int16 i = 0; i < state.Width; i++)
            {
                for (Int16 j = 0; j < state.Height; j++)
                {
                    DrawTile(surface, state[i, j], point, size);
                    point.Offset(0, size.Height);
                }
                point.Offset(size.Width, 0);
                point.Y = 0;
            }

            DrawRover(surface, state.RoverPosition.X, state.RoverPosition.Y, size.Width, size.Height);
        }

        private void DrawTile(Graphics surface, TerrainType terrain, Point topLeft, Size size)
        {
            Brush brush = GetBrushForTerrain(terrain);
            Rectangle rect = new Rectangle(topLeft, size);
            surface.FillRectangle(brush, rect);
        }

        private void DrawRover(Graphics surface, Int32 roverX, Int32 roverY, Int32 tileWidth, Int32 tileHeight)
        {
            const Int32 padding = 2;
            Int32 roverLeft = roverX * tileWidth + 2;
            Int32 roverTop = roverY * tileHeight + 2;
            Int32 roverWidth = Math.Max(tileWidth - padding * 2, 1);
            Int32 roverHeight = Math.Max(tileHeight - padding * 2, 1);

            Brush brush = Brushes.LightGreen;
            surface.FillRectangle(brush, roverLeft, roverTop, roverWidth, roverHeight);
        }

        private static Brush GetBrushForTerrain(TerrainType terrain)
        {
            return terrain switch
            {
                TerrainType.Impassable => Brushes.Black,
                TerrainType.Rough => Brushes.Red,
                TerrainType.SampledRough => Brushes.Brown,
                TerrainType.SampledSmooth => Brushes.DarkGray,
                TerrainType.Smooth => Brushes.LightGray,
                TerrainType.Unknown => Brushes.LightGoldenrodYellow,
                _ => Brushes.Blue,
            };
        }
    }
}
