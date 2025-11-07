## New Features
- Jumping by pressing UP arrow
- Punching by pressing Space button
- Added backgroun to the scene and a pole on a foreground to add depth to the game

## State machine
My state machine is an enum that has four states: Idle, Walk, Jump and Punch. By default the state is Idle.  
It is very usefull because you can always check the state of the character to apply physics for example to apply gravity and vertical position when state is Jump

## Challenges
The main challenges for me were to allow pressing Jump only once and freeze animation while in air.   
I solved it by creating a state machine and setting it to Jump until the charater isGrounded. Then just setting frame to last frame of the jump to make it look freezed.  
Additionaly calculating physics was also a challenge. 
Lastly there was a lot of different small issues I had throughout this assignment where the fix was pretty simple yet it took some time to figure out.

## Credits

Sprite: https://spritedatabase.net/file/10828
Background: https://www.artstation.com/artwork/DA88D0
