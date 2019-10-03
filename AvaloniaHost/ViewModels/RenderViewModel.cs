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
                IAi ai = Ai.Create(Simulation.AiIdentifier, Simulation.Parameters);
                Level originalLevel = Simulation.OriginalLevel;
                MutableLevel workingLevel = originalLevel.AsMutable();
                Rover rover = new Rover(workingLevel, Simulation.Parameters);

                return Observable.Create<(RoverAction action, Update update)>(obs =>
                {
                    return Task.Run(async () =>
                    {
                        var actionEnumerator = ai.Simulate(rover.Accessor).GetEnumerator();
                        while (actionEnumerator.MoveNext() && rover.Perform(actionEnumerator.Current, out Update update))
                        {
                            obs.OnNext((actionEnumerator.Current, update));
                            await Task.Delay(100);
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
