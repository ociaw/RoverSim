using System;

namespace RoverSim
{
    public readonly struct AdjacentTerrain
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
    }
}
