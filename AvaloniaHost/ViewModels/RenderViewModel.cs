using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using RoverSim.Rendering;

namespace RoverSim.AvaloniaHost.ViewModels
{
    public sealed class RenderViewModel : ViewModelBase
    {
        public RenderViewModel(IAiFactory aiFactory, CompletedSimulation simulation)
        {
            Ai = aiFactory ?? throw new ArgumentNullException(nameof(aiFactory));
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

            Start = ReactiveCommand.CreateFromObservable(() =>
            {
                IAi ai = Ai.Create(Simulation.Parameters);
                Level level = Simulation.ProtoLevel.Generate();
                Rover rover = new Rover(level, Simulation.Parameters);

                return Observable.Create<(RoverAction action, Update update)>(obs =>
                {
                    return Task.Run(async () =>
                    {
                        var actionEnumerator = ai.Simulate(rover.Accessor).GetEnumerator();
                        while (actionEnumerator.MoveNext() && rover.Perform(actionEnumerator.Current, out Update update))
                        {
                            obs.OnNext((actionEnumerator.Current, update));
                            Int32 delay = actionEnumerator.Current.Instruction switch
                            {
                                Instruction.Move => 75,
                                Instruction.CollectSample => 50,
                                _ => 0
                            };
                            if (delay != 0)
                                await Task.Delay(delay);
                        }
                    });
                });
            });

            Stats = Start.Aggregate(RoverStats.Create(Simulation.Parameters), (s, update) => s.Add(update.action, update.update));

            State = this
                .WhenAnyObservable(m => m.Start)
                .Select(update => update.update)
                .Scan(VisibleState.GenerateBlank(Simulation.Parameters), (state, update) =>
                {
                    state.Apply(update);
                    return state;
                });
        }

        public IAiFactory Ai { get; }

        public CompletedSimulation Simulation { get; }

        public IObservable<VisibleState> State { get; }

        public ReactiveCommand<Unit, (RoverAction action, Update update)> Start { get; }

        public IObservable<RoverStats> Stats { get; }
    }
}
