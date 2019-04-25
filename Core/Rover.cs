using System;

namespace RoverSim
{
    public sealed class Rover : IRover
    {
        private Int32 moves = 1000, power = 500;

        public Rover(MutableLevel level)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            PosX = level.CenterX;
            PosY = level.CenterY;
        }

        public static Int32 TransmitCost => 50;
        public static Int32 SampleCost => 10;
        public static Int32 ProcessCost => 30;
        public static Int32 SmoothCost => 10;
        public static Int32 RoughCost => 50;

        private MutableLevel Level { get; }

        public Int32 PosX { get; private set; }
        public Int32 PosY { get; private set; }
        public Int32 SamplesCollected { get; private set; } = 0;
        public Int32 MovesLeft
        {
            get { return moves; }
            private set { if (value > 0) moves = value; else moves = 0; }
        }

        public Boolean IsHalted => Power == 0 || MovesLeft == 0;

        public Int32 Power
        {
            get { return power; }
            private set { if (value > 0) power = value; else power = 0; }
        }
        public Int32 SamplesProcessed { get; private set; }
        public Int32 NoBacktrack { get; private set; } = 1;
        public Int32 SamplesTransmitted { get; private set; }

        public Int32 PotentialLight => NoBacktrack * NoBacktrack * NoBacktrack;

        public TerrainType SenseSquare(Direction direction)
        {
            return Level.GetTerrainSquare(PosX + direction.ChangeInX(), PosY + direction.ChangeInY()).Type;
        }

        public Boolean Move(Direction direction)
        {
            ThrowIfHalted();
            
            Int32 newX = PosX + direction.ChangeInX();
            Int32 newY = PosY + direction.ChangeInY();
            TerrainSquare newTerrain = Level.GetTerrainSquare(newX, newY);
            if (newTerrain.Type == TerrainType.Impassable)
                return false;

            PosX = newX;
            PosY = newY;
            MovesLeft -= 1;
            Power -= newTerrain.PowerNeeded;
            if (newTerrain.Type != TerrainType.Smooth)
                NoBacktrack = 1;
            else
                NoBacktrack += 1;

            return true;
        }

        public Int32 Transmit()
        {
            ThrowIfHalted();

            Int32 processedCount = SamplesProcessed;
            
            MovesLeft -= 1;
            Power -= TransmitCost;
            SamplesTransmitted += processedCount;
            SamplesProcessed = 0;

            return processedCount;
        }

        public Int32 CollectPower()
        {
            if (MovesLeft == 0)
                throw new OutOfMovesException();

            Int32 gatheredPower = PotentialLight;
            
            MovesLeft -= 1;
            Power += gatheredPower;
            NoBacktrack = 1;
            return gatheredPower;
        }

        public (Boolean isSuccess, TerrainType newTerrain) CollectSample()
        {
            ThrowIfHalted();
            
            MovesLeft -= 1;
            Power -= SampleCost;
            TerrainSquare square = Level.GetTerrainSquare(PosX, PosY);
            if (square.Type != TerrainType.Smooth && square.Type != TerrainType.Rough || SamplesCollected >= 10)
                return (false, square.Type);

            SamplesCollected += 1;
            return (true, Level.SampleSquare(PosX, PosY));
        }

        public Int32 ProcessSamples()
        {
            ThrowIfHalted();
            
            MovesLeft -= 1;
            Power -= ProcessCost;
            var processingCount = SamplesCollected > 3 ? 3 : SamplesCollected;
            SamplesProcessed += processingCount;
            SamplesCollected -= processingCount;
            return processingCount;
        }

        private void ThrowIfHalted()
        {
            if (MovesLeft == 0)
                throw new OutOfMovesException();
            if (Power == 0)
                throw new OutOfPowerException();
        }
    }
}
