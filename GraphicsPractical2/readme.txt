This is the README file for the 2nd practical assignement of Graphics 2012 
of Toby Hijzen (F113228) and Kristin Rieping (F113559):


If you run the code you can use the up- and down-arrow keys to switch between the different exercises.
With the left- and right arrow keys you can rotate the teapot.
We both worked regular together on the project.

Exercise 1.1: Coloring Using normals
In the simple.fx file we build the function NormalColor()
In the Game.cs file we pass a boolean if the first exercise must be shown

Exercise 1.2: Checkerboard pattern
In the effect file we add another function ProceduralColor()
This function and the NormalColor() function are both called

Exercise 2.1: Lambertian Shading
In the effect file we implemented the function LambertianShading()
Our light source is a point light at (50,50,50)

Exercise 2.2: Ambient Shading
The color is red and the intensity is 0.2.
We made sure that ambient color is red.
In the game.cs file we used an if statement so that the ambient light is used for this and all following exercises.

Exercise 2.3: Phong-Shading
We actually implemented Bill-Phong-Shading.
All following exercises also have this effect

Exercise 2.4: Non-uniform scaling problem
We scaled the model in the x,y,z-directions with 10,6.5,2.5 respectively.
The scaling is only used in this exercise.

Exercise 3.1: Texturing a quad using U,V-coordinates
Here for we added a new technique in the simple.fx file which we call simple2 technique.
We only return the tex2D for the texture.
In the game.cs file we created a quad to define the size and place of the surface.

BONUS
Exercise 4.1: Gamma Correction
We created a new effect file called PostProcessing.fx. For every RGB value the gamma correction is applied.
The Gamma value is only for this exercise 1.5.

Exercise 4.2: Normal Mapping
Herefor we  used the Lambertian Shader in the simple.fx file on the texture map combined with the normal map of the underground.
