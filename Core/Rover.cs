using System;

namespace RoverSim
{
    public sealed class Rover : IRover
    {
        private Int32 _moves;
        private Int32 _power;

        public Rover(MutableLevel level, SimulationParameters parameters)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Position = parameters.InitialPosition;
            MovesLeft = parameters.InitialMovesLeft;
            Power = parameters.InitialPower;
        }

        private MutableLevel Level { get; }

        public SimulationParameters Parameters { get; }

        public Position Position { get; private set; }

        public Int32 SamplesCollected { get; private set; } = 0;
        public Int32 MovesLeft
        {
            get => _moves;
            private set => _moves = value > 0 ? value : 0;
        }

        public Boolean IsHalted => Power == 0 || MovesLeft == 0;

        public Int32 Power
        {
            get => _power;
            private set => _power = value > 0 ? value : 0;
        }
        public Int32 SamplesProcessed { get; private set; }
        public Int32 NoBacktrack { get; private set; } = 1;
        public Int32 SamplesTransmitted { get; private set; }

        public Int32 PotentialLight => NoBacktrack * NoBacktrack * NoBacktrack;

        public TerrainType SenseSquare(Direction direction)
            => Level.GetTerrain(Position + direction);

        public Boolean Move(Direction direction)
        {
            ThrowIfHalted();

            Position newPos = Position + direction;
            TerrainType newTerrain = Level.GetTerrain(newPos);
            if (newTerrain == TerrainType.Impassable)
                return false;

            Position = newPos;
            MovesLeft -= 1;
            Power -= Parameters.GetMovementPowerCost(newTerrain);
            if (newTerrain != TerrainType.Smooth)
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
            Power -= Parameters.TransmitCost;
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
            Power -= Parameters.SampleCost;
            TerrainType terrain = Level.GetTerrain(Position);
            if (terrain != TerrainType.Smooth && terrain != TerrainType.Rough || SamplesCollected >= Parameters.HopperSize)
                return (false, terrain);

            SamplesCollected += 1;
            return (true, Level.SampleSquare(Position));
        }

        public Int32 ProcessSamples()
        {
            ThrowIfHalted();
            
            MovesLeft -= 1;
            Power -= Parameters.ProcessCost;
            var processingCount = SamplesCollected > Parameters.HopperSize ? Parameters.HopperSize : SamplesCollected;
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
