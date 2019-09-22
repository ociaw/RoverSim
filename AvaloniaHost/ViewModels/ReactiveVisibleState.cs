using System;
using ReactiveUI;

namespace RoverSim.Rendering
{
    public sealed class ReactiveVisibleState : ReactiveObject
    {
        private VisibleState _wrapped;

        public ReactiveVisibleState(VisibleState wrapped)
        {
            _wrapped = wrapped;
        }

        public Position BottomRight => _wrapped.BottomRight;

        public Int32 Width => _wrapped.Width;

        public Int32 Height => _wrapped.Height;

        public TerrainType this[Int32 x, Int32 y] => _wrapped[x, y];

        public Position RoverPosition => _wrapped.RoverPosition;

        public void UpdateRoverPos(PositionUpdate update)
        {
            _wrapped.UpdateRoverPos(update);
            this.RaisePropertyChanged();
        }

        public void UpdateTerrain(TerrainUpdate update)
        {
            _wrapped.UpdateTerrain(update);
            this.RaisePropertyChanged();
        }
    }
}
