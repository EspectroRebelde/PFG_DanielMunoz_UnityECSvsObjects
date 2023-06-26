# Movement sample

*These very simple sample demonstrate the basic comparison between independently moving entities and objects.*
<br><br>
`Object Oriented Programming (OOP)`
<br>
`Data Oriented Programming (DOP)`

The goal is to move the previously created prefabs in a given direction and rotate them both with a given speed and rotation speed.

## OOP sample

This sample contains a single Object on the hierarchy called `OOP`. <br>
* **Creation**: This acts as a *Spawner* for the prefab assigned on it and holds the variables to give to the instanced prefabs. `OnAwake` created the prefabs and assigns the values. <br>
* **Movement**: Then, `OnUpdate` they're moved and rotated by reference as we hold them after creation.

## DOP sample

This sample contains a single Entity called `DOP` inside the `ECS` sub-scene. <br>
* **Creation**: This acts as a *Spawner* with the prefab assigned on it and assigns the variables to those instanced prefabs. These are assigned `OnAwake` to a singleton which is accessed by the active System `DOP_SpawnSystem` to assign them as the instances are created.
* **Movement**: Then, a system `DOP_MovementSystem` moves and rotates them querying them by `LocalTransform` after getting the value of the Singleton.
