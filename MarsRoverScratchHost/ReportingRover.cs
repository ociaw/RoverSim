using System;
using System.Threading;

namespace RoverSim.WinFormsClient
{
    public sealed class ReportingRover : IRover
    {
        public ReportingRover(
            IRover rover,
            IProgress<TerrainUpdate> terrainUpdateProgress,
            IProgress<PositionUpdate> positionUpdateProgress,
            IProgress<StatsUpdate> statsUpdateProgress,
            CancellationToken cancellationToken
        )
        {
            WrappedRover = rover ?? throw new ArgumentNullException(nameof(rover));
            TerrainUpdateProgress = terrainUpdateProgress ?? throw new ArgumentNullException(nameof(terrainUpdateProgress));
            PositionUpdateProgress = positionUpdateProgress ?? throw new ArgumentNullException(nameof(positionUpdateProgress));
            StatsUpdateProgress = statsUpdateProgress ?? throw new ArgumentNullException(nameof(statsUpdateProgress));
            CancellationToken = cancellationToken;
        }

        private IRover WrappedRover { get; }

        private IProgress<TerrainUpdate> TerrainUpdateProgress { get; }

        private IProgress<PositionUpdate> PositionUpdateProgress { get; }

        private IProgress<StatsUpdate> StatsUpdateProgress { get; }

        private CancellationToken CancellationToken { get; }

        public Position Position => WrappedRover.Position;

        public Int32 MovesLeft => WrappedRover.MovesLeft;

        public Int32 Power => WrappedRover.Power;

        public Int32 SamplesCollected => WrappedRover.SamplesCollected;

        public Int32 SamplesProcessed => WrappedRover.SamplesProcessed;

        public Int32 SamplesTransmitted => WrappedRover.SamplesTransmitted;

        public Int32 NoBacktrack => WrappedRover.NoBacktrack;

        public Boolean IsHalted => WrappedRover.IsHalted;

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
            StatsUpdateProgress.Report(StatsUpdate);
            return power;
        }

        public (Boolean isSuccess, TerrainType newTerrain) CollectSample()
        {
            (Boolean isSuccess, TerrainType newTerrain) = WrappedRover.CollectSample();

            CancellationToken.ThrowIfCancellationRequested();
            if (isSuccess)
                TerrainUpdateProgress.Report(new TerrainUpdate(Position, newTerrain));
            StatsUpdateProgress.Report(StatsUpdate);

            return (isSuccess, newTerrain);
        }

        public Boolean Move(Direction direction)
        {
            Position previous = Position;
            Boolean isSuccess = WrappedRover.Move(direction);

            CancellationToken.ThrowIfCancellationRequested();
            if (isSuccess)
                PositionUpdateProgress.Report(new PositionUpdate(previous, Position));
            StatsUpdateProgress.Report(StatsUpdate);

            Thread.Sleep(100);
            return isSuccess;
        }

        public Int32 ProcessSamples()
        {
            Int32 samples = WrappedRover.ProcessSamples();
            StatsUpdateProgress.Report(StatsUpdate);
            return samples;
        }

        public TerrainType SenseSquare(Direction direction)
        {
            TerrainType terrain = WrappedRover.SenseSquare(direction);
            var updatedCoords = Position + direction;
            if (!updatedCoords.IsNegative)
                TerrainUpdateProgress.Report(new TerrainUpdate(new Position(updatedCoords), terrain));
            StatsUpdateProgress.Report(StatsUpdate);
            return terrain;
        }

        public Int32 Transmit()
        {
            Int32 transmitted = WrappedRover.Transmit();
            StatsUpdateProgress.Report(StatsUpdate);
            return transmitted;
        }
    }
}
