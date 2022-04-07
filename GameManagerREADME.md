GameManager.cs README

This code was used in my capstone project for my Gaming and Simulation Degree. This project was called Dungeon Designer. 
Dungeon Designer was a game where the player could build and play through their own dungeon made in the Unity engine. 
This dungeon was viewed from an Isometric view and the was made up of 3d objects. So this ment that for the first time I 
would have to Dynamically generate a 3D mesh that would update whenever the player would place a new block. It would also
have to place specifically what block they wanted. Think of similar to Minecraft in appearance. 

This code was responsible for handling all processes in the game that required a singleton to manage. This code also allowed
other classes to get access to other singletons like the game manager. This class included methods that did things like 
place blocks, remove blocks, place game objects, remove game objects, place lights, remove lights, save mape data to database,
create new map data, get map data and more.
