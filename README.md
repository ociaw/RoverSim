RoverSim is the result of a homework assignment taken several steps too far. Based off of a
[Scratch](https://scratch.mit.edu/) [project](), this is a complete rewrite in C#. Essentially,
the goal is to write an AI that can collect, process, and transmit the most soil samples on the
surface of Mars before it runs out of *moves* or *power*. The surface is represented by a grid with Smooth, Rough, and Impassable tiles.

![Screenshot of the original Scratch project](https://ociaw.com/assets/img/rover-scratch-terrain.png)

More information about the project is available [here](https://ociaw.com/posts/rover), and a
detailed description of each AI is available [here](https://ociaw.com/rover-ai).

# Running
All releases are available [On GitHub](https://github.com/ociaw/RoverSim/releases). However, for
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
As its name implies, `Core` is the . `Core` defines the basic types, such as `IRover` and `IAi`,
as well as simulation and terrain types. Everything else in the solution depends on `Core`.

## AIs
`Ais` contains AIs written against the modern API. Currently, there is only `FixedStateAi`, an
AI that tries to make the most out of a fixed amount of memory.

## Scratch AIs
`ScratchAis` has all of the AIs originally written and destined for Scratch. This includes
* `IntelligentRandom` - always moves towards an adjacent smooth tile, or randomly otherwise.
* `Mark I` - similar to `IntelligentRandom`, but tries to get unstuck deterministically instead of
randomly. The predecessor to `FixedStateAi`.
* `Mark II` - uses Dijkstra's algorithm to always find the closest smooth tile.

# TODO
* Simulation rendering in Avalonia Host
* Tests
* Improved Pathfinding AI
* Selectable Level Generation
* Blazor Host
