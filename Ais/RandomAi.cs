using System;

namespace MarsRoverScratch.Ais
{
    /// <summary>
    /// Basic, extremely inefficient. The example AI and starting point for further development.
    /// </summary>
    public sealed class RandomAi : IAi
    {
        private readonly Random _random;

        public RandomAi(Int32 identifier, Random random)
        {
            Identifier = identifier;
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public Int32 Identifier { get; }

        public Boolean Step(Rover rover)
        {
            if (rover == null)
                throw new ArgumentNullException(nameof(rover));

            if (rover.CollectSample()) return true;
            if (rover.ProcessSamples()) return true;
            if (rover.Transmit()) return true;

            Int32 num = _random.Next(0, 4);
            if (num == 0)
                rover.Move(Direction.Up);
            else if (num == 1)
                rover.Move(Direction.Right);
            else if (num == 2)
                rover.Move(Direction.Down);
            else if (num == 3)
                rover.Move(Direction.Left);

            if (rover.IsHalted)
                return true;

            return rover.CollectPower();
        }
    }
}
