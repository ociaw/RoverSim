using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace RoverSim.AvaloniaHost.Controls
{
    public sealed class RenderControl : Control
    {
        public static readonly StyledProperty<TerrainType[,]> TilesProperty =
            AvaloniaProperty.Register<RenderControl, TerrainType[,]>(nameof(Tiles));

        public static readonly StyledProperty<Int32> RoverXProperty =
            AvaloniaProperty.Register<RenderControl, Int32>(nameof(RoverX));

        public static readonly StyledProperty<Int32> RoverYProperty =
            AvaloniaProperty.Register<RenderControl, Int32>(nameof(RoverY));

        static RenderControl()
        {
            AffectsRender<RenderControl>(TilesProperty);
            AffectsRender<RenderControl>(RoverXProperty);
            AffectsRender<RenderControl>(RoverYProperty);
        }

        public Int32 HorizontalTileCount => Tiles?.GetLength(0) ?? 0;

        public Int32 VerticalTileCount => Tiles?.GetLength(1) ?? 0;

        public TerrainType[,] Tiles
        {
            get => GetValue(TilesProperty);
            set => SetValue(TilesProperty, value);
        }

        public Int32 RoverX
        {
            get => GetValue(RoverXProperty);
            set => SetValue(RoverXProperty, value);
        }

        public Int32 RoverY
        {
            get => GetValue(RoverYProperty);
            set => SetValue(RoverYProperty, value);
        }

        public Double RoverScale { get; } = .64;

        public override void Render(DrawingContext context)
        {
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
                    TerrainType terrain = Tiles[i, j];
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
            Point topLeft = new Point(tileSize.Width * RoverX + padding.Width, tileSize.Height * RoverY + padding.Height);
            Rect rect = new Rect(topLeft, size);

            context.FillRectangle(brush, rect);
        }
    }
}
