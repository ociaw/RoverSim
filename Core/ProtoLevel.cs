using System;

namespace RoverSim
{
    public sealed class ProtoLevel : IEquatable<ProtoLevel>
    {
        public ProtoLevel(ILevelGenerator levelGenerator, Int32 seed)
        {
            LevelGenerator = levelGenerator ?? throw new ArgumentNullException(nameof(levelGenerator));
            Seed = seed;
        }

        public ILevelGenerator LevelGenerator { get; }

        public Int32 Seed { get; }

        public Level Generate() => LevelGenerator.Generate(Seed);

        public Boolean Equals(ProtoLevel other)
        {
            if (other is null)
                return false;

            return LevelGenerator == other.LevelGenerator && Seed == other.Seed;
        }

        public override Boolean Equals(Object obj) => obj is ProtoLevel level && Equals(level);

        public override Int32 GetHashCode() => HashCode.Combine(LevelGenerator, Seed);

        public static Boolean operator ==(ProtoLevel left, ProtoLevel right) => left?.Equals(right) ?? right is null;

        public static Boolean operator !=(ProtoLevel left, ProtoLevel right) => !(left == right);
    }
}
