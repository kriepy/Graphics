This is the README file for the 3rd practical assignement of Graphics 2012 
of Toby Hijzen (F113228) and Kristin Rieping (F113559):

We both worked regular together on the project.

We implemented the six easy tasks. Toby did the 5th and 6th excersize. Kristin did the 2nd and 4th. The rest we did together.

If you run the code you can use the z- and x- key to rotate the object for the exercises except for the Frustum culling.
You can use the space bar to cycle through the first four exercises. Use g- and b-key to see the other exercises.


[E1] Cook-Torrance BRDF
We implemented the formula given in the pdf-file to caculate the specular component of the lighting. 
The roughness factor 'm' was set to 0.5f and the reflectance at normal incidence F0 was set to 1.42f

[E2] Spotlight
A spotlight pointed at the middlepoint of the face. The position of the lightsource is (0,0,40) and the cosinus of the angles are
cos(theta) = 0.9 and cos(phi) = 0.8

[E3] Multiple light sources
There are three lightsources in the colors red, blue and green used in the model. Respectively in postions (-20,0,40), (10,-20,80) and (10,40,80)
The number of light sources can changed in the effect file by setting the MAX_LIGHT-value.

[E4] Frustum culling
For this exercise we loaded 200 models and placed them on regular intervals along the x-axis. At the top left corner the amount of models
that are culled is shown. We used a bounding sphere class in the XNA file to check if the models lies in the view frustum.

[E5] Post-processing effect: Simple color filter
Press the g-key to convert the image into gray scale and press again to undo.

[E6] Post-processing effect: Gaussian blur
Press the b-key to blurr the image and again to undo the blurring.