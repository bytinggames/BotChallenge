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


        public abstract class Bot
        {
            public const float RADIUS = 0.4f;

            public const float ACCELERATION = 0.005f;
            internal const float MAXVELOCITY = 10f;

            internal const bool COLLISIONSPEEDWASTE = true;
            internal const float SPEED = 0.1f;

            const float MAXTURNPERFRAME = 0.04f;

            private Vector2 _pos;
            public Vector2 Pos => _pos;
            internal Vector2 pos
            {
                get { return _pos; }
                set
                {
                    _pos = value;
                    mask.pos = Pos;
                }
            }

            private float _orientation;
            internal float orientation
            {
                get { return _orientation; }
                set
                {
                    _orientation = value;
                    mask.vertices = maskSourceVertices.ToList();
                    mask.RotateRadians(_orientation);
                }
            }
            public float Orientation => orientation;

            public float steer, accelerate;

            internal float health;
            public EnvCarRace env;
            internal List<Vector2> maskSourceVertices;
            internal M_Polygon mask;
            private int id;

            //visible for the bot programmer
            public int Id { get { return id; } }
            public bool Alive { get { return health > 0; } }
            
            //visible for the bot programmer
            protected Bot[] enemies;

            //public float Charge { get { return charge; } }
            public float Health => health;
            protected Random rand;
            internal Vector2 velocity;

            public Vector2 Velocity => velocity;

            internal int frameTime; // time it took for this car to finish the race

            const float length = 4.7f;
            const float width = 1.9f;
            const float mass = 1302; // kg

            internal void Initialize(EnvCarRace env, Bot[] enemies, Vector2 pos, int id)
            {
                float w = length / 2f;
                float h = width / 2f;
                maskSourceVertices = new List<Vector2>() { new Vector2(w, h), new Vector2(-w, h), new Vector2(-w, -h), new Vector2(w, -h) };
                mask = new M_Polygon(pos, maskSourceVertices.ToList());

                this.env = env;
                this.enemies = enemies;
                this.pos = pos;
                this.id = id;

                health = 1;

                rand = Env.constRand;

                if (id == 0)
                {
                    this.pos = Vector2.Zero;
                    velocity = new Vector2(27.777f, 0);
                }
            }

            internal Action GetInternalAction()
            {
                return GetAction();
            }
            protected abstract Action GetAction();

            internal void Move(Vector2 velocity)
            {
                if (velocity != Vector2.Zero)
                {

                    #region Move loop

                    Vector2 move = velocity;
                    float newMoveDir = 0;//in which direction the last collision was directed
                    
                    float t = 0;
                    while (true)
                    {
                        if (move.Length() < 0.001f)
                            break;
                        
                        M_Rectangle rect = new M_Rectangle(pos, Vector2.Zero);
                        rect.Enlarge(RADIUS + 1);
                        rect.Expand(move);

                        CollisionResult cr = new CollisionResult();
                        //env.EachWall(rect, (x, y) =>
                        //{
                        //    CollisionResult cr2 = mask.DistToPolygon(new M_Rectangle(x,y,1,1).ToPolygon(), move);
                        //    cr.AddCollisionResult(cr2, move);
                        //});

                        if (!cr.distance.HasValue || t + cr.distance.Value >= 1)
                        {
                            if (t > 1)
                                t = 1;
                            pos += move * (1 - t);
                            break;
                        }

                        #region step move

                        //move to nearest dist
                        pos += move * cr.distance.Value;
                        t += cr.distance.Value;

                        Vector2 lastMoveN = new Vector2(-move.Y, move.X);

                        Vector2 tangent = new Vector2(-cr.axisCol.Y, cr.axisCol.X);

                        if (COLLISIONSPEEDWASTE)
                        {
                            move = Vector2.Dot(velocity, tangent) * tangent;
                        }
                        else
                        {
                            float moveLength = move.Length();
                            move = Vector2.Dot(velocity, tangent) * tangent;
                            if (move != Vector2.Zero)
                                move = Vector2.Normalize(move) * moveLength;
                        }
                        if (newMoveDir == 0)
                            newMoveDir = Vector2.Dot(lastMoveN, move);
                        else if (Math.Sign(newMoveDir) != Math.Sign(Vector2.Dot(lastMoveN, move)))
                        {
                            //sackgasse
                            move = Vector2.Zero;
                        }

                        if (move.Length() < SPEED * 0.1f)
                            move = Vector2.Zero;

                        #endregion
                    }

                    #endregion
                }
            }

            internal string GetName() { return GetType().Name; }
            internal Color GetInternalColor() { return GetColor(); }
            
            protected abstract Color GetColor();

            float speed;

            float gravitationalAcceleration = 9.8f; //m/s^2


            internal void Update()
            {
                //pos = Vector2.Zero;
                //velocity = new Vector2(1, 0);
                //steer = -MathHelper.PiOver4;

                //speed += accelerate * ACCELERATION;

                //if (steer != 0)
                //{
                //    float steeringRadiusBack = (float)Math.Tan(MathHelper.PiOver2 - Math.Abs(steer)) * length;
                //    float steeringRadiusFront = length / (float)Math.Cos(MathHelper.PiOver2 - Math.Abs(steer));

                //    Vector2 steeringRotationOrigin = mask.pos + (mask.vertices[1] + mask.vertices[2]) / 2f
                //        + Calculate.AngleToVector(orientation + MathHelper.PiOver2 * Math.Sign(steer)) * steeringRadiusBack;


                //    if (speed != 0)
                //    {
                //        float steeringRadiusCenter = (steeringRotationOrigin - pos).Length();
                //        float steeringAngleOffsetFromCenterToBack = (float)Math.Atan((length / 2f) / steeringRadiusBack);


                //        float angleSpeed = MathHelper.TwoPi / ((MathHelper.TwoPi * steeringRadiusCenter) / speed);
                //        if (angleSpeed > MAXTURNPERFRAME)
                //        {

                //        }

                //        orientation += angleSpeed * Math.Sign(steer);

                //        Vector2 newBackPos = steeringRotationOrigin + Calculate.AngleToVector(orientation - MathHelper.PiOver2 * Math.Sign(steer)) * steeringRadiusBack;
                //        Vector2 newPos = newBackPos + Calculate.AngleToVector(orientation) * (length / 2f);

                //        //Vector2 newFrontPos = steeringRotationOrigin + Calculate.AngleToVector(orientation + steer - MathHelper.PiOver2 * Math.Sign(steer)) * steeringRadiusFront;
                //        //Vector2 newPos = newFrontPos - Calculate.AngleToVector(orientation) * (length / 2f);
                //        velocity = newPos - pos;
                //    }

                //}
                //else
                //{
                //    velocity = Calculate.AngleToVector(orientation) * speed;
                //}

                //speed *= FRICTION;

                //if (accelerate)
                //{
                //    Vector2 move = new Vector2((float)Math.Cos(bots[i].Orientation), (float)Math.Sin(bots[i].Orientation));

                //    bots[i].velocity += move * Bot.ACCELERATION;
                //    if (bots[i].velocity.Length() > Bot.MAXVELOCITY)
                //        bots[i].velocity = Vector2.Normalize(bots[i].velocity) * Bot.MAXVELOCITY;
                //}
                //else
                //{
                //    bots[i].velocity *= FRICTION;
                //}


                float gravityForce = mass * gravitationalAcceleration; //kg * m/s^2

                float frictionValue = 0.6f;

                float frictionForce = gravityForce * frictionValue;
                float frictionalDeceleration = frictionForce / mass * (1f / 60f);
                if (frictionalDeceleration < velocity.Length())
                    velocity -= Vector2.Normalize(velocity) * frictionalDeceleration; // m / s
                else
                    velocity = Vector2.Zero;
                //velocity *= FRICTION;

                Vector2 lastPos = pos;
                Move(velocity * (1f / 60f));

                //Vector2 realMove = pos - lastPos;
                //velocity = realMove;
            }

            public virtual void Draw()
            {
                if (steer != 0)
                {
                    float steeringRadius = (float)Math.Tan(MathHelper.PiOver2 - Math.Abs(steer)) * length;

                    Vector2 steeringRotationOrigin = mask.pos + (mask.vertices[1] + mask.vertices[2]) / 2f
                        + Calculate.AngleToVector(orientation + MathHelper.PiOver2 * Math.Sign(steer)) * steeringRadius;

                    DrawM.Vertex.DrawCircleOutline(steeringRotationOrigin, steeringRadius, Color.Black, 8f);
                }
            }
        }

        public struct Action
        {
            public float accelerate;
            public int steer;
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

                this.bots[i].Initialize(this, botsList.ToArray(), pos, i);
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
                if (bots[i].Alive)
                {
                    bots[i].steer = actions[i].steer * MathHelper.PiOver4; // 45°
                    bots[i].accelerate = actions[i].accelerate;
                }
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
                    text += "Health: " + bots[i].health + "\n";
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
