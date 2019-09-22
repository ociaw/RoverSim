using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using ReactiveUI;
using RoverSim.Rendering;
using System.Reactive;

namespace RoverSim.AvaloniaHost.ViewModels
{
    public sealed class RenderViewModel : ViewModelBase
    {
        private readonly CancellationTokenSource _source = new CancellationTokenSource();
        private ReactiveVisibleState _state;

        public RenderViewModel(IAiFactory ai, CompletedSimulation simulation)
        {
            Ai = ai ?? throw new ArgumentNullException(nameof(ai));
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
            Start = ReactiveCommand.CreateFromObservable(() => StartSim(_source.Token));
            _state = new ReactiveVisibleState(VisibleState.GenerateBlank(simulation.OriginalLevel.BottomRight, simulation.Parameters.InitialPosition));
            Start.Where(union => union.IsT0).Select(union => union.AsT0).Subscribe(Observer.Create<TerrainUpdate>(State.UpdateTerrain));
            Start.Where(union => union.IsT1).Select(union => union.AsT1).Subscribe(Observer.Create<PositionUpdate>(State.UpdateRoverPos));
        }

        public IAiFactory Ai { get; }

        public CompletedSimulation Simulation { get; }

        public ReactiveVisibleState State
        {
            get => _state;
            set => this.RaiseAndSetIfChanged(ref _state, value);
        }

        public ReactiveCommand<Unit, OneOf<TerrainUpdate, PositionUpdate, StatsUpdate>> Start { get; }

        public IObservable<OneOf<TerrainUpdate, PositionUpdate, StatsUpdate>> StartSim(CancellationToken token)
        {
            IAi ai = Ai.Create(Simulation.AiIdentifier, Simulation.Parameters);
            Level originalLevel = Simulation.OriginalLevel;
            MutableLevel workingLevel = originalLevel.AsMutable();
            
            return CreateObservable<OneOf<TerrainUpdate, PositionUpdate, StatsUpdate>>(progress =>
            {
                IRover rover = new ObservableRover(
                    new Rover(workingLevel, Simulation.Parameters),
                    progress,
                    token
                );
                Simulation sim = new Simulation(originalLevel, Simulation.Parameters, ai, rover);
                return Task.Run(() => 
                {
                    try
                    {
                        sim.Simulate();
                    }
                    catch (OperationCanceledException)
                    {
                        // Ignore this exception, since it'll only happen when we've already closed the form
                    }
                    catch (OutOfPowerOrMovesException)
                    {
                        // This is to be expected if an AI doesn't keep track of their power or moves
                    }
                }, token);
            });
        }

        private static IObservable<T> CreateObservable<T>(Func<IProgress<T>, Task> action)
        {
            return Observable.Create<T>(async obs =>
            {
                await action(new DelegateProgress<T>(obs.OnNext));
                obs.OnCompleted();
                //No apparent cancellation support.
                return Disposable.Empty;
            });
        }

        private sealed class DelegateProgress<T> : IProgress<T>
        {
            public DelegateProgress(Action<T> reportee = null) => Reportee = reportee;

            public Action<T> Reportee { get; set; }

	        public void Report(T value) => Reportee(value);
        }
    }
}
