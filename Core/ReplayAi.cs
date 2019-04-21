using System;
using System.Collections.Generic;

namespace MarsRoverScratch
{
    public sealed class ReplayAi : IAi
    {
        public ReplayAi(IEnumerator<RoverAction> actions)
        {
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public Int32 Identifier { get; }

        private IEnumerator<RoverAction> Actions { get; }

        public void Simulate(IRover rover)
        {
            while (!Step(rover)) ;
        }

        public Boolean Step(IRover rover)
        {
            if (!Actions.MoveNext())
                return true;

            RoverAction action = Actions.Current;
            
            switch (action.Instruction)
            {
                case Instruction.CollectPower:
                    rover.CollectPower();
                    break;
                case Instruction.CollectSample:
                    rover.CollectSample();
                    break;
                case Instruction.ProcessSamples:
                    rover.ProcessSamples();
                    break;
                case Instruction.Transmit:
                    rover.Transmit();
                    break;
                case Instruction.Sense:
                    rover.SenseSquare(action.Direction);
                    break;
                case Instruction.Move:
                    rover.Move(action.Direction);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return rover.IsHalted;
        }
    }
}
