using System;

namespace RoverSim
{
    public sealed class Rover
    {
        private Int32 _moves;
        private Int32 _power;

        public Rover(Level level, SimulationParameters parameters)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Position = parameters.InitialPosition;
            MovesLeft = parameters.InitialMovesLeft;
            Power = parameters.InitialPower;
            Adjacent = GetAdjacentTerrain(parameters.InitialPosition);
            Accessor = new StatusAccessor(this);
        }

        private Level Level { get; }

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

        public AdjacentTerrain Adjacent { get; private set; }

        public IRoverStatusAccessor Accessor { get; }

        /// <summary>
        /// Attempts to perform the given action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>Whether or not the rover was halted *before* the action was taken.</returns>
        public Boolean Perform(in RoverAction action, out Update update)
        {
            if (IsHalted)
            {
                update = Update.NoChange;
                return false;
            }

            update = action.Instruction switch
            {
                Instruction.CollectPower => CollectPower(),
                Instruction.CollectSample => CollectSample(),
                Instruction.ProcessSamples => ProcessSamples(),
                Instruction.Transmit => Transmit(),
                Instruction.Move => Move(action.Direction),
                _ => throw new Exception("This is impossible.")
            };

            Apply(update);
            return true;
        }

        private void Apply(in Update update)
        {
            MovesLeft += update.MoveDelta;
            Power += update.PowerDelta;
            Position = new Position(Position + update.PositionDelta);
            SamplesCollected += update.HopperDelta;
            SamplesProcessed += update.PendingTransmissionDelta;
            SamplesTransmitted += update.TransmittedDelta;
            NoBacktrack += update.NoBacktrackDelta;
            Adjacent = update.Terrain ?? Adjacent;
        }

        private Update Move(in Direction direction)
        {
            CoordinatePair newCoords = Position + direction;
            TerrainType newTerrain = Level.GetTerrain(newCoords);
            if (newTerrain == TerrainType.Impassable || newCoords.IsNegative)
                return Update.NoChange;

            return new Update(
                moveDelta: -1,
                powerDelta: -Parameters.GetMovementPowerCost(newTerrain),
                positionDelta: direction.Delta,
                noBacktrackDelta: newTerrain != TerrainType.Smooth ? 1 - NoBacktrack : 1,
                terrain: GetAdjacentTerrain(Position + direction)
            );
        }

        private Update Transmit()
        {
            return new Update(
                moveDelta: -1,
                powerDelta: -Parameters.TransmitCost,
                pendingTransmissinDelta: -SamplesProcessed,
                transmittedDelta: SamplesProcessed
            );
        }

        private Update CollectPower()
        {
            return new Update(
                moveDelta: -1,
                powerDelta: PotentialLight,
                noBacktrackDelta: 1 - NoBacktrack
            );
        }

        private Update CollectSample()
        {
            if (!Adjacent.Occupied.IsSampleable() || SamplesCollected >= Parameters.HopperSize)
                return new Update(moveDelta: -1, powerDelta: Parameters.SampleCost);

            TerrainType newTerrain = Level.SampleSquare(Position);
            return new Update(
                moveDelta: -1,
                powerDelta: -1,
                hopperDelta: 1,
                terrain: new AdjacentTerrain(Adjacent.Up, Adjacent.Right, Adjacent.Down, Adjacent.Left, newTerrain)
            );
        }

        private Update ProcessSamples()
        {
            Int32 processingCount = SamplesCollected > Parameters.HopperSize ? Parameters.HopperSize : SamplesCollected;
            return new Update
            (
                moveDelta: -1,
                powerDelta: -Parameters.ProcessCost,
                hopperDelta: -processingCount,
                pendingTransmissinDelta: processingCount
            );
        }

        private AdjacentTerrain GetAdjacentTerrain(in CoordinatePair position) =>
            new AdjacentTerrain
            (
                Level.GetTerrain(position + Direction.Up),
                Level.GetTerrain(position + Direction.Right),
                Level.GetTerrain(position + Direction.Down),
                Level.GetTerrain(position + Direction.Left),
                Level.GetTerrain(position)
            );


        private sealed class StatusAccessor : IRoverStatusAccessor
        {
            private readonly Rover _rover;

            public StatusAccessor(Rover rover) => _rover = rover ?? throw new ArgumentNullException(nameof(rover));

            public Position Position => _rover.Position;

            public Int32 MovesLeft => _rover.MovesLeft;

            public Int32 Power => _rover.Power;

            public Int32 SamplesCollected => _rover.SamplesCollected;

            public Int32 SamplesProcessed => _rover.SamplesProcessed;

            public Int32 SamplesTransmitted => _rover.SamplesTransmitted;

            public Int32 NoBacktrack => _rover.NoBacktrack;

            public Boolean IsHalted => _rover.IsHalted;

            public AdjacentTerrain Adjacent => _rover.Adjacent;
        }
    }
}
