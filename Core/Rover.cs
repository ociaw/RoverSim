using System;

namespace MarsRoverScratch
{
    public sealed class Rover : IRover
    {
        private Int32 moves = 1000, power = 500;

        public Rover(Level terrain)
        {
            Terrain = terrain ?? throw new ArgumentNullException(nameof(terrain));
            PosX = 16;
            PosY = 11;
        }

        public static Int32 TransmitCost => 50;
        public static Int32 SampleCost => 10;
        public static Int32 ProcessCost => 30;
        public static Int32 SmoothCost => 10;
        public static Int32 RoughCost => 50;

        private Level Terrain { get; }

        public Int32 PosX { get; private set; }
        public Int32 PosY { get; private set; }
        public Int32 SamplesCollected { get; private set; } = 0;
        public Int32 MovesLeft
        {
            get { return moves; }
            private set { if (value > 0) moves = value; else moves = 0; }
        }
        public Int32 Power
        {
            get { return power; }
            private set { if (value > 0) power = value; else power = 0; }
        }
        public Int32 SamplesProcessed { get; private set; }
        public Int32 NoBacktrack { get; private set; } = 1;
        public Int32 SamplesTransmitted { get; private set; }
        public TerrainType Sense { get; private set; } = TerrainType.Smooth;

        public Int16 TimesPowered { get; private set; }
        public Int16 TimesMoved { get; private set; }
        public Int16 TimesTransmitted { get; private set; }
        public Int16 TimesProcessed { get; private set; }
        public Int16 TimesSampled { get; private set; }

        public Int32 PotentialLight => NoBacktrack * NoBacktrack * NoBacktrack;

        public void SenseSquare(Direction direction)
        {
            Sense = Terrain.GetTerrainSquare(PosX + direction.ChangeInX(), PosY + direction.ChangeInY()).Type;
        }

        public Boolean Move(Direction direction)
        {
            ThrowIfHalted();

            TimesMoved++;
            Int32 newX = PosX + direction.ChangeInX();
            Int32 newY = PosY + direction.ChangeInY();
            TerrainSquare newTerrain = Terrain.GetTerrainSquare(newX, newY);
            if (newTerrain.Type != TerrainType.Impassable)
            {
                PosX = newX;
                PosY = newY;
                MovesLeft -= 1;
                Power -= newTerrain.PowerNeeded;
                if (newTerrain.Type != TerrainType.Smooth)
                    NoBacktrack = 1;
                else
                    NoBacktrack += 1;
            }
            return IsHalted;
        }

        public Boolean Transmit()
        {
            ThrowIfHalted();

            TimesTransmitted++;
            MovesLeft -= 1;
            Power -= TransmitCost;
            SamplesTransmitted += SamplesProcessed;
            SamplesProcessed = 0;
            return IsHalted;
        }

        public Boolean CollectPower()
        {
            if (MovesLeft == 0)
                throw new OutOfMovesException();

            TimesPowered++;
            MovesLeft -= 1;
            Power += PotentialLight;
            NoBacktrack = 1;
            return IsHalted;
        }

        public Boolean CollectSample()
        {
            ThrowIfHalted();

            TimesSampled++;
            MovesLeft -= 1;
            Power -= SampleCost;
            TerrainSquare square = Terrain.GetTerrainSquare(PosX, PosY);
            if ((square.Type == TerrainType.Smooth || square.Type == TerrainType.Rough) && SamplesCollected < 10)
            {
                SamplesCollected += 1;
                Terrain.SampleSquare(PosX, PosY);
            }
            return IsHalted;
        }

        public Boolean ProcessSamples()
        {
            ThrowIfHalted();

            TimesProcessed++;
            MovesLeft -= 1;
            Power -= ProcessCost;
            var processingCount = SamplesCollected > 3 ? 3 : SamplesCollected;
            SamplesProcessed += processingCount;
            SamplesCollected -= processingCount;
            return IsHalted;
        }

        public Boolean IsHalted => Power == 0 || MovesLeft == 0;

        private void ThrowIfHalted()
        {
            if (MovesLeft == 0)
                throw new OutOfMovesException();
            if (Power == 0)
                throw new OutOfPowerException();
        }
    }
}
