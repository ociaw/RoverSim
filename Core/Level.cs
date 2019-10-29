using System;

namespace RoverSim
{
    public sealed class Level
    {
        public Level(TerrainType[,] terrain, ProtoLevel protoLevel)
        {
            if (terrain == null)
                throw new ArgumentNullException(nameof(terrain));
            if (terrain.Length < 1)
                throw new ArgumentOutOfRangeException(nameof(terrain), "Must have at least one element.");

            Terrain = terrain;
            ProtoLevel = protoLevel ?? throw new ArgumentNullException(nameof(protoLevel));
        }

        private TerrainType[,] Terrain { get; }

        public ProtoLevel ProtoLevel { get; }

        public Position BottomRight => ProtoLevel.LevelGenerator.Parameters.BottomRight;

        public Int32 Width => BottomRight.X + 1;

        public Int32 Height => BottomRight.Y + 1;

        public TerrainType this[CoordinatePair coords]
        {
            get
            {
                if (!BottomRight.Contains(coords))
                    throw new ArgumentOutOfRangeException(nameof(coords));

                return Terrain[coords.X, coords.Y];
            }
        }

        public TerrainType GetTerrain(CoordinatePair coordinates)
        {
            if (coordinates.IsNegative || !BottomRight.Coordinates.Contains(coordinates))
                return TerrainType.Impassable;
            else
                return Terrain[coordinates.X, coordinates.Y];
        }

        public TerrainType SampleSquare(Position position)
        {
            if (!BottomRight.Contains(position))
                throw new ArgumentOutOfRangeException(nameof(position), position, "Must be contained within this level.");

            (Int32 x, Int32 y) = position;
            if (Terrain[x, y] == TerrainType.Rough)
                Terrain[x, y] = TerrainType.SampledRough;
            else if (Terrain[x, y] == TerrainType.Smooth)
                Terrain[x, y] = TerrainType.SampledSmooth;

            return Terrain[x, y];
        }
    }
}
