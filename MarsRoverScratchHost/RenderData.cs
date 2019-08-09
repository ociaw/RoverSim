using System;
using RoverSim;

namespace MarsRoverScratchHost
{
    /// <summary>
    /// Contains the information necessary to render the rover.
    /// </summary>
    public sealed class RenderData
    {
        private RenderData(Int32 width, Int32 height, TerrainType[,] terrain, Position roverPosition)
        {
            Width = width;
            Height = height;
            Terrain = terrain;
            RoverPosition = roverPosition;
        }

        public Int32 Width { get; }

        public Int32 Height { get; }

        public TerrainType[,] Terrain { get; }

        public Position RoverPosition { get; private set; }

        public void UpdateRoverPos(PositionUpdate update)
        {
            if (update.PreviousX >= Width)
                throw new ArgumentOutOfRangeException(nameof(update), update.PreviousX, "Previous X must be less than " + Width);
            if (update.PreviousY >= Height)
                throw new ArgumentOutOfRangeException(nameof(update), update.PreviousY, "Previous Y must be less than " + Height);
            if (update.NewX >= Width)
                throw new ArgumentOutOfRangeException(nameof(update), update.NewX, "New X must be less than " + Width);
            if (update.NewY >= Height)
                throw new ArgumentOutOfRangeException(nameof(update), update.NewY, "New Y must be less than " + Height);

            RoverPosition = update.New;
        }

        public void UpdateTerrain(TerrainUpdate update)
        {
            if (update.PosX >= Width)
                throw new ArgumentOutOfRangeException(nameof(update), update.PosX, "X Position must be less than " + Width);
            if (update.PosY >= Height)
                throw new ArgumentOutOfRangeException(nameof(update), update.PosY, "Y Position must be less than " + Height);

            Terrain[update.PosX, update.PosY] = update.NewTerrain;
        }

        public static RenderData GenerateBlank(Int32 width, Int32 height, Position roverPos)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, "Must be non-negative");
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, "Must be non-negative");
            if (roverPos.IsNegative)
                throw new ArgumentOutOfRangeException(nameof(roverPos), roverPos, "Must be non-negative.");
            if (roverPos.X >= width)
                throw new ArgumentOutOfRangeException(nameof(roverPos), roverPos, "X must be within " + nameof(width));
            if (roverPos.Y >= height)
                throw new ArgumentOutOfRangeException(nameof(roverPos), roverPos, "Y must be within " + nameof(height));

            TerrainType[,] terrain = new TerrainType[width, height];
            for (Byte i = 1; i < width - 1; i++)
            {
                for (Byte j = 1; j < height - 1; j++)
                {
                    terrain[i, j] = TerrainType.Unknown;
                }
            }

            return new RenderData(width, height, terrain, roverPos);
        }
    }
}