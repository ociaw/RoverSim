using System;

namespace RoverSim
{
    public readonly struct RoverAction : IEquatable<RoverAction>
    {
        public RoverAction(Direction direction)
        {
            Instruction = Instruction.Move;
            Direction = direction.IsValid ? direction : throw new ArgumentOutOfRangeException(nameof(direction));
        }

        public RoverAction(Instruction instruction)
        {
            Instruction = instruction >= 0 && instruction <= Instruction.Move ? instruction : throw new ArgumentOutOfRangeException(nameof(instruction));
            Direction = Direction.None;
        }

        public Instruction Instruction { get; }

        public Direction Direction { get; }

        public static RoverAction CollectPower { get; } = new RoverAction(Instruction.CollectPower);

        public static RoverAction CollectSample { get; } = new RoverAction(Instruction.CollectSample);

        public static RoverAction ProcessSamples { get; } = new RoverAction(Instruction.ProcessSamples);

        public static RoverAction Transmit { get; } = new RoverAction(Instruction.Transmit);

        public override String ToString() => Instruction.ToString() + (Instruction == Instruction.Move ? " " + Direction.ToString() : "");

        public Boolean Equals(RoverAction other) => Instruction == other.Instruction && Direction == other.Direction;

        public override Boolean Equals(Object obj) => obj is RoverAction action && Equals(action);

        public override Int32 GetHashCode() => HashCode.Combine(Instruction, Direction);

        public static Boolean operator ==(RoverAction left, RoverAction right) => left.Equals(right);

        public static Boolean operator !=(RoverAction left, RoverAction right) => !(left == right);
    }
}
