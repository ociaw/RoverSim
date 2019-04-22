using System;
using RoverSim;

namespace MarsRoverScratchHost
{
    /// <summary>
    /// Contains the information necessary to render the rover.
    /// </summary>
    public sealed class RenderData
    {
        private RenderData(Int32 width, Int32 height, TerrainType[,] terrain, Int32 roverX, Int32 roverY)
        {
            Width = width;
            Height = height;
            Terrain = terrain;
            RoverX = roverX;
            RoverY = roverY;
        }

        public Int32 Width { get; }

        public Int32 Height { get; }

        public TerrainType[,] Terrain { get; }

        public Int32 RoverX { get; private set; }

        public Int32 RoverY { get; private set; }

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

            RoverX = update.NewX;
            RoverY = update.NewY;
        }

        public void UpdateTerrain(TerrainUpdate update)
        {
            if (update.PosX >= Width)
                throw new ArgumentOutOfRangeException(nameof(update), update.PosX, "X Position must be less than " + Width);
            if (update.PosY >= Height)
                throw new ArgumentOutOfRangeException(nameof(update), update.PosY, "Y Position must be less than " + Height);

            Terrain[update.PosX, update.PosY] = update.NewTerrain;
        }

        public static RenderData GenerateBlank(Int32 width, Int32 height, Int32 roverX, Int32 roverY)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, "Must be non-negative");
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, "Must be non-negative");
            if (roverX < 0 || roverX >= width)
                throw new ArgumentOutOfRangeException(nameof(roverX), roverX, "Must non-negative and less than " + nameof(width));
            if (roverY < 0 || roverY >= height)
                throw new ArgumentOutOfRangeException(nameof(roverY), roverY, "Must non-negative and less than " + nameof(height));

            TerrainType[,] terrain = new TerrainType[width, height];
            for (Byte i = 1; i < width - 1; i++)
            {
                for (Byte j = 1; j < height - 1; j++)
                {
                    terrain[i, j] = TerrainType.Unknown;
                }
            }

            return new RenderData(width, height, terrain, roverX, roverY);
        }
    }
}