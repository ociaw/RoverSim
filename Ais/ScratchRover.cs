using System;

namespace RoverSim.ScratchAis
{
    public sealed class ScratchRover
    {
        public ScratchRover(IRover rover)
        {
            Rover = rover ?? throw new ArgumentNullException(nameof(rover));
        }

        private IRover Rover { get; }

        public Int32 MovesLeft => Rover.MovesLeft;

        public Int32 Power => Rover.Power;

        public Int32 SamplesCollected => Rover.SamplesCollected;

        public Int32 SamplesProcessed => Rover.SamplesProcessed;

        public Int32 SamplesTransmitted => Rover.SamplesTransmitted;

        public Int32 NoBacktrack => Rover.NoBacktrack;

        public TerrainType Sense { private set; get; } = TerrainType.Smooth;

        public Boolean IsHalted => Rover.IsHalted;

        public Boolean CollectPower()
        {
            Rover.CollectPower();
            return Rover.IsHalted;
        }

        public Boolean CollectSample()
        {
            Rover.CollectSample();
            return Rover.IsHalted;
        }

        public Boolean Move(Direction direction)
        {
            Rover.Move(direction);
            return Rover.IsHalted;
        }

        public Boolean ProcessSamples()
        {
            Rover.ProcessSamples();
            return Rover.IsHalted;
        }

        public void SenseSquare(Direction direction)
        {
            Sense = Rover.SenseSquare(direction);
        }

        public Boolean Transmit()
        {
            Rover.Transmit();
            return Rover.IsHalted;
        }
    }
}
