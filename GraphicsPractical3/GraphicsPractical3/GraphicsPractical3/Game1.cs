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
        GraphicsDevice device;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        FrameRateCounter frameRateCounter;

        // Amount of models for the culling exersize
        const int NR_MODELS = 200;
        bool[] InViewFrustum = new bool[NR_MODELS];

        // A font to draw text in the image
        SpriteFont font;
        
        // Game objects and variables
        Camera camera;
        Vector3 camEye = new Vector3(0, 40, 300);

        // Model
        Effect[] effect = new Effect[4];
        Effect effect2;
        Model model;
        Model[] model2 = new Model[NR_MODELS];
        Matrix[] World2n = new Matrix[NR_MODELS];

        // for rotation and translation
        float rotationAmount = 0;

        // Post Proccesing
        private Effect postEffect;

        // Set render target
        RenderTarget2D renderTarget;

        // For switching excersizes
        int ExcNum = 0;
        int maxExc = 3;
        bool postGray = false;
        bool postBlur = false;
        bool Bpressed = false;
        bool Gpressed = false;
        bool SpacePressed = false;

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

            // initialize render target
            renderTarget = new RenderTarget2D(device, device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight, false, device.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            IsMouseVisible = true;

            // generate several world matrices for translating the multiple models for use in the culling exersize
            Matrix World2 = Matrix.CreateScale(2.0f);
            Matrix Rotate2 = Matrix.CreateRotationY((float)Math.PI * rotationAmount);
            Matrix Translate;
            
            for (int i = 0; i < model2.Length; i++)
            {
                Translate = Matrix.CreateTranslation(new Vector3((i - 2) * 150, 0, 0));
                World2n[i] = Rotate2 * World2 * Translate;

            }

            base.Initialize();
        }




        protected override void LoadContent()
        {
            // Create a SpriteBatch object
            spriteBatch = new SpriteBatch(device);
            // Load the "Simple" effect
            effect[0] = Content.Load<Effect>("Effect/CookTorrance");
            effect[1] = Content.Load<Effect>("Effect/Spotlight");
            effect[2] = Content.Load<Effect>("Effect/MultiLight");
            effect[3] = Content.Load<Effect>("Effect/CookTorrance");
            effect2 = Content.Load<Effect>("Effect/CookTorrance");
            // Load the model and let it use the "Simple" effect
            model = Content.Load<Model>("Model/femalehead");
            for(int i=0;i<NR_MODELS;i++){
                model2[i] = Content.Load<Model>("Model/femalehead");
            }
            // Load the "PostProcessing" effect
            postEffect = Content.Load<Effect>("Effect/postProccesing");

            //load font
            font = Content.Load<SpriteFont>("myFont");
        }

        protected override void Update(GameTime gameTime)
        {
            BoundingSphere sphere;
            model.Meshes[0].MeshParts[0].Effect = effect[ExcNum];
            for (int i = 0; i < NR_MODELS; i++)
            {

                sphere = new BoundingSphere();
                sphere = BoundingSphere.CreateMerged(sphere, model2[i].Meshes[0].BoundingSphere);

                InViewFrustum[i] = camera.InView(sphere.Transform(World2n[i]));
            }
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds * 60.0f;

            //Keyboard usage
            KeyboardState KeyState = Keyboard.GetState();

            //Change Rotation using  the 'left' and 'right' keys
            if (KeyState.IsKeyDown(Keys.Z))
            {
                rotationAmount = rotationAmount + timeStep / 100;
            }

            if (KeyState.IsKeyDown(Keys.G))
            {
                if (!Gpressed) { postGray = !postGray; }
                Gpressed = true;
            }
            if (KeyState.IsKeyUp(Keys.G)) { Gpressed = false; }

            if (KeyState.IsKeyDown(Keys.B))
            {
                if (!Bpressed) { postBlur = !postBlur; }
                Bpressed = true;
            }
            if (KeyState.IsKeyUp(Keys.B)) { Bpressed = false; }


            if (KeyState.IsKeyDown(Keys.X))
            {
                rotationAmount = rotationAmount - timeStep / 100;
            }

            if (KeyState.IsKeyDown(Keys.Space))
            {
                if (!SpacePressed) { ExcNum = (ExcNum + 1) % (maxExc + 1); }
                SpacePressed = true;
            }
            if (KeyState.IsKeyUp(Keys.Space)) { SpacePressed = false; }

            // Update Excersize
            model.Meshes[0].MeshParts[0].Effect = effect[ExcNum];
            // Update the window title
            Window.Title = "XNA Renderer | FPS: " + frameRateCounter.FrameRate;

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            int num = 0;
           for (int i = 0; i < NR_MODELS; i++)
           {
               if (!(InViewFrustum[i])) { num++; }
           }
           string numstr = num.ToString();
            // set render target
            device.SetRenderTarget(renderTarget);
            device.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Clear the screen in a predetermined color and clear the depth buffer
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);
            Matrix World = Matrix.CreateScale(2.0f);
            Matrix Rotate = Matrix.CreateRotationY((float)Math.PI * rotationAmount);
            World = Rotate * World;

            // Get the model's only mesh
            ModelMesh mesh = model.Meshes[0];
            Effect effect = mesh.Effects[0];

            Vector3 Light = new Vector3(0, 0, 40);

            switch (ExcNum)
            {
                case 0:
                    // Set the effect parameters
                    effect.Parameters["AmbientColor"].SetValue(Color.Red.ToVector4());
                    effect.CurrentTechnique = effect.Techniques["Simple"];
                    // Matrices for 3D perspective projection
                    camera.SetEffectParameters(effect);


                    //modelMaterial.SetEffectParameters(effect);

                    effect.Parameters["LightSource"].SetValue(Light);
                    effect.Parameters["AmbientColor"].SetValue(Color.Red.ToVector4());
                    effect.Parameters["AmbientIntensity"].SetValue(0.3f);
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector4());
                    effect.Parameters["Eye"].SetValue(camEye);
                    effect.Parameters["World"].SetValue(World);
                    effect.Parameters["InvTransWorld"].SetValueTranspose(Matrix.Invert(World));
                    mesh.Draw();
                    break;
                case 1:
                    effect.CurrentTechnique = effect.Techniques["Spotlight"];
                    camera.SetEffectParameters(effect);
                    effect.Parameters["Phi"].SetValue(0.8f);
                    effect.Parameters["Theta"].SetValue(0.9f);
                    effect.Parameters["LightSource"].SetValue(Light);
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector4());
                    effect.Parameters["Eye"].SetValue(camEye);
                    effect.Parameters["World"].SetValue(World);
                    effect.Parameters["InvTransWorld"].SetValueTranspose(Matrix.Invert(World));
                    mesh.Draw();
                    break;

                case 2:
                    effect.CurrentTechnique = effect.Techniques["MultiLight"];
                    Vector4[] lightColors = new Vector4[3];
                    lightColors[0] = Color.Red.ToVector4();
                    lightColors[1] = Color.Blue.ToVector4();
                    lightColors[2] = Color.Green.ToVector4();
                    Vector3[] lightPositions = new Vector3[3];
                    lightPositions[0] = new Vector3(-20, 0, 40);
                    lightPositions[1] = new Vector3(10, -20, 80);
                    lightPositions[2] = new Vector3(10, 40, 80);
                    effect.Parameters["LightPositions"].SetValue(lightPositions);
                    effect.Parameters["LightColors"].SetValue(lightColors);
                    effect.Parameters["World"].SetValue(World);
                    effect.Parameters["InvTransWorld"].SetValueTranspose(Matrix.Invert(World));
                    effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
                    camera.SetEffectParameters(effect);
                    mesh.Draw();
                    break;
                case 3:

                    ModelMesh[] mesh2 = new ModelMesh[NR_MODELS];
                    //World2 = Rotate2 * World2 * Translate;
                    for (int i = 0; i < NR_MODELS; i++)
                    {
                        mesh2[i] = model2[i].Meshes[0];
                        effect = mesh2[i].Effects[0];
                    }
                    effect.Parameters["AmbientColor"].SetValue(Color.Red.ToVector4());
                    effect.CurrentTechnique = effect.Techniques["Simple"];
                    // Matrices for 3D perspective projection
                    camera.SetEffectParameters(effect);


                    //modelMaterial.SetEffectParameters(effect);

                    effect.Parameters["LightSource"].SetValue(Light);
                    effect.Parameters["AmbientColor"].SetValue(Color.Red.ToVector4());
                    effect.Parameters["AmbientIntensity"].SetValue(0.3f);
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector4());
                    effect.Parameters["Eye"].SetValue(camEye);
                    effect.Parameters["InvTransWorld"].SetValueTranspose(Matrix.Invert(World));


                    
                    for (int i = 0; i < NR_MODELS; i++)
                    {
                        if (InViewFrustum[i])
                        {
                            effect.Parameters["World"].SetValue(World2n[i]);
                            mesh2[i].Draw();
                        }
                        
                    }
                    //BoundingFrustum
                    effect.Parameters["World"].SetValue(World);
                    break;

                // do nothing for now
            }

            //Draw the model
            

            //Handle Post Effects
            if (postGray) { postEffect.Parameters["ApplyGray"].SetValue(true); }
            else { postEffect.Parameters["ApplyGray"].SetValue(false); }
            if (postBlur)
            {
                postEffect.Parameters["ApplyBlur"].SetValue(true);
            }
            else { postEffect.Parameters["ApplyBlur"].SetValue(false); }

            device.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
                    SamplerState.LinearClamp, DepthStencilState.Default,
                    RasterizerState.CullNone, postEffect);
            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, 800, 600), Color.White);
            if (ExcNum == 3)
                spriteBatch.DrawString(font, numstr, new Vector2(20, 45), Color.White);
            spriteBatch.End();

            // Draw the model
            //mesh.Draw();

            base.Draw(gameTime);
        }
        


        //public BoundingSphere BoundingSphere
        //{

        //    get { return new BoundingSphere(Position, model.Meshes[0].BoundingSphere.Radius); }

        //}
    }

}