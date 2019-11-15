using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using RoverSim.Rendering;

namespace RoverSim.AvaloniaHost.Controls
{
    public sealed class RenderControl : Control
    {
        public static readonly StyledProperty<VisibleState> StateProperty =
            AvaloniaProperty.Register<RenderControl, VisibleState>(nameof(State));

        public static readonly StyledProperty<IObservable<VisibleState>> VisibleStateProperty =
            AvaloniaProperty.Register<RenderControl, IObservable<VisibleState>>(nameof(VisibleState));

        private static readonly Color[] _terrainColors = new Color[]
        {
            Colors.Black, // Impassable
            Colors.Red, // Rough
            Colors.LightGray, // Smooth
            Colors.Brown, // Rough Sampled
            Colors.DarkGray, // Smooth Sampled
            Colors.LightGoldenrodYellow // Unknown
        };

        static RenderControl()
        {
            AffectsRender<RenderControl>(StateProperty);
        }

        private readonly Brush[] _terrainBrushes;

        public RenderControl()
        {
            _terrainBrushes = new Brush[_terrainColors.Length];
            for (Int32 i = 0; i < _terrainColors.Length; i++)
                _terrainBrushes[i] = new SolidColorBrush(_terrainColors[i]);

            this
                .WhenAnyObservable(m => m.VisibleState)
                .Subscribe(state => State = state);
        }

        public SimulationParameters Parameters => SimulationParameters.Default;

        public VisibleState State
        {
            get => GetValue(StateProperty);
            set
            {
                SetValue(StateProperty, value);
                // We have to manually raise this, since it seems to be ignored otherwise.
                RaisePropertyChanged(StateProperty, value, value);
            }
        }
        public IObservable<VisibleState> VisibleState
        {
            get => GetValue(VisibleStateProperty);
            set => SetValue(VisibleStateProperty, value);
        }

        private Int32 HorizontalTileCount => State.Width;

        private Int32 VerticalTileCount => State.Height;

        public Double RoverScale { get; } = .64;

        public override void Render(DrawingContext context)
        {
            if (State == null)
                return;

            Double widthFactor = Bounds.Size.Width / HorizontalTileCount;
            Double heightFactor = Bounds.Size.Height / VerticalTileCount;
            Double sideLength = Math.Max(Math.Min(widthFactor, heightFactor), 1);

            Size tileSize = new Size(sideLength, sideLength);

            for (Int32 i = 0; i < HorizontalTileCount; i++)
            {
                for (Int32 j = 0; j < VerticalTileCount; j++)
                {
                    TerrainType terrain = State[i, j];
                    if (terrain < 0 || (Int32)terrain > _terrainBrushes.Length)
                        terrain = TerrainType.Unknown;

                    Brush brush = _terrainBrushes[(Int32)terrain];
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
