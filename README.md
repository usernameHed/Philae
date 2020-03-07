# Philae

This project is a big sand box for me, my final goal is to create a mario galaxy like physics engine, and may be some game based on that later.
<br><br>
<img src="Philae Unity Project/Misc/Pics/Philae.gif" width="290"><img src="Philae Unity Project/Misc/Pics/Philae 2.gif" width="290"><img src="Philae Unity Project/Misc/Pics/Philae 3.gif" width="290">

<br>



The direction of the gravity is relative to the environement. I Need to quickly setup in wich direction the gravity is facing for each environement. To Have a generic aproche, my idea is to have the direction of gravity facing the closest object in the level Design.

First, I am calculating closest points to cylinder primitive:
<br><br>
<img src="Philae Unity Project/Misc/Pics/Gravity Toward Primitive.gif" width="500">
<br>
Here you see the direction of gravity can be facing:
- the trunk of the cylinder,
- the extremity of the disk top or bottom
- inside the disk top or bottom

Then I need to apply gravity force from multiple target. I call them **Attractors**
For design purpose, the closest **Attractors** apply always the same force X.
Then the other **Attractors** farest from the player will apply there own force, divided by a ratio from the distance.
<br>
Farest the primitive is from the other, less the force will be applied
<br><br>
<img src="Philae Unity Project/Misc/Pics/Gravity direction with multiples targets.gif" width="500">
<br>
<br>
<br>

For going further, I need to be able to use only certain part of the **Attractors** to apply the gravity. Therefore I have done a custom editor to be able to manipulate these option directly from editor. I call these option **GravityOverride**
You see bellow that I can choose to not apply gravity when the player is over a certain part of the cylinder:
<br><br>
<img src="Philae Unity Project/Misc/Pics/Gravity Toward Primitive With Override.gif" width="500">
<br>


I have worked hard to have as many **Attractors** as possible, and for each of them, as many **GravityOverride** as I can.
**GravityOverrides** are separate from there **non-GravityOverride** friends, to save performance if we don't want them.
<br><br>
<img src="Philae Unity Project/Misc/Pics/Gravity Override Cube.gif" width="500">
<br>
These **Attractors** can be Moved, Rotated and Scaled like a normal gameObject in editor !
<br><br>
<img src="Philae Unity Project/Misc/Pics/Move Rotate Scale.gif" width="500">
<br><br>

Here the final list of all current **Attractors**:
<br><br>
<img src="Philae Unity Project/Misc/Pics/ClosestPoints.jpg" width="500">
<br>


I have created a line editor to have custom gravity setup for some level. This editor is of course, compatible with both **GravityOverride**, and **Line Edition**
<br><br>
<img src="Philae Unity Project/Misc/Pics/Line Editor.gif" width="436"><img src="Philae Unity Project/Misc/Pics/Line Editor With Gravity.gif" width="436">
<br>
<br>
<br>

To optimize calculation, I need to be able to organize the level design into chunk, and calculate only attractors inside this chunk.
I have create Special **Zone**, which contain a list of **Attractors**:
<br><br>
<img src="Philae Unity Project/Misc/Pics/Zone.gif" width="500">
<br><br>
I can explicitly set a **Zone** inclusive, or exclusive from others:
<br><br>
<img src="Philae Unity Project/Misc/Pics/Zone Substractive.gif" width="500">
<br>
<br>

Here a list of **zone** shape I can choose:
<br>
<img src="Philae Unity Project/Misc/Pics/Zones List.gif" width="500">
<br>
