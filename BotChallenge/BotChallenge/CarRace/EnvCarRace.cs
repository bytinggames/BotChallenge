using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace BotChallenge.CarRace
{
    public class EnvCarRace : Env, MonoMethods
    {
        public int Width { get; } = 20;
        public int Height { get; } = 20;
        public const int TOTALFRAMES = 60 * 60 * 2; // 2min

        public const float FRICTION = 0.98f;

        public List<Vector2> goals;
        
        public struct Action
        {
            public float accelerate;
            public float steer;
            public float brake;
        }

        int frame = 0;

        public int Frame => frame;
        
        internal Bot[] bots;
        GameBase game;
        
        bool gameEnd;

        GraphicsDeviceManager graphics;

        Texture2D texBot, pixel;
        SpriteFont font;

        public static Matrix matrix;
        
        public EnvCarRace(Type[] botTypes)
        {
            Collision.minDist = 0.01f;
            CollisionResult.minDist = 0;

            this.bots = GetBots<Bot>(botTypes);
        }

        internal override float[] Loop()
        {
            if (Visible)
            {
                using (game = new GameBase(this))
                    game.Run();
            }
            else
            {
                while (!gameEnd)
                {
                    MyUpdate();
                }
            }

            float[] scores = new float[bots.Length];
            /*
            List<Bot> r = new List<Bot>();
            for (int i = 0; i < bots.Length; i++)
            {
                int j;
                for (j = 0; j < r.Count; j++)
                {
                    if (bots[i].timeAlive < r[j].timeAlive)
                        break;
                }
                r.Insert(j, bots[i]);
            }

            for (int i = 0; i < r.Count; i++)
            {
                scores[r[i].Id] = i;
            }*/

            if (bots[0].frameTime == bots[1].frameTime)
            {
                scores = new float[] { 1, 1 };
            }
            else
            {
                if (bots[0].frameTime > bots[1].frameTime)
                    scores = new float[] { 2, 0 };
                else
                    scores = new float[] { 0, 2 };
            }

            //for (int i = 0; i < scores.Length; i++)
            //{
            //    scores[i] = bots[i].damageDealt;
            //}
            return scores;// bots.Select(f => (float)f.wins).Cast<float>().ToArray();
        }
        
        void MonoMethods.LoadContent(SpriteBatch spriteBatch, ContentManager content, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;

            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            texBot = content.Load<Texture2D>("Shooter/Textures/bot");
            font = content.Load<SpriteFont>("Fonts/lato-thin-mod_10");
            
            for (int i = 0; i < this.bots.Length; i++)
            {
                List<Bot> botsList = this.bots.ToList();
                botsList.RemoveAt(i);

                int x, y;
                Vector2 pos;
                do
                {
                    x = constRand.Next(Width) - Width / 2;
                    y = constRand.Next(Height) - Height / 2;
                    pos = new Vector2(x + 0.5f, y + 0.5f);
                } while (this.bots.Any(f => f.pos == pos));

                this.bots[i].Initialize(this, pos, i);
            }
        }

        void MyUpdate()
        {
            if ((TOTALFRAMES > 0 && frame == TOTALFRAMES) || bots.Count(f => !f.Alive) >= bots.Length - 1) //either timeout or all bots dead but one
            {
                gameEnd = true;
            }

            if (gameEnd)
            {
                if (Visible && (Input.enter.down || Input.space.pressed))
                    game.Exit();
                return;
            }

            Action[] actions = new Action[bots.Length];

            for (int j = 0; j < bots.Length; j++)
            {
                if (bots[j].Alive)
                    actions[j] = bots[j].GetInternalAction();
            }

            ExecuteActions(actions);

            for (int i = 0; i < bots.Length; i++)
            {
                if (bots[i].Alive)
                    bots[i].Update();
            }


            frame++;
        }

        void MonoMethods.Update()
        {
            MyUpdate();
        }

        private void ExecuteActions(Action[] actions)
        {
            for (int i = 0; i < bots.Length; i++)
            {
                bots[i].action = actions[i];
            }
        }

        void MonoMethods.Draw(SpriteBatch spriteBatch)
        {

            float scale = Math.Min((float)graphics.PreferredBackBufferWidth / Width, (float)graphics.PreferredBackBufferHeight / Height);

            float realW = graphics.PreferredBackBufferWidth / scale;
            float realH = graphics.PreferredBackBufferHeight / scale;

            scale *= 0.25f;

            Vector2 shift = new Vector2(realW, realH) / 2f;// - new Vector2(width, height) / 2f;

            matrix =  Matrix.CreateTranslation(new Vector3(shift, 0)) * Matrix.CreateScale(scale);
            //DrawM.basicEffect.SetWorldAndInvTransp(matrix);
            DrawM.basicEffect.World = matrix;
            DrawM.scale = scale;

            //spriteBatch.GraphicsDevice.Clear(new Color(128,128,128));
            spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp,null,null,null,matrix);

            //DrawM.Vertex.DrawRectangle(new M_Rectangle(0, 0, width, height), Color.CornflowerBlue);
            //DrawM.Vertex.DrawCircle(Vector2.Zero, MAPRADIUS, Color.CornflowerBlue, 16f);
            

            string text;

            for (int i = 0; i < bots.Length; i++)
            {
                if (bots[i].Alive)
                {
                    bots[i].mask.Draw(bots[i].GetInternalColor());

                    bots[i].Draw();
                }
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            text = "";

            text += "time: " + Math.Floor(frame / 60f) + " s\n";

            if (gameEnd)
            {
                for (int i = 0; i < bots.Length; i++)
                {
                    text += "______________________\n";
                    text += "Bot " + i + "\n";
                    //text += "Health: " + bots[i].health + "\n";
                    text += "Time: " + bots[i].frameTime + "\n";
                    /*text += "Kills: " + bots[i].kills + "\n";
                    text += "Damage Dealt: " + bots[i].damageDealt + "\n";*/
                }
            }

            spriteBatch.DrawString(font, text, new Vector2(16, 16), Color.White);

            spriteBatch.End();
        }
    }
}
