# RoverSim
RoverSim is what you might an AI test-bed or playground and is the result of taking a homework
assignment a few steps too far. Based off of a [Scratch](https://scratch.mit.edu/) [project](),
this is a complete rewrite in C#. Essentially, the goal is to write an AI that can collect,
process, and finally transmit the most soil samples on the surface of Mars before it runs out of
*moves* or *power*. The surface is represented by a grid with Smooth, Rough, and Impassable tiles.

![Screenshot of the original Scratch project](https://ociaw.com/assets/img/rover-scratch-terrain.png)

More information about the project is available [here](https://ociaw.com/posts/rover), and a
detailed description of each AI is available [here](https://ociaw.com/rover-ai).

# Running
All releases are available [on GitHub](https://github.com/ociaw/RoverSim/releases). However, for
the most up-to-date code, you should clone this repository and build from source.

Run either `WinFormsClient.exe` or `RoverSim.AvaloniaHost.exe`, depending on which host
you're using.

# Create an AI
A typical AI will implement `IAi` through `Simulate(IRover rover)`. All interaction is through
the `IRover` instance, which provides methods to sense, sample, and move around terrain, and
tracks the position of the rover. `IAiFactory` must also be implemented to create instances of
the AI.

Once that's done, the factory needs to be added to the list of available AIs in each client.
This is in `WinFormsClient.WorkManager.GetAIs` and `AvaloniaHost.Program.GetAvailableAis`.

## Scratch AI
Scratch AIs are restricted to a Scratch-like interface similar to the original in functionality.
Specifically, `ScratchRover` does not provide a way to get the current coordinates - the AI must
handle this by itself. Likewise, however, the AI may assume that it it will be running on the
default simulation parameters, such as starting position and level dimensions. To write a
Scratch AI, implement `IScratchAi` instead, which is provided with a `ScratchRover` instance.
Then implement `IAiFactory` as usual, but instead wrapping the `IScratchAi` instance with an
instance of `ScratchAiWrapper` and returning it instead.


# Project Structure
## Core
As its name implies, `Core` is the heart of the project. `Core` defines the basic types, such as
`IRover` and `IAi`, as well as simulation and terrain types. It also has classes to generate
levels and to facilitate running millions of simulations using TPL dataflow. Everything else in
the solution depends on `Core`.

## AIs
`Ais` contains AIs written against the modern API. Currently, there is only `FixedStateAi`, an
AI that tries to make the most out of a fixed amount of memory.

## Scratch AIs
`ScratchAis` has all of the AIs originally written and destined for Scratch. This includes
* `IntelligentRandom` - always moves towards an adjacent smooth tile, or randomly otherwise.
* `Mark I` - similar to `IntelligentRandom`, but tries to get unstuck deterministically instead of
randomly. The predecessor to `FixedStateAi`.
* `Mark II` - uses Dijkstra's algorithm to always find the closest smooth tile.

## Windows Forms Client
`WinFormsClient` is a client built on Windows Forms (surprise!). With it, you can run millions of
simulations and watch how the rover behaves on the worst one of the lot.

## Avalonia Client
`AvaloniaHost` is a client built with the [Avalonia](http://avaloniaui.net/) UI framework. It aims
to be a cross-platform client developed using modern UI techniques and patters. However, it is
still a work in progress, with proper simulation rendering being the next goal.

# TODO
* Simulation rendering in Avalonia Client
* Tests
* Improved Pathfinding AI
* Runtime Selectable Level Generation
* Web Host (Possibly using Blazor)
* Reproducible RNG
* Move this TODO list to GitHub issues

## Simulation rendering in Avalonia Client
The Avalonia Client currently can render terrain correctly. However, it does not have the ability
to simulate the rover and update the terrain as it progresses. This will happen once I can figure
out how to properly do this using ReactiveUI/ReactiveX.

## Tests
`Core` should have unit tests to help ensure correct behavior. AIs themselves probably don't need
tests however, and clients are generally just a wrapper around `Core` anyway.

## Improved Pathfinding AI
This AI will bring the pathfinding of the Mark II to the modern API. It'll cache pathfinding
results to make simulation much quicker, and it will be more intelligent about where it decides to
go.

## Selectable Level Generation
`Core` currently offers two different level generators - `DefaultLevelGenerator` and
`MazeLevelGenerator`. At the moment, these can only be switched at compile time by changing the
referenced class. This should be a runtime setting, using a dropdown box or something similar.

## Web Host (Possibly using Blazor)
I'd like to be able to view and share AI on the internet easily. Ideally, I could use Blazor to
serve the assemblies and do all the work on the browser so I could just host it on like any old
static site. If necessary, however, an ASP.NET Core API could be created to do simulations and
then render the results clientside using ordinary JavaScript.

## Reproducible RNG
Currently we use `System.Random`. However, while it does take a seed, `Random` isn't actually
guaranteed to return the same numbers across different versions of the runtime. In addition, it
has flaws that mean the numbers it generates aren't as high quality as intended. This makes a
strong case for moving to a high quality, reproducible RNG instead.
