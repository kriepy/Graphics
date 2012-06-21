using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GraphicsPractical3
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Often used XNA objects
        GraphicsDevice          device;
        GraphicsDeviceManager   graphics;
        SpriteBatch             spriteBatch;
        FrameRateCounter        frameRateCounter;

        // Game objects and variables
        Camera camera;
        Vector3 camEye = new Vector3(0, 50, 300);

        // Model
        Effect[] effect = new Effect[3];
        Model model;
        Material modelMaterial;

        // Quad
        VertexPositionNormalTexture[] quadVertices;
        short[] quadIndices;
        Matrix quadTransform;

        //   for rotation
        float rotationAmount = 0;
        int ExcNum = 2;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // Create and add a frame rate counter
            frameRateCounter = new FrameRateCounter(this);
            Components.Add(frameRateCounter);
        }




        protected override void Initialize()
        {
            device = graphics.GraphicsDevice;
            // Copy over the device's rasterizer state to change the current fillMode
            device.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
            // Set up the window
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            // Let the renderer draw and update as often as possible
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            // Flush the changes to the device parameters to the graphics card
            graphics.ApplyChanges();
            // Initialize the camera
            camera = new Camera(camEye, new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            IsMouseVisible = true;

            base.Initialize();
        }




        protected override void LoadContent()
        {
            // Create a SpriteBatch object
            spriteBatch = new SpriteBatch(device);
            // Load the "Simple" effect
            effect[0] = Content.Load<Effect>("Effect/CookTorrance");
            effect[1] = Content.Load<Effect>("Effect/MultiLight");
            effect[2] = Content.Load<Effect>("Effect/Spotlight");
            // Load the model and let it use the "Simple" effect
            model = Content.Load<Model>("Model/femalehead");
            
            // Setup the quad
            //SetupQuad();

            //font
            //font = Content.Load<SpriteFont>("myFont");
        }





        protected override void Update(GameTime gameTime)
        {
            model.Meshes[0].MeshParts[0].Effect = effect[ExcNum];
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds * 60.0f;

            //Keyboard usage
            KeyboardState KeyState = Keyboard.GetState();

            //Change Rotation using  the 'left' and 'right' keys
            if (KeyState.IsKeyDown(Keys.Left))
            {
                rotationAmount = rotationAmount + timeStep / 100;
            }

            if (KeyState.IsKeyDown(Keys.Right))
            {
                rotationAmount = rotationAmount - timeStep / 100;
            }

            // Update the window title
            Window.Title = "XNA Renderer | FPS: " + frameRateCounter.FrameRate;

            base.Update(gameTime);
        }




        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen in a predetermined color and clear the depth buffer
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);
            Matrix World = Matrix.CreateScale(3.0f);
            Matrix Rotate = Matrix.CreateRotationY((float)Math.PI * rotationAmount);
            World = Rotate * World;

            // Get the model's only mesh
            ModelMesh mesh = model.Meshes[0];
            Effect effect = mesh.Effects[0];
            
            switch (ExcNum)
            {
                case 0:
                    // Set the effect parameters
                    effect.Parameters["AmbientColor"].SetValue(Color.Red.ToVector4());
                    effect.CurrentTechnique = effect.Techniques["Simple"];
                    // Matrices for 3D perspective projection
                    camera.SetEffectParameters(effect);

            
                    //modelMaterial.SetEffectParameters(effect);
                    effect.Parameters["AmbientColor"].SetValue(Color.Red.ToVector4());
                    effect.Parameters["AmbientIntensity"].SetValue(0.3f);
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector4());
                    effect.Parameters["Eye"].SetValue(camEye);
                    effect.Parameters["World"].SetValue(World);
                    effect.Parameters["InvTransWorld"].SetValueTranspose(Matrix.Invert(World));     
                    break;
                case 1:
                    effect.CurrentTechnique = effect.Techniques["MultiLight"];
                    effect.Parameters["LightColor"].SetValue(Color.Red.ToVector4());
                    effect.Parameters["LightColor2"].SetValue(Color.Blue.ToVector4());
                    effect.Parameters["World"].SetValue(World);
                    effect.Parameters["InvTransWorld"].SetValueTranspose(Matrix.Invert(World));
                    effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
                    camera.SetEffectParameters(effect);
                    break;

                case 2:
                    effect.CurrentTechnique = effect.Techniques["Spotlight"];
                    camera.SetEffectParameters(effect);
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector4());
                    effect.Parameters["Eye"].SetValue(camEye);
                    effect.Parameters["World"].SetValue(World);
                    effect.Parameters["InvTransWorld"].SetValueTranspose(Matrix.Invert(World));
                    break;
                    // do nothing for now
            }
            // Draw the model
            mesh.Draw();

            base.Draw(gameTime);
        }
    }
}
