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
`Ais` contains AIs written against the modern API. Currently, there is only `LimitedStateAi`, an
AI that tries to make the most out of a limited amount of memory.

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
still a work in progress, with proper simulation rendering being the next goal.
