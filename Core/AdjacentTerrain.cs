using System;

namespace RoverSim
{
    public readonly struct AdjacentTerrain : IEquatable<AdjacentTerrain>
    {
        public AdjacentTerrain(TerrainType up, TerrainType right, TerrainType down, TerrainType left, TerrainType occupied)
        {
            Up = up;
            Right = right;
            Down = down;
            Left = left;
            Occupied = occupied;
        }

        public TerrainType Up { get; }

        public TerrainType Right { get; }

        public TerrainType Down { get; }

        public TerrainType Left { get; }

        public TerrainType Occupied { get; }

        public TerrainType this[Direction direction] => direction.Value switch
        {
            0 => Up,
            1 => Right,
            2 => Down,
            3 => Left,
            4 => Occupied,
            _ => throw new Exception("This is not possible.")
        };

        public static Int32 Count => 5;

        public Boolean Equals(AdjacentTerrain other) =>
            Up == other.Up && Right == other.Right &&
            Down == other.Down && Left == other.Left &&
            Occupied == other.Occupied;

        public override Boolean Equals(Object obj) => obj is AdjacentTerrain terrain && Equals(terrain);

        public override Int32 GetHashCode() => HashCode.Combine(Up, Right, Down, Left, Occupied);

        public static Boolean operator ==(AdjacentTerrain left, AdjacentTerrain right) => left.Equals(right);

        public static Boolean operator !=(AdjacentTerrain left, AdjacentTerrain right) => !(left == right);
    }
}
