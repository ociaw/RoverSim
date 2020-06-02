# RoverSim
RoverSim is what you might an AI test-bed or playground and is the result of taking a homework
assignment a few steps too far. Based off of a [Scratch](https://scratch.mit.edu/)
[project](https://scratch.mit.edu/projects/293816651/),
this is a complete rewrite in C#. Essentially, the goal is to write an AI that can collect,
process, and finally transmit the most soil samples on the surface of Mars before it runs out of
*moves* or *power*. The surface is represented by a grid with Smooth, Rough, and Impassable tiles.

![Animation of the limited state AI on a default simulation](https://ociaw.com/assets/img/rover-ai-fixed-state.apng)

More information about the project is available [here](https://ociaw.com/roversim), and a
detailed description of each AI is available [here](https://ociaw.com/roversim/ai).

# Running
All releases are available [on GitHub](https://github.com/ociaw/RoverSim/releases). However, for
the most up-to-date code, you should clone this repository and build from source.

Run either `WinFormsClient.exe` or `RoverSim.AvaloniaHost.exe`, depending on which client
you're using. The Blazor Client isn't able to be run directly, but any HTTP server can host the
site which can be viewed in any web browser supporting WebAssembly.

# Creating an AI
A typical AI will implement `IAi` through `Simulate(IRoverStatusAccessor rover)`. Rover status is
accessed through the `IRoverStatusAccessor` instance, which senses nearby terrain and various
rover information. Actions are taken by `yield return`ing a `RoverAction` - this will be applied
by the caller and changes will be immediately visible through the status accessor. `IAiFactory`
must also be implemented to create instances of the AI.

Once that's done, the factory needs to be added to the list of available AIs in each client.
This is in `WinFormsClient.Program.GetAIs`, `AvaloniaHost.Program.GetAvailableAis`, and the
constructor of `AiProvider`. For Blazor, the AI also needs to be added to the `Renderer.razor`
component if you want it to be selectable.

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
`Ais` contains AIs written against the modern API. Currently, there is only `LimitedStateAi`, an
AI that tries to make the most out of a limited amount of memory, as opposed to mapping out the
entire level as it moves.

## Scratch AIs
`ScratchAis` has all of the AIs originally written and destined for Scratch. This includes
* `IntelligentRandom` - always moves towards an adjacent smooth tile, or randomly otherwise.
* `Mark I` - similar to `IntelligentRandom`, but tries to get unstuck deterministically instead of
randomly. The predecessor to `LimitedStateAi`.
* `Mark II` - uses Dijkstra's algorithm to always find the closest smooth tile.

## Windows Forms Client
`WinFormsClient` is a client built on Windows Forms (surprise!). With it, you can run millions of
simulations and watch how the rover behaves on the worst one of the lot.

## Avalonia Client
`AvaloniaHost` is a client built with the [Avalonia](http://avaloniaui.net/) UI framework. It aims
to be a cross-platform client developed using modern UI techniques and patters. However, it is
still a work in progress, with in depth statistics being the next goal.
