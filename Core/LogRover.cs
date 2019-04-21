using System;
using System.Collections.Generic;

namespace MarsRoverScratch
{
    public sealed class LoggingRover : IRover
    {
        private readonly List<RoverAction> _actions = new List<RoverAction>();

        public LoggingRover(IRover wrappedRover)
        {
            WrappedRover = wrappedRover ?? throw new ArgumentNullException(nameof(wrappedRover));
            Actions = _actions.AsReadOnly();
        }

        private IRover WrappedRover { get; }

        public IReadOnlyList<RoverAction> Actions { get; }

        public Int32 MovesLeft => WrappedRover.MovesLeft;

        public Int32 Power => WrappedRover.Power;

        public Int32 SamplesCollected => WrappedRover.SamplesCollected;

        public Int32 SamplesProcessed => WrappedRover.SamplesProcessed;

        public Int32 SamplesTransmitted => WrappedRover.SamplesTransmitted;

        public Int32 NoBacktrack => WrappedRover.NoBacktrack;

        public Boolean IsHalted => WrappedRover.IsHalted;

        public Int32 PosX => throw new NotImplementedException();

        public Int32 PosY => throw new NotImplementedException();

        public Int32 CollectPower()
        {
            Int32 powerAmount = WrappedRover.CollectPower();
            _actions.Add(new RoverAction(Instruction.CollectPower));
            return powerAmount;
        }

        public (Boolean isSuccess, TerrainType newTerrain) CollectSample()
        {
            (Boolean isSuccess, TerrainType newTerrain) = WrappedRover.CollectSample();
            _actions.Add(new RoverAction(Instruction.CollectSample));
            return (isSuccess, newTerrain);
        }

        public Int32 ProcessSamples()
        {
            Int32 processCount = WrappedRover.ProcessSamples();
            _actions.Add(new RoverAction(Instruction.ProcessSamples));
            return processCount;
        }

        public Int32 Transmit()
        {
            Int32 transmitCount = WrappedRover.Transmit();
            _actions.Add(new RoverAction(Instruction.Transmit));
            return transmitCount;
        }

        public Boolean Move(Direction direction)
        {
            Boolean success = WrappedRover.Move(direction);
            _actions.Add(new RoverAction(Instruction.Move, direction));
            return success;
        }

        public TerrainType SenseSquare(Direction direction)
        {
            TerrainType terrain = WrappedRover.SenseSquare(direction);
            _actions.Add(new RoverAction(Instruction.Sense, direction));
            return terrain;
        }
    }
}
