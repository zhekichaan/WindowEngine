# Assignment 9

## The collision detection method used

For this assignment, I used AABB collision detection for all World Objects in my scene.

## How your collision and movement integration works

Basically, each object has a bounding box (a house has several), which works as an invisible border around that object.   
A player also has this box, and the point of collision is to check whether the player's box intersects any of the game objects.  
When that intersection happens, any movement in the direction of a bounding box gets blocked, but you can still slide next to it.

## Any challenges encountered and how you solved them

A couple of challenges for me were to implement a door and to have all walls in the house to have collision. For the door, I found a free door model online, which I imported into a scene and added collision for it. Then, I decided to try something new and decided to add door-opening functionality by clicking the RMB while looking at the door. To solve this challenge, I added a GetLookedAtDoor function, which gets the camera's forward direction, checks for a distance, and then toggles the door, which opens 90 degrees inside the house. For house walls, instead of making one bounding box for one object, I added a possibility to add several bounding boxes for one object, and then I just manually measured each wall in the house and added the collision boxes for each wall.
