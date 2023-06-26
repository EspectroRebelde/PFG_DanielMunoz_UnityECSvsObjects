# Shooter sample

*This contains the complex behavioral controls for OOP and DOP character controllers.*
<br><br>
`Object Oriented Programming (OOP)`
<br>
`Data Oriented Programming (DOP)`

The goal is to change the controllers to be able to shoot when a predetermined key is pressed.<br>
Since this contains the first complex systems, the `OOP` and `DOP` scenes are separated. <br>

These assets have been chosen as they hold a similar approach to solving the problems.

[Rival](https://github.com/Unity-Technologies/rival-documentation) is the `DOP` example and Unity implemented it as a build-in package.
[Kinematic Character Controller](https://docs.google.com/document/d/1qT71uGaUO4UmK1NbW9-UsrX1G-0dWWQ2JQlKWBH0dIw/edit#heading=h.xehyhisi3yke)
is the `OOP` example which held the closest approach to the `DOP` example.

## OOP scene

This scene contains: <br>

- **ExampleCamera** - A camera that follows the ExampleCharacter.
- **ExampleCharacter** - The embodiment of the player.
- **Player** - A connection point between the player and the character while managing the camera.
- **Floor** - A simple floor to walk on as gravity is in effect.

These are the example prefabs from the `Kinematic Character Controller` asset as it wasn't the focus of this
sample to create nor fully understand the character controller.

### ExampleCamera

It uses the basic `Kinematic Character Controller` asset camera.

### ExampleCharacter

It uses the basic `Kinematic Character Controller` asset character.

### Player

Connects the camera and character together.

## DOP scene

This scene contains a subscene called `ECS` which contains: <br>

- **OrbitCamera** - A camera that orbits around the player.
- **ThirdPersonCharacter** - The embodiment of the player. Holds the capsule itself and an empty game object for the camera focus point.
- **ThirdPersonPlayer** - A connection between the player and the camera.
- **Floor** - A simple floor to walk on as gravity is in effect.

### OrbitCamera

It uses the basic `Rival` asset camera.

### ThirdPersonCharacter

It uses the basic `Rival` asset character.

### ThirdPersonPlayer

Connects the camera and character together.

### ECS Methodology

Needing to understand the insides of this asset more, I've studied how it was done and there are some remarks:

- There is a differentiation between the concepts of `Player`, `Controller` and `Character`.
- The `Player` is the user which inputs we have to process and understand.
- The `Controller` is the processed inputs from the `Player` or `AI` which we use to control the `Character`.
- The `Character` is the actual object that is being controlled and which we want to move and act around.
---
- The systems in play process the input of the player at different steps to ensure the smooth visualization on the character side.
- This means we have a `Fixed` and `Variable` update loop for the systems.
- An example of the `Player` needs is for the *forward* and *right* movement. 
The `Player` needs to know the direction of the camera to know where to move while an `AI` doesn't need this.
The `Character` shouldn't have to worry about this things as it should only move in the direction it is told to.
- This segmentation of the process makes the sections both easier to debug and multi-purpose.

## Added functionality for Testing
The outline Asset was used to start testing the in-game look. <br>
In the `OOP` scene, the asset was as easy to use as dragging one component to the camera and another to the outlineable object.
In the `DOP` scene, said asset didn't work.