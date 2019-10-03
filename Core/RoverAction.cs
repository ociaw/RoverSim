using System;

namespace RoverSim
{
    public readonly struct RoverAction
    {
        public RoverAction(Direction direction)
        {
            Instruction = Instruction.Move;
            Direction = direction.IsValid ? direction : throw new ArgumentOutOfRangeException();
        }

        public RoverAction(Instruction instruction)
        {
            Instruction = instruction >= 0 && instruction <= Instruction.Move ? instruction : throw new ArgumentOutOfRangeException();
            Direction = Direction.None;
        }

        public Instruction Instruction { get; }

        public Direction Direction { get; }

        public static RoverAction CollectPower { get; } = new RoverAction(Instruction.CollectPower);

        public static RoverAction CollectSample { get; } = new RoverAction(Instruction.CollectSample);

        public static RoverAction ProcessSamples { get; } = new RoverAction(Instruction.ProcessSamples);

        public static RoverAction Transmit { get; } = new RoverAction(Instruction.Transmit);

        public override String ToString() => Instruction.ToString() + (Instruction == Instruction.Move ? " " + Direction.ToString() : "");

        public override Int32 GetHashCode() => (Int32)Instruction << 4 | (Int32)Direction;

        public override Boolean Equals(Object obj)
        {
            if (!(obj is RoverAction action))
                return false;

            return Instruction == action.Instruction && Direction == action.Direction;
        }
    }
}
