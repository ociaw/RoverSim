using System;
using System.Collections.Generic;

namespace RoverSim.ScratchAis
{
    /// <summary>
    /// Basic, extremely inefficient. The example AI and starting point for further development.
    /// </summary>
    public sealed class RandomAi : IScratchAi
    {
        private readonly Int32 _seed;
        private readonly Random _random;

        public RandomAi(Int32 seed)
        {
            _seed = seed;
            _random = new Random(_seed);
        }

        public IScratchAi CloneFresh() => new RandomAi(_seed);

        public IEnumerable<RoverAction> Simulate(ScratchRover rover)
        {
            while (true)
            {
                yield return RoverAction.CollectSample;
                yield return RoverAction.ProcessSamples;
                yield return RoverAction.Transmit;

                Int32 num = _random.Next(0, 4);
                if (num == 0)
                    yield return new RoverAction(Direction.Up);
                else if (num == 1)
                    yield return new RoverAction(Direction.Right);
                else if (num == 2)
                    yield return new RoverAction(Direction.Down);
                else if (num == 3)
                    yield return new RoverAction(Direction.Left);

                if (rover.IsHalted)
                    yield break;

                yield return RoverAction.CollectPower;
            }
        }
    }
}
