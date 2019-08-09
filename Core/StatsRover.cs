using System;

namespace RoverSim
{
    public sealed class StatsRover : IRover
    {
        private Int32 _collectPowerCallCount;
        private Int32 _powerCumulative;

        private Int32 _collectSampleCallCount;
        private Int32 _smoothSampleCumulative;
        private Int32 _roughSampleCumulative;

        private Int32 _processSamplesCallCount;

        private Int32 _transmitCallCount;

        private Int32 _moveCallCount;
        private Int32 _moveCount;

        private Int32 _senseCallCount;
        
        public StatsRover(IRover wrappedRover)
        {
            WrappedRover = wrappedRover ?? throw new ArgumentNullException(nameof(wrappedRover));
        }

        private IRover WrappedRover { get; }

        public Int32 MovesLeft => WrappedRover.MovesLeft;

        public Int32 Power => WrappedRover.Power;

        public Int32 SamplesCollected => WrappedRover.SamplesCollected;

        public Int32 SamplesProcessed => WrappedRover.SamplesProcessed;

        public Int32 SamplesTransmitted => WrappedRover.SamplesTransmitted;

        public Int32 NoBacktrack => WrappedRover.NoBacktrack;

        public Boolean IsHalted => WrappedRover.IsHalted;

        public Position Position => WrappedRover.Position;

        public RoverStats GetStats() => new RoverStats(
            MovesLeft,
            Power,

            SamplesCollected,
            SamplesProcessed,
            SamplesTransmitted,

            _collectPowerCallCount,
            _powerCumulative,

            _collectSampleCallCount,
            _smoothSampleCumulative,
            _roughSampleCumulative,

            _processSamplesCallCount,

            _transmitCallCount,

            _moveCallCount,
            _moveCount,

            _senseCallCount
        );

        public Int32 CollectPower()
        {
            _collectPowerCallCount++;
            Int32 powerAmount = WrappedRover.CollectPower();
            _powerCumulative += powerAmount;
            return powerAmount;
        }

        public (Boolean isSuccess, TerrainType newTerrain) CollectSample()
        {
            _collectSampleCallCount++;
            (Boolean isSuccess, TerrainType newTerrain) = WrappedRover.CollectSample();
            if (isSuccess)
            {
                if (newTerrain == TerrainType.SampledSmooth)
                    _smoothSampleCumulative++;
                else
                    _roughSampleCumulative++;
            }
            return (isSuccess, newTerrain);
        }

        public Int32 ProcessSamples()
        {
            _processSamplesCallCount++;
            Int32 processCount = WrappedRover.ProcessSamples();
            return processCount;
        }

        public Int32 Transmit()
        {
            _transmitCallCount++;
            Int32 transmitCount = WrappedRover.Transmit();
            return transmitCount;
        }

        public Boolean Move(Direction direction)
        {
            _moveCallCount++;
            Boolean success = WrappedRover.Move(direction);
            if (success)
                _moveCount++;
            return success;
        }

        public TerrainType SenseSquare(Direction direction)
        {
            _senseCallCount++;
            TerrainType terrain = WrappedRover.SenseSquare(direction);
            return terrain;
        }
    }
}
