using System;

namespace RoverSim
{
    public readonly struct Update : IEquatable<Update>
    {
        public Update(Int32 moveDelta = 0, Int32 powerDelta = 0, CoordinatePair positionDelta = default, Int32 hopperDelta = 0, Int32 pendingTransmissinDelta = 0, Int32 transmittedDelta = 0, Int32 noBacktrackDelta = 0, AdjacentTerrain? terrain = null)
        {
            MoveDelta = moveDelta;
            PowerDelta = powerDelta;
            PositionDelta = positionDelta;
            HopperDelta = hopperDelta;
            PendingTransmissionDelta = pendingTransmissinDelta;
            TransmittedDelta = transmittedDelta;
            NoBacktrackDelta = noBacktrackDelta;
            Terrain = terrain;
        }

        public Int32 MoveDelta { get; }

        public Int32 PowerDelta { get; }

        public CoordinatePair PositionDelta { get; }

        public Int32 HopperDelta { get; }

        public Int32 PendingTransmissionDelta { get; }

        public Int32 TransmittedDelta { get; }

        public Int32 NoBacktrackDelta { get; }

        public AdjacentTerrain? Terrain { get; }

        public static Update NoChange { get; } = default;

        public override String ToString()
        {
            if (Equals(NoChange))
                return "No change";

            return String.Join(", ",
                MoveDelta == 0 ? "" : "Moves " + MoveDelta,
                PowerDelta == 0 ? "" : "Power " + PowerDelta,
                PositionDelta == default ? "" : "POS " + PositionDelta,
                HopperDelta == 0 ? "" : "Hopper " + HopperDelta,
                PendingTransmissionDelta == 0 ? "" : "Pending " + PendingTransmissionDelta,
                TransmittedDelta == 0 ? "" : "Transmitted " + TransmittedDelta,
                NoBacktrackDelta == 0 ? "" : "Backtrack " + NoBacktrackDelta,
                !Terrain.HasValue ? "" : "Terrain"
            );
        }

        public Boolean Equals(Update other) =>
            MoveDelta == other.MoveDelta &&
            PowerDelta == other.PowerDelta &&
            PositionDelta == other.PositionDelta &&
            HopperDelta == other.HopperDelta &&
            PendingTransmissionDelta == other.PendingTransmissionDelta &&
            TransmittedDelta == other.TransmittedDelta &&
            NoBacktrackDelta == other.NoBacktrackDelta &&
            Terrain == other.Terrain;

        public override Boolean Equals(Object obj) => obj is Update update && Equals(update);

        public override Int32 GetHashCode() => HashCode.Combine(MoveDelta, PowerDelta, PositionDelta, HopperDelta, PendingTransmissionDelta, TransmittedDelta, NoBacktrackDelta, Terrain);

        public static Boolean operator ==(Update left, Update right) => left.Equals(right);

        public static Boolean operator !=(Update left, Update right) => !(left == right);
    }
}
