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
        public int Width { get; } = 160;
        public int Height { get; } = 90;
        public const int TOTALFRAMES = 60 * 60 * 2; // 2min

        public const int GOALCOUNT = 5;
        public const float GOALRADIUS = 3f;

        public const float FRICTION = 0.98f;

        public List<Vector2> goals;
        
        public struct Action
        {
            /// <summary>
            /// [0,1]
            /// </summary>
            public float accelerate;
            /// <summary>
            /// [-1,1]
            /// </summary>
            public float steer;
            /// <summary>
            /// [0,1]
            /// </summary>
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

            goals = new List<Vector2>();

            int space = 4;
            for (int i = 0; i < GOALCOUNT; i++)
            {
                Vector2 pos = Vector2.Zero;
                do
                {
                    pos = new Vector2(constRand.Next(space, Width - space), constRand.Next(space, Height - space)) + new Vector2(0.5f);
                } while (goals.Any(f => Vector2.Distance(pos, f) < 3f));

                goals.Add(pos);
            }
        }

        public override float[] Loop()
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

            int x, y;
            Vector2 pos;
            x = constRand.Next(Width);
            y = constRand.Next(Height);
            pos = new Vector2(x + 0.5f, y + 0.5f);

            float orientation = constRand.NextFloat() * MathHelper.TwoPi;

            for (int i = 0; i < this.bots.Length; i++)
            {
                this.bots[i].Initialize(this, pos, orientation, i);
            }
        }

        void MyUpdate()
        {
            if ((TOTALFRAMES > 0 && frame == TOTALFRAMES))// || bots.Count(f => !f.Alive) >= bots.Length - 1) //either timeout or all bots dead but one
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
            float wScale = (float)graphics.PreferredBackBufferWidth / Width;
            float hScale = (float)graphics.PreferredBackBufferHeight / Height;
            float scale = Math.Min(wScale, hScale);

            float realW = graphics.PreferredBackBufferWidth / scale;
            float realH = graphics.PreferredBackBufferHeight / scale;

            //scale *= 0.25f;

            Vector2 shift = Vector2.Zero;// new Vector2(realW, realH) / 2f;// - new Vector2(width, height) / 2f;

            if (hScale > wScale)
            {
                shift.Y += (realH - Height) * 0.5f;
            }
            else if (wScale > hScale)
            {
                shift.X += (realW - Width) * 0.5f;
            }


            matrix =  Matrix.CreateTranslation(new Vector3(shift, 0)) * Matrix.CreateScale(scale);
            //DrawM.basicEffect.SetWorldAndInvTransp(matrix);
            DrawM.basicEffect.World = matrix;
            DrawM.scale = scale;

            spriteBatch.GraphicsDevice.Clear(new Color(128,128,128));
            //spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp,null,null,null,matrix);

            Drawer.depth = new DepthLayer(0f);

            DrawM.Vertex.DrawRectangle(new M_Rectangle(0, 0, Width, Height), Color.CornflowerBlue);


            for (int i = 0; i < goals.Count; i++)
            {
                if (i + 1 < goals.Count)
                {

                    DrawM.Vertex.DrawLine(goals[i], goals[i + 1], Color.DeepSkyBlue, 0.2f);
                }

                float angle = 0f;
                float fov = MathHelper.TwoPi / bots.Length;
                for (int j = 0; j < bots.Length; j++)
                {
                    Color color = bots[j].goalIndex > i ? Color.Lime : bots[j].goalIndex == i ? Color.White : Color.DeepSkyBlue;
                    DrawM.Vertex.DrawCone(goals[i], GOALRADIUS, angle, fov, color, color, 8f);
                    angle += fov;
                }
                //DrawM.Vertex.DrawCircle(goals[i], GOALRADIUS, Color.Lime, 16f);
                //font.Draw(i.ToString(), Anchor.Center(goals[i]), Color.Black, new Vector2(0.2f));
            }


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
            
            for (int i = 0; i < goals.Count; i++)
            {
                Vector2 pos = Vector2.Transform(goals[i], matrix);
                font.Draw((i + 1).ToString(), Anchor.Center(pos), Color.Black);
            }

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
