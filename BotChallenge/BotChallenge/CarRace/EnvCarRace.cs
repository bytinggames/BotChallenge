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
        public struct Action
        {
            /// <summary>
            /// [0,1]
            /// 0: no acceleration
            /// 0 ... 1: in between
            /// 1: full acceleration
            /// </summary>
            public float accelerate;
            /// <summary>
            /// [-1,1]
            /// -1: left steer
            /// -1 ... 1: in between
            /// 1: right steer
            /// </summary>
            public float steer;
            /// <summary>
            /// [0,1]
            /// 0: no brake
            /// 0 ... 1: in between
            /// 1: full brake
            /// </summary>
            public float brake;
        }

        /// <summary>
        /// width of the map in meters
        /// </summary>
        public int Width { get; } = 160;
        /// <summary>
        /// height of the mapin meters
        /// </summary>
        public int Height { get; } = 90;
        /// <summary>
        /// maximum frame count it takes for one race (60fps)
        /// </summary>
        public readonly int totalFrames = 60 * 60 * 2; // 2min

        /// <summary>
        /// how many goals you need to reach
        /// </summary>
        public readonly int goalCount = 5;
        /// <summary>
        /// in meters
        /// </summary>
        public readonly float goalRadius = 3f;

        internal List<Vector2> goals;

        int frame = 0;

        /// <summary>
        /// gets current frame
        /// </summary>
        public int Frame => frame;
        
        internal Bot[] bots;
        GameBase game;
        
        bool gameEnd;

        GraphicsDeviceManager graphics;
        
        SpriteFont font;

        Matrix matrix;

        bool firstPlayerView;

        public EnvCarRace(Type[] botTypes)
        {
            Collision.minDist = 0.01f;
            CollisionResult.minDist = 0;

            goals = new List<Vector2>();

            Vector2 pos;

            int space = 4;
            for (int i = 0; i < goalCount; i++)
            {
                pos = Vector2.Zero;
                do
                {
                    pos = new Vector2(constRand.Next(space, Width - space), constRand.Next(space, Height - space)) + new Vector2(0.5f);
                } while (goals.Any(f => Vector2.Distance(pos, f) < 3f));

                goals.Add(pos);
            }

            this.bots = GetBots<Bot>(botTypes);

            firstPlayerView = bots.Length == 1;

            int x, y;
            x = constRand.Next(Width);
            y = constRand.Next(Height);
            pos = new Vector2(x + 0.5f, y + 0.5f);

            float orientation = constRand.NextFloat() * MathHelper.TwoPi;

            for (int i = 0; i < this.bots.Length; i++)
            {
                this.bots[i].Initialize(this, pos, orientation, i, goals.ToList());
            }
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

            if (bots.Length > 1)
            {
                for (int i = 0; i < bots.Length; i++)
                {
                    scores[i] = bots.Count(f => f.frameTime >= bots[i].frameTime) - 1;
                }
            }
            else
            {
                scores[0] = bots[0].frameTime;
            }
            
            return scores;
        }
        
        void MonoMethods.LoadContent(SpriteBatch spriteBatch, ContentManager content, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            
            font = content.Load<SpriteFont>("Fonts/lato-thin-mod_10");
        }

        void MyUpdate()
        {
            if ((totalFrames > 0 && frame == totalFrames) || bots.All(f => f.goalIndex == goalCount)) //either timeout or all bots dead but one
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

            for (int i = 0; i < bots.Length; i++)
            {
                if (bots[i].Alive && bots[i].control)
                    actions[i] = bots[i].GetInternalAction();
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

        float cameraZoom = 1f;
        float cameraOrientation = 99f;

        void MonoMethods.Draw(SpriteBatch spriteBatch)
        {
            float wScale = (float)graphics.PreferredBackBufferWidth / Width;
            float hScale = (float)graphics.PreferredBackBufferHeight / Height;
            float scale = Math.Min(wScale, hScale);

            if (Input.mbWheel != 0)
            {
                cameraZoom -= Math.Sign(Input.mbWheel);
                if (cameraZoom < 1)
                    cameraZoom = 1f;
            }

            scale *= cameraZoom;

            float realW = graphics.PreferredBackBufferWidth / scale;
            float realH = graphics.PreferredBackBufferHeight / scale;


            if (firstPlayerView)
            {
                if (cameraOrientation == 99)
                    cameraOrientation = -bots[0].orientation - MathHelper.PiOver2;
                else
                    cameraOrientation += (-bots[0].orientation - MathHelper.PiOver2 - cameraOrientation) * 0.1f;// 04f;

                matrix = Matrix.CreateTranslation(new Vector3(-bots[0].positionV, 0f)) * Matrix.CreateRotationZ(cameraOrientation) * Matrix.CreateTranslation(new Vector3(new Vector2(realW, realH) / 2f, 0)) * Matrix.CreateScale(scale);
            }
            else
            {
                Vector2 shift = Vector2.Zero;// new Vector2(realW, realH) / 2f;// - new Vector2(width, height) / 2f;

                if (hScale > wScale)
                {
                    shift.Y += (realH - Height) * 0.5f;
                }
                else if (wScale > hScale)
                {
                    shift.X += (realW - Width) * 0.5f;
                }

                matrix = Matrix.CreateTranslation(new Vector3(shift, 0)) * Matrix.CreateScale(scale);
            }
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

                DrawGoal(i, goals[i], 1f);
            }

            void DrawGoal(int i, Vector2 pos, float opaque)
            {
                DrawM.Vertex.DrawCircleOutline(pos, goalRadius, Color.DeepSkyBlue, 8f);

                float angle = 0f;
                float fov = MathHelper.TwoPi / bots.Length;
                for (int j = 0; j < bots.Length; j++)
                {
                    Color color = bots[j].goalIndex > i ? Color.Transparent : bots[j].goalIndex == i ? bots[j].GetInternalColor() : Color.DeepSkyBlue;
                    DrawM.Vertex.DrawCone(pos, goalRadius, angle, fov, color * opaque, color * opaque, 8f);
                    angle += fov;
                }
                //DrawM.Vertex.DrawCircle(goals[i], GOALRADIUS, Color.Lime, 16f);
                //font.Draw(i.ToString(), Anchor.Center(goals[i]), Color.Black, new Vector2(0.2f));
            }

            if (firstPlayerView && bots[0].goalIndex < goals.Count)
            {
                // if goal is outside of screen, show it at the border of the screen


                M_Rectangle rect = new M_Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                Vector2 center = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight) / 2f;
                M_Polygon poly = rect.ToPolygon();
                //rect.pos -= rect.size / 2f;
                Matrix invert = Matrix.Invert(matrix);
                poly.Transform(invert);
                center = Vector2.Transform(center, invert);
                Vector2 onScreenGoalPos = goals[bots[0].goalIndex];// Vector2.Transform(goals[bots[0].goalIndex], matrix);

                //DrawM.Vertex.DrawRectangle(rect, Color.Red * 0.5f);

                if (!poly.ColVector(onScreenGoalPos))
                {
                    Vector2 dir = onScreenGoalPos - center;
                    var cr = poly.DistToVector(onScreenGoalPos, dir);

                    onScreenGoalPos -= dir * cr.distance.Value;// + Vector2.Normalize(dir) * 3f;

                    //if (onScreenGoalPos.X < rect.Left)
                    //    onScreenGoalPos.X = rect.Left;
                    //if (onScreenGoalPos.X > rect.Right)
                    //    onScreenGoalPos.X = rect.Right;
                    //if (onScreenGoalPos.Y < rect.Top)
                    //    onScreenGoalPos.Y = rect.Top;
                    //if (onScreenGoalPos.Y > rect.Bottom)
                    //    onScreenGoalPos.Y = rect.Bottom;

                    DrawGoal(bots[0].goalIndex, onScreenGoalPos, 0.5f);
                }

                //camera
                //bots[0].goalIndex
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
