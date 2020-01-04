using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace BotChallenge.Bumper
{
    public class EnvBumper : Env, MonoMethods
    {
        int width = 20;
        int height = 20;
        bool loadFromTexture = false;
        public bool LoadFromTexture => loadFromTexture;
        public const int TOTALFRAMES = 60 * 60 * 2; // 2min


        public abstract class Bot
        {
            public const float ACCELERATION = 0.005f;
            public const float MAXVELOCITY = 10f;
            public const float SPEED = 0.1f;
            public const float RADIUS = 0.4f;
            public const bool COLLISIONSPEEDWASTE = true;
            public const float CHARGESPEED = 1f / 60f;
            public const float MAXSHOOTSPEED = 0.5f;

            internal float aim;
            private Vector2 _pos;
            internal Vector2 pos
            {
                get { return _pos; }
                set
                {
                    _pos = value;
                    mask.pos = _pos;
                }
            }
            internal float health;
            internal int ammo;
            public EnvBumper env;
            internal M_Circle mask;
            private int id;
            internal float charge;
            internal float damageDealt;
            internal int kills;

            //visible for the bot programmer
            public int Id { get { return id; } }
            public bool Alive { get { return health > 0; } }

            internal void Initialize(EnvBumper env, Bot[] enemies, bool[,] map, Vector2 pos, int id)
            {
                mask = new M_Circle(pos, RADIUS);

                this.env = env;
                this.enemies = enemies;
                this.map = map;
                this.pos = pos;
                this.id = id;

                health = 1;
                ammo = 3;
                charge = 0;

                rand = Env.constRand;
            }

            //visible for the bot programmer
            protected Bot[] enemies;
            protected bool[,] map;
            public float Charge { get { return charge; } }
            public Vector2 Pos { get { return _pos; } }
            public int Ammo { get { return ammo; } }
            public float Health => health;
            protected Random rand;
            internal Vector2 velocity;

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
                        env.EachWall(rect, (x, y) =>
                        {
                            CollisionResult cr2 = mask.DistToPolygon(new M_Rectangle(x,y,1,1).ToPolygon(), move);
                            cr.AddCollisionResult(cr2, move);
                        });

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

            internal void Update()
            {
                Vector2 lastPos = pos;
                Move(velocity);

                Vector2 realMove = pos - lastPos;
                velocity = realMove;
            }
        }

        public struct Action
        {
            public bool accelerate;
            public float angle;
        }

        int frame = 0;

        public int Frame => frame;

        bool[,] map;
        internal Bot[] bots;
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        List<Bullet> bullets;
        GameBase game;

        public Bullet[] Bullets;


        bool gameEnd;

        GraphicsDeviceManager graphics;

        Texture2D texBot, pixel;
        SpriteFont font;

        public static Matrix matrix;
        
        public EnvBumper(Type[] botTypes)
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

            List<Bot> r = new List<Bot>();
            for (int i = 0; i < bots.Length; i++)
            {
                int j;
                for (j = 0; j < r.Count; j++)
                {
                    if (bots[i].damageDealt < r[j].damageDealt)
                        break;
                }
                r.Insert(j, bots[i]);
            }

            for (int i = 0; i < r.Count; i++)
            {
                scores[r[i].Id] = i;
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

            if (loadFromTexture)
            {
                Texture2D texMap = content.Load<Texture2D>("Shooter/Textures/map");


                Color[] colors = new Color[texMap.Width * texMap.Height];
                texMap.GetData<Color>(colors);

                width = texMap.Width;
                height = texMap.Height;

                map = new bool[width, height];
                int i = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        map[x, y] = colors[i++] == Color.Black;
                    }
                }
            }
            else
            {
                map = new bool[width, height];

                //for (int x = 0; x < width; x++)
                //{
                //    map[x, 0] = true;
                //    map[x, height - 1] = true;
                //}
                //for (int y = 1; y < height - 1; y++)
                //{
                //    map[0, y] = true;
                //    map[width - 1, y] = true;
                //}
            }

            Vector2 center = new Vector2(Math.Min(width, height) / 2f);
            for (int i = 0; i < this.bots.Length; i++)
            {
                List<Bot> botsList = this.bots.ToList();
                botsList.RemoveAt(i);

                int x, y;
                Vector2 pos;
                do
                {
                    x = constRand.Next(Width);
                    y = constRand.Next(Height);
                    pos = new Vector2(x + 0.5f, y + 0.5f);
                } while (map[x, y] || this.bots.Any(f => f.pos == pos) || Vector2.Distance(pos, center) > center.X * 0.75f);
                
                this.bots[i].Initialize(this, botsList.ToArray(), map, pos, i);
            }

            bullets = new List<Bullet>();
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
            }
            else
            {

                Bullets = bullets.ToArray();

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

                for (int i = 0; i < bots.Length; i++)
                {
                    if (bots[i].Alive)
                    {
                        Vector2 center = new Vector2(Math.Min(width, height) / 2f);
                        if (Vector2.Distance(bots[i].pos, center) > center.X)
                            bots[i].health = 0f;
                    }

                    if (bots[i].Alive)
                    {
                        for (int j = i + 1; j < bots.Length; j++)
                        {
                            if (bots[j].Alive)
                            {
                                if (bots[i].mask.ColMask(bots[j].mask))// && Vector2.Dot(bots[i].velocity, bots[j].velocity) >= 0)
                                {
                                    Vector2 dist = bots[j].pos - bots[i].pos;
                                    Vector2 distN = Vector2.Normalize(dist);
                                    Vector2 v = Vector2.Zero;

                                    if (bots[i].velocity != Vector2.Zero)
                                        v += Vector2.Dot(bots[i].velocity, distN) * distN;
                                    if (bots[j].velocity != Vector2.Zero)
                                        v += Vector2.Dot(-bots[j].velocity, distN) * distN;

                                    v += Vector2.Normalize(distN) * 0.02f;

                                    bots[i].velocity -= v;
                                    bots[j].velocity += v;
                                }
                            }
                        }
                    }
                }

                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    if (bullets[i].Update())
                        bullets.RemoveAt(i);
                }

                frame++;
            }
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
                    if (actions[i].accelerate)
                    {
                        Vector2 move = new Vector2((float)Math.Cos(actions[i].angle), (float)Math.Sin(actions[i].angle));

                        bots[i].velocity += move * Bot.ACCELERATION;
                        if (bots[i].velocity.Length() > Bot.MAXVELOCITY)
                            bots[i].velocity = Vector2.Normalize(bots[i].velocity) * Bot.MAXVELOCITY;
                    }
                    else
                    {
                        bots[i].velocity *= 0.99f;
                    }
                }
            }

        }

        void MonoMethods.Draw(SpriteBatch spriteBatch)
        {
            float scale = Math.Min((float)graphics.PreferredBackBufferWidth / width, (float)graphics.PreferredBackBufferHeight / height);

            float realW = graphics.PreferredBackBufferWidth / scale;
            float realH = graphics.PreferredBackBufferHeight / scale;

            Vector2 shift = new Vector2(realW, realH) / 2f - new Vector2(width, height) / 2f;

            matrix =  Matrix.CreateTranslation(new Vector3(shift, 0)) * Matrix.CreateScale(scale);
            //DrawM.basicEffect.SetWorldAndInvTransp(matrix);
            DrawM.basicEffect.World = matrix;
            DrawM.scale = scale;

            spriteBatch.GraphicsDevice.Clear(new Color(128,128,128));

            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp,null,null,null,matrix);

            //DrawM.Vertex.DrawRectangle(new M_Rectangle(0, 0, width, height), Color.CornflowerBlue);
            DrawM.Vertex.DrawCircle(new Vector2(width, height) / 2f, Math.Min(width, height) / 2f, Color.CornflowerBlue, 16f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[x, y])
                        DrawM.Vertex.DrawRectangle(new M_Rectangle(x, y, 1, 1), Color.Black);
                        //spriteBatch.Draw(pixel, new Vector2(x, y), Color.Black);
                }
            }

            for (int i = 0; i < bullets.Count; i++)
                bullets[i].Draw(spriteBatch);

            string text;

            for (int i = 0; i < bots.Length; i++)
            {
                if (bots[i].Alive)
                {
                    DrawM.Vertex.DrawCircle(bots[i].pos, Bot.RADIUS, Color.White, 8f);

                    #region Health

                    List<Vector2> vertices = new List<Vector2>();
                    float targetAngle = bots[i].health * MathHelper.TwoPi - MathHelper.PiOver2;
                    Vector2 p;
                    for (float angle = -MathHelper.PiOver2; angle < targetAngle; angle += 0.1f)
                    {
                        p = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                        vertices.Add(Vector2.Zero);
                        vertices.Add(p * Bot.RADIUS);
                    }
                    p = new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle));
                    vertices.Add(Vector2.Zero);
                    vertices.Add(p * Bot.RADIUS);
                    DrawM.Vertex.DrawTriangleStrip(bots[i].pos, vertices, bots[i].GetInternalColor());

                    #endregion

                    #region Charge

                    vertices = new List<Vector2>();
                    targetAngle = bots[i].charge * MathHelper.TwoPi - MathHelper.PiOver2;
                    float borderRadius = 0.05f;

                    for (float angle = -MathHelper.PiOver2; angle < targetAngle; angle += 0.1f)
                    {
                        p = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                        vertices.Add(p * Bot.RADIUS * (1 - borderRadius));
                        vertices.Add(p * Bot.RADIUS * (1 + borderRadius));
                    }
                    p = new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle));
                    vertices.Add(p * Bot.RADIUS * (1 - borderRadius));
                    vertices.Add(p * Bot.RADIUS * (1 + borderRadius));
                    DrawM.Vertex.DrawTriangleStrip(bots[i].pos, vertices, Color.Black);

                    #endregion


                    //M_Polygon chargePoly = M_Polygon.GetCircleClosed(bots[i].pos, Bot.RADIUS, 16);
                    //DrawM.Vertex.DrawPolygon(chargePoly.pos, chargePoly.vertices, bots[i].Color);
                    //DrawM.Sprite.DrawCircle(spriteBatch, bots[i].pos, 0.5f, Color.White, 0, 16);
                    //spriteBatch.Draw(texBot, bots[i].pos, Color.White);

                    text = bots[i].ammo.ToString();
                    Vector2 textSize = font.MeasureString(text);
                    float textScale = Bot.RADIUS * 2f / textSize.Length();
                    textSize *= textScale;
                    spriteBatch.DrawString(font, text, bots[i].pos - textSize / 2f, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);
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
                    text += "Kills: " + bots[i].kills + "\n";
                    text += "Damage Dealt: " + bots[i].damageDealt + "\n";
                }
            }

            spriteBatch.DrawString(font, text, new Vector2(16, 16), Color.White);

            spriteBatch.End();
        }

        internal void EachWall(M_Rectangle onRect, Action<int, int> action)
        {
            int x1 = Math.Max((int)onRect.Left, 0);
            int y1 = Math.Max((int)onRect.Top, 0);
            int x2 = Math.Min((int)onRect.Right + 1, width);
            int y2 = Math.Min((int)onRect.Bottom + 1, height);

            for (int y = y1; y < y2; y++)
                for (int x = x1; x < x2; x++)
                    if (map[x, y])
                        action(x, y);
        }
    }
}
