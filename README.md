# Philae

This project is a big sand box for me, my final goal is to create a mario galaxy like physics engine, and may be some game based on that later.

The direction of the gravity is relative to the environement. I Need to quickly setup in wich direction the gravity is facing for each environement. To Have a generic aproche, my idea is to have the direction of gravity facing the closest object in the level Design.

First, I am calculating closest points to cylinder primitive:
<br>
<img src="Philae Unity Project/Misc/Pics/Gravity Toward Primitive.gif" width="500">
<br>
Here you see the direction of gravity can be facing:
- the trunk of the cylinder,
- the extremity of the disk top or bottom
- inside the disk top or bottom

Then I need to apply gravity force from multiple target.
For design purpose, the closest primitive apply always the same force X.
Then the other primitive farest from the player will apply there own force, divided by a ratio from the distance.
<br>
Farest the primitive is from the other, less the force will be applied
<br>
<img src="Philae Unity Project/Misc/Pics/Gravity direction with multiples targets.gif" width="500">
<br>


For going further, I need to be able to use only certain part of the primitive to apply the gravity. Therefore I have done a custom editor to be able to manipulate these option directly from editor. I call these option **GravityOverride**
You see bellow that I can choose to not apply gravity when the player is over a certain part of the cylinder:
<br>
<img src="Philae Unity Project/Misc/Pics/Gravity Toward Primitive With Override.gif" width="500">
<br>


I have worked hard to have as many primitive as possible, and for each of them, as many **GravityOverride** as I can.
<br>
<img src="Philae Unity Project/Misc/Pics/Gravity Override Cube.gif" width="500">
<br>
These primitive can be Moved, Rotated and Scaled like a normal gameObject in editor !
<br>
<img src="Philae Unity Project/Misc/Pics/Move Rotate Scale.gif" width="500">
<br>

Here the final list of all primitives:
<br>
<img src="Philae Unity Project/Misc/Pics/ClosestPoints.jpg" width="500">
<br>


I have created a line editor to have custom gravity setup for some level:
<br>
<img src="Philae Unity Project/Misc/Pics/Line Editor.gif" width="500">

This editor is of course, compatible with both **GravityOverride**, and **Line Edition**:
<img src="Philae Unity Project/Misc/Pics/Line Editor With Gravity.gif" width="500">

