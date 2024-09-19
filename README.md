
For my coding discipline, I emphasis on the following subjects:
- Single responsibility, each object should have a clear domain on what they operate on.
- Prefer composition over inheritance.
- Operation results as values. When possible, functions should return a value that describes the outcome.
- Treating errors as values. Exceptions are cases where it's not recoverable. This is to avoid try/catch and encourage functional programming.
- A separation between data, logic, and presentation.

-----

For the code structure, the dependencies are mostly managed by GameScope class, a dependency injection container derived from VContainer plugin. It serves as an entry point to introduce each system together.

By using dependency injection, it also means the "inversion of control" concept is common. I inject MonoBehaviors/GameObjects into pure C# classes, encapsulate the presentation layer, act as the sole owner of the object. Logic is written exclusively on pure C# code. With that being said, I still don't have much experience with the framework and the design. So the dependencies and the encapsulations are not logically strong. Ideally, most classes should know each other in a form of interface, but I haven't done that yet.

-----

To handle the player's input, I use the input system package provided by Unity.
https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/index.html
We can create an input action asset that will generate a C# code that we can interact with.
I use PlayerInputManager to read the generated class and serve game logic in an asynchronous way using the UniTask plugin. 

------
# Code Components

- GameState manages what happens in the game in an asynchronous manner. It describes how systems interact with each other. Most game logics are in this class.
- BoardManager creates and manages the board. Acting as a hub for other classes to contain information about the board, like in collision checking or moving characters.
- CharacterSpawningManager responsible for spawning characters to the board, based on information described in the game settings.
- HeroRow manages movement of the hero line. It works in tandem with the board manager. Moving each character following the previous coordinate of the hero in front of them.
- The Hero, Enemy, and Obstacle class don't do anything by themselves. They act as proxy objects that indirectly call various systems via dependency injection. For example, calling GetBoardCoordinate on Hero class delegates the responsibility to LocateCharacterHandler that obtains the coordinate from the BoardManager. The point is for the character classes to keep as little information about themselves as possible. BoardManager should be the single source of truth who can dictate the coordinates. So there's no conflicting information.
