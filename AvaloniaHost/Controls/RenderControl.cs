using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using RoverSim.Rendering;

namespace RoverSim.AvaloniaHost.Controls
{
    public sealed class RenderControl : Control
    {
        public static readonly StyledProperty<ReactiveVisibleState> StateProperty =
            AvaloniaProperty.Register<RenderControl, ReactiveVisibleState>(nameof(State));

        static RenderControl()
        {
            AffectsRender<RenderControl>(StateProperty);
        }

        public ReactiveVisibleState State
        {
            get => GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        private Int32 HorizontalTileCount => State.Width;

        private Int32 VerticalTileCount => State.Height;

        public Double RoverScale { get; } = .64;

        public override void Render(DrawingContext context)
        {
            if (State == null)
                return;

            Color[] terrainColors = new Color[]
            {
                Colors.Black, // Impassable
                Colors.Red, // Rough
                Colors.LightGray, // Smooth
                Colors.Brown, // Rough Sampled
                Colors.DarkGray, // Smooth Sampled
                Colors.LightGoldenrodYellow // Unknown
            };

            Size tileSize = new Size(Bounds.Size.Width / HorizontalTileCount, Bounds.Size.Height / VerticalTileCount);

            Brush[] terrainBrushes = new Brush[terrainColors.Length];
            for (Int32 i = 0; i < terrainColors.Length; i++)
                terrainBrushes[i] = new SolidColorBrush(terrainColors[i]);

            for (Int32 i = 0; i < HorizontalTileCount; i++)
            {
                for (Int32 j = 0; j < VerticalTileCount; j++)
                {
                    TerrainType terrain = State[i, j];
                    if (terrain < 0 || (Int32)terrain > terrainBrushes.Length)
                        terrain = TerrainType.Unknown;

                    Brush brush = terrainBrushes[(Int32)terrain];
                    Point topLeft = new Point(tileSize.Width * i, tileSize.Height * j);
                    Rect rect = new Rect(topLeft, tileSize);
                    context.FillRectangle(brush, rect);
                }
            }

            RenderRover(context, tileSize);
        }

        private void RenderRover(DrawingContext context, Size tileSize)
        {
            Size size = tileSize * RoverScale;
            Size padding = (tileSize - size) / 2;
            Brush brush = new SolidColorBrush(Colors.LightGreen);
            Position roverPos = State.RoverPosition;
            Point topLeft = new Point(tileSize.Width * roverPos.X + padding.Width, tileSize.Height * roverPos.Y + padding.Height);
            Rect rect = new Rect(topLeft, size);

            context.FillRectangle(brush, rect);
        }
    }
}
