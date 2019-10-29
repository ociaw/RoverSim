using System;

namespace RoverSim
{
    public sealed class Level
    {
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

        public Level(TerrainType[,] terrain, ProtoLevel protoLevel)
        {
            if (terrain == null)
                throw new ArgumentNullException(nameof(terrain));
            if (terrain.Length < 1)
                throw new ArgumentOutOfRangeException(nameof(terrain), "Must have at least one element.");

            ProtoLevel = protoLevel;
            Terrain = CloneArray(terrain);
        }

        public MutableLevel AsMutable() => new MutableLevel(BottomRight, CloneArray(Terrain));

        private static TerrainType[,] CloneArray(TerrainType[,] original)
        {
            Int32 width = original.GetLength(0);
            Int32 height = original.GetLength(1);

            TerrainType[,] newTerrain = new TerrainType[width, height];
            Array.Copy(original, newTerrain, width * height);
            return newTerrain;
        }
    }
}
