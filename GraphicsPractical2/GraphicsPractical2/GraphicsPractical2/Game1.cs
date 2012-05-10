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

namespace GraphicsPractical2
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Often used XNA objects
        GraphicsDevice        device;
        GraphicsDeviceManager graphics;
        SpriteBatch           spriteBatch;
        FrameRateCounter      frameRateCounter;

        // Game objects and variables
        Camera camera;
        
        // Model
        Model    model;
        Material modelMaterial;

        // Quad
        VertexPositionNormalTexture[] quadVertices;
        short[] quadIndices;
        Matrix  quadTransform;

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
            camera = new Camera(new Vector3(0, 50, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a SpriteBatch object
            spriteBatch = new SpriteBatch(device);
            // Load the "Simple" effect
            Effect effect = Content.Load<Effect>("Effects/Simple");
            // Load the model and let it use the "Simple" effect
            model = Content.Load<Model>("Models/Teapot");
            model.Meshes[0].MeshParts[0].Effect = effect;
            // Setup the quad
            SetupQuad();
        }

        private void SetupQuad()
        {
            float scale = 50.0f;

            // Normal points up
            Vector3 quadNormal = new Vector3(0, 1, 0);

            quadVertices = new VertexPositionNormalTexture[4];
            // Top left
            quadVertices[0].Position = new Vector3(-1, 0, -1);
            quadVertices[0].Normal = quadNormal;
            // Top right
            quadVertices[1].Position = new Vector3(1, 0, -1);
            quadVertices[1].Normal = quadNormal;
            // Bottom left
            quadVertices[2].Position = new Vector3(-1, 0, 1);
            quadVertices[2].Normal = quadNormal;
            // Bottom right
            quadVertices[3].Position = new Vector3(1, 0, 1);
            quadVertices[3].Normal = quadNormal;

            quadIndices = new short[] { 0, 1, 2, 1, 2, 3 };
            quadTransform = Matrix.CreateScale(scale);
        }

        protected override void Update(GameTime gameTime)
        {
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds * 60.0f;

            // Update the window title
            Window.Title = "XNA Renderer | FPS: " + frameRateCounter.FrameRate;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen in a predetermined color and clear the depth buffer
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);
            
            // Get the model's only mesh
            ModelMesh mesh = model.Meshes[0];
            Effect effect = mesh.Effects[0];

            // Set the effect parameters
            effect.CurrentTechnique = effect.Techniques["Simple"];
            // Matrices for 3D perspective projection
            camera.SetEffectParameters(effect);
            effect.Parameters["World"].SetValue(Matrix.CreateScale(10.0f));
            // Draw the model
            mesh.Draw();

            base.Draw(gameTime);
        }
    }
}
