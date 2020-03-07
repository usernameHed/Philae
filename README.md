# Philae

This project is a big sand box for me, my final goal is to create a mario galaxy like physics engine, and may be some game based on that later.

The direction of the gravity is relative to the environement. I Need to quickly setup in wich direction the gravity is facing for each environement.

To Have a generic aproche, my idea is to have the direction of gravity facing the closest object in the level Design.

Bellow some iteration of that idea:

First, I am calculating closest points to primitives
<br>
<img src="Philae Unity Project/Misc/Pics/ClosestPoints.jpg" width="500">

Overriding primitives option to prevent Closest point calculation from happening to certain location of my level design
<br>
<img src="Philae Unity Project/Misc/Pics/Gravity Override Cube.gif" width="500">

Creation of a line editor, thanks to serializeProperty. Here I am using a cached Matrix TRS from the position of my line.
<br>
<img src="Philae Unity Project/Misc/Pics/Line Editor.gif" width="500">


This editor is of course, compatible with both gravityOverride, and LineMovement:
<img src="Philae Unity Project/Misc/Pics/Line Editor With Gravity.gif" width="500">

