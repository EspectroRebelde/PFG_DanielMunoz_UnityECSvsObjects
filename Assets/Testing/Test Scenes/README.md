# Creation sample

*These very simple samples demonstrate the basic elements of the Entities API.*

The Data Oriented part is done with Unity ECS (DOTS) and is always executed inside a subscene to hold everything in (a world to manage and not share with the objects). These scenes hold an Entity calles `Execute` which tells which systems will be executed.

## Creation sample

The sample contains a single cube with a smaller child cube. The larger cube has a `RotationSpeedAuthoring` MonoBehavior, which adds a `RotationSpeed` IComponentData to the entity in baking.

At runtime, the `RotationSpeedSystem` spins all entities having the `RotationSpeed` component (in this case, just the single parent cube).

<br>

## Movement sample

This sample is the same as "MainThread", except the `RotationSpeedSystem` now uses a job (an `IJobEntity`) to spin the cube instead of doing so directly on the main thread.

<br>

## Controller sample

This sample is like "MainThread", except the `RotationSpeedSystem` now uses an aspect to move the cube up and down.

<br>

## NAME sample

The sample contains a single non-rendered entity with a `Spawner` component that references a cube prefab.

At runtime, the `SpawnSystem` spawns many instances of the cube prefab and places the instances at random positions. The `RotationSpeedSystem` makes the cubes rotate and fall. The `FallAndDestroySystem` destroys cubes when they fall below y coord 0. Once all cubes are destroyed, `SpawnSystem` will spawn more cubes.

<br>
