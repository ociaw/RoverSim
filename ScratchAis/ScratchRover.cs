using System;

namespace RoverSim.ScratchAis
{
    public sealed class ScratchRover
    {
        public ScratchRover(IRoverStatusAccessor rover)
        {
            Rover = rover ?? throw new ArgumentNullException(nameof(rover));
        }

        private IRoverStatusAccessor Rover { get; }

        public Int32 MovesLeft => Rover.MovesLeft;

        public Int32 Power => Rover.Power;

        public Int32 SamplesCollected => Rover.SamplesCollected;

        public Int32 SamplesProcessed => Rover.SamplesProcessed;

        public Int32 SamplesTransmitted => Rover.SamplesTransmitted;

        public Int32 NoBacktrack => Rover.NoBacktrack;

        public TerrainType Sense { get; private set; } = TerrainType.Smooth;

        public Boolean IsHalted => Rover.IsHalted;

        public void SenseSquare(Direction direction)
        {
            Sense = Rover.Adjacent[direction];
        }
    }
}
