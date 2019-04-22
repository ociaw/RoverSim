using System;

namespace RoverSim
{
    public struct RoverAction
    {
        public RoverAction(Instruction instruction, Direction direction)
        {
            Instruction = instruction >= 0 && instruction <= Instruction.Move ? instruction : throw new ArgumentOutOfRangeException();
            Direction = direction >= 0 && direction <= Direction.Left ? direction : throw new ArgumentOutOfRangeException();
        }

        public RoverAction(Instruction instruction)
        {
            Instruction = instruction >= 0 && instruction <= Instruction.Move ? instruction : throw new ArgumentOutOfRangeException();
            Direction = Direction.None;
        }

        public Instruction Instruction { get; }

        public Direction Direction { get; }

        public override Int32 GetHashCode() => (Int32)Instruction << 4 | (Int32)Direction;

        public override Boolean Equals(Object obj)
        {
            if (!(obj is RoverAction action))
                return false;

            return Instruction == action.Instruction && Direction == action.Direction;
        }
    }
}
