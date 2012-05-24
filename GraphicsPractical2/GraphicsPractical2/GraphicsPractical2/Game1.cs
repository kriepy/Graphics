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

        //Texture
        Texture texture;
        

        // Quad
        VertexPositionNormalTexture[] quadVertices;
        short[] quadIndices;
        Matrix  quadTransform;
        VertexDeclaration quadVertexDecl;

        //Texture
        Texture2D texture;

        //Post Processing
        private Effect postEffect;

        // Create a new render target
        RenderTarget2D renderTarget;

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

            // initialize render target
            renderTarget = new RenderTarget2D( device, device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight, false, device.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a SpriteBatch object
            spriteBatch = new SpriteBatch(device);
            // Load the "Simple" effect
            Effect effect = Content.Load<Effect>("Effects/Simple");
            // Load the "PostProcessing" effect
            postEffect = Content.Load<Effect>("Effects/PostProcessing");
            // Load the model and let it use the "Simple" effect
            model = Content.Load<Model>("Models/Teapot");
            texture = Content.Load<Texture>("Textures/CobblestonesDiffuse");
            model.Meshes[0].MeshParts[0].Effect = effect;
            // Setup the quad
            SetupQuad();
            // load texture
            texture = Content.Load<Texture2D>("Textures/CobblestonesDiffuse");
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
            quadVertices[0].TextureCoordinate = new Vector2(0.0f, 0.0f);
            // Top right
            quadVertices[1].Position = new Vector3(1, 0, -1);
            quadVertices[1].Normal = quadNormal;
            quadVertices[1].TextureCoordinate = new Vector2(1.0f, 0.0f);
            // Bottom left
            quadVertices[2].Position = new Vector3(-1, 0, 1);
            quadVertices[2].Normal = quadNormal;
            quadVertices[2].TextureCoordinate = new Vector2(0.0f, 1.0f);
            // Bottom right
            quadVertices[3].Position = new Vector3(1, 0, 1);
            quadVertices[3].Normal = quadNormal;
            quadVertices[3].TextureCoordinate = new Vector2(1.0f, 1.0f);

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
            // set render target
            device.SetRenderTarget(renderTarget);
            device.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Clear the screen in a predetermined color and clear the depth buffer
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);
            
            // Get the model's only mesh
            ModelMesh mesh = model.Meshes[0];
            Effect effect = mesh.Effects[0];

            // Set the effect parameters
            effect.CurrentTechnique = effect.Techniques["Simple"];
            // Matrices for 3D perspective projection
            camera.SetEffectParameters(effect);

            Matrix World = Matrix.CreateScale(10.0f);//, 6.5f, 2.5f);
            effect.Parameters["World2"].SetValueTranspose(Matrix.Invert(World));
            effect.Parameters["World"].SetValue(World);
            //Material effecten of zo
            //modelMaterial.SetEffectParameters(effect);

            effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector4());
            effect.Parameters["AmbientColor"].SetValue(Color.Red.ToVector4());
            effect.Parameters["AmbientIntensity"].SetValue(0.2f);
            effect.Parameters["SpecularColor"].SetValue(Color.White.ToVector4());
            effect.Parameters["SpecularIntensity"].SetValue(2.0f);
            effect.Parameters["SpecularPower"].SetValue(25.0f);

            //effecten voor texture
            effect.Parameters["Texture"].SetValue(texture);
            effect.Parameters["quadTransform"].SetValue(quadTransform);

            //set effecten voor model
            effect.Parameters["Shading"].SetValue(true);
            effect.Parameters["Move"].SetValue(new Vector4(0, 0, 0, 0));
            // Draw the model
            mesh.Draw();

            //set effecten voor underground
            effect.Parameters["Shading"].SetValue(false);
            effect.Parameters["Move"].SetValue(new Vector4(0, -0.5f, 0, 0));
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) { pass.Apply(); }
            // Draw the underground
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, quadVertices, 0, 4, quadIndices, 0, 2);
            
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                    SamplerState.LinearClamp, DepthStencilState.Default,
                    RasterizerState.CullNone, postEffect);
            
            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, 800, 600), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
