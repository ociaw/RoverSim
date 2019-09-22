using System;
using System.Threading;
using OneOf;

namespace RoverSim.Rendering
{
    public sealed class ObservableRover : IRover
    {
        public ObservableRover(
            IRover rover,
            IProgress<OneOf<TerrainUpdate, PositionUpdate, StatsUpdate>> updateProgress,
            CancellationToken cancellationToken
        )
        {
            WrappedRover = rover ?? throw new ArgumentNullException(nameof(rover));
            UpdateProgress = updateProgress ?? throw new ArgumentNullException(nameof(updateProgress));
            CancellationToken = cancellationToken;
        }

        private IRover WrappedRover { get; }

        private IProgress<OneOf<TerrainUpdate, PositionUpdate, StatsUpdate>> UpdateProgress { get; }

        private CancellationToken CancellationToken { get; }

        public Position Position => WrappedRover.Position;

        public Int32 MovesLeft => WrappedRover.MovesLeft;

        public Int32 Power => WrappedRover.Power;

        public Int32 SamplesCollected => WrappedRover.SamplesCollected;

        public Int32 SamplesProcessed => WrappedRover.SamplesProcessed;

        public Int32 SamplesTransmitted => WrappedRover.SamplesTransmitted;

        public Int32 NoBacktrack => WrappedRover.NoBacktrack;

        public Boolean IsHalted => WrappedRover.IsHalted;

        public IObservable<TerrainUpdate> TerrainObservable { get; }

        public StatsUpdate StatsUpdate => new StatsUpdate
        (
            0,
            MovesLeft,
            Power,
            SamplesCollected,
            SamplesProcessed,
            SamplesTransmitted,
            NoBacktrack
        );

        public Int32 CollectPower()
        {
            Int32 power = WrappedRover.CollectPower();
            UpdateProgress.Report(StatsUpdate);
            return power;
        }

        public (Boolean isSuccess, TerrainType newTerrain) CollectSample()
        {
            (Boolean isSuccess, TerrainType newTerrain) = WrappedRover.CollectSample();

            CancellationToken.ThrowIfCancellationRequested();
            if (isSuccess)
                UpdateProgress.Report(new TerrainUpdate(Position, newTerrain));
            UpdateProgress.Report(StatsUpdate);

            return (isSuccess, newTerrain);
        }

        public Boolean Move(Direction direction)
        {
            Position previous = Position;
            Boolean isSuccess = WrappedRover.Move(direction);

            CancellationToken.ThrowIfCancellationRequested();
            if (isSuccess)
                UpdateProgress.Report(new PositionUpdate(previous, Position));
            UpdateProgress.Report(StatsUpdate);

            Thread.Sleep(100);
            return isSuccess;
        }

        public Int32 ProcessSamples()
        {
            Int32 samples = WrappedRover.ProcessSamples();
            UpdateProgress.Report(StatsUpdate);
            return samples;
        }

        public TerrainType SenseSquare(Direction direction)
        {
            TerrainType terrain = WrappedRover.SenseSquare(direction);
            var updatedCoords = Position + direction;
            if (!updatedCoords.IsNegative)
                UpdateProgress.Report(new TerrainUpdate(new Position(updatedCoords), terrain));
            UpdateProgress.Report(StatsUpdate);
            return terrain;
        }

        public Int32 Transmit()
        {
            Int32 transmitted = WrappedRover.Transmit();
            UpdateProgress.Report(StatsUpdate);
            return transmitted;
        }
    }
}
