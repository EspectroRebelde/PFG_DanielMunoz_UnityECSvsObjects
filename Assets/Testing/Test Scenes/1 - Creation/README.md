# Creation sample

*These very simple sample demonstrate the basic comparison between creating entities and objects.*
<br><br>
`Object Oriented Programming (OOP)`
<br>
`Data Oriented Programming (DOP)`

The goal is to instantiate a given number of prefabs with a set position and rotation and the possibility to randomize their spawn position.

## OOP sample

This sample contains a single Object on the hierarchy called `OOP`. <br>
* **Creation**: This acts as a *Spawner* for the prefab assigned on it and holds the variables to give to the instanced prefabs. `OnAwake` created the prefabs and assigns the values.

## DOP sample

This sample contains a single Entity called `DOP` inside the `ECS` sub-scene. <br>
* **Creation**: This acts as a *Spawner* with the prefab assigned on it and assigns the variables to those instanced prefabs. These are assigned `OnAwake` to a singleton which is accessed by the active System `DOP_SpawnSystem` to assign them as the instances are created.
