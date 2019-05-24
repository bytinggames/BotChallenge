using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using JuliHelper;

namespace BotChallenge
{
    class GameBase : Game
    {
        static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MonoMethods env;

        #region window

        public static GameWindow window;
        static bool changeResolution = false;
        public static bool isFullScreen;

        private static int windowResX, windowResY;

        public static int resX
        {
            get
            { return graphics.PreferredBackBufferWidth; }
        }
        public static int resY
        {
            get
            { return graphics.PreferredBackBufferHeight; }
        }

        #endregion


        public GameBase(MonoMethods env)
        {
            this.env = env;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth *= 2;
            graphics.PreferredBackBufferHeight *= 2;
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            window = Window;

            windowResX = resX;
            windowResY = resY;
        }

        protected override void Initialize()
        {

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new System.EventHandler<System.EventArgs>(Window_ClientSizeChanged);

            CenterWindow();

            DrawM.Initialize(GraphicsDevice);
            Input.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            env.LoadContent(spriteBatch, Content, graphics);
        }

        protected override void UnloadContent()
        {
        }

        bool pause = false;

        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            if (Input.f11.pressed)
                ToggleFullscreen();

            if (Input.esc.pressed)
                Exit();

            if (Input.space.pressed)
                pause = !pause;
            if ((!Input.rightShift.down && !pause) || Input.anyControl.pressed || Input.rightShift.down && Input.rightShift.TimeDown % (Input.anyControl.down?16:4)  == 0)
            {
                int iterations = 1;
                if (Input.leftShift.down)
                {
                    iterations *= 10;
                    if (Input.leftControl.down)
                        iterations *= 10;
                }

                for (int i = 0; i < iterations; i++)
                {
                    env.Update();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            env.Draw(spriteBatch);

            base.Draw(gameTime);
        }


        public void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (!changeResolution)
            {
                changeResolution = true;

                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.ApplyChanges();

                if (!isFullScreen)
                {
                    windowResX = resX;
                    windowResY = resY;
                }

                changeResolution = false;
            }

            DrawM.Initialize(graphics.GraphicsDevice);
        }
        public static void ToggleFullscreen()
        {
            isFullScreen = !isFullScreen;

            changeResolution = true;

            if (isFullScreen)
            {
                window.IsBorderless = true;
                window.Position = new Point(0, 0);
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;//System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;//System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                graphics.ApplyChanges();
            }
            else
            {
                window.IsBorderless = false;
                graphics.PreferredBackBufferWidth = windowResX;
                graphics.PreferredBackBufferHeight = windowResY;
                graphics.ApplyChanges();

                CenterWindow();
            }

            changeResolution = false;

            DrawM.ResizeWindow();
        }
        public static void CenterWindow()
        {
            if (!isFullScreen)
            {
                window.Position = new Point((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - graphics.PreferredBackBufferWidth) / 2, (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 80 - graphics.PreferredBackBufferHeight) / 2);
            }
        }
    }
}
