using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BotChallenge.Snake
{
    public enum SnakeDirection
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3
    }

    public class EnvSnake : Env, MonoMethods
    {
        public abstract class Bot
        {
            internal int gridWidth = 0;
            internal int gridHeight = 0;
            internal int posX = 0;
            internal int posY = 0;
            internal SnakeDirection direction;
            internal int bodyLength = 0;
            internal bool alive = true;
            internal List<Point> bodyParts;
            internal List<Point> food;
            
            internal bool initialized = false;
            internal float score;

            private Random rand;

            public int GridWidth { get { return gridWidth; } }
            public int GridHeight { get { return gridHeight; } }
            public int HeadPosX { get { return posX; } }
            public int HeadPosY { get { return posY; } }
            public SnakeDirection Direction { get { return direction; } }
            public int BodyLength { get { return bodyParts == null ? 0 : bodyParts.Count; } }
            public bool Alive { get { return alive; } }
            public List<Point> BodyParts { get { return bodyParts; } }
            public List<Point> Food { get { return food; } }



            internal void Initialize(Bot[] enemies, int gridWidth, int gridHeight, int posX, int posY,Random rand)
            {
                this.rand = rand;
                this.enemies = enemies;

                this.gridWidth = gridWidth;
                this.gridHeight = gridHeight;
                this.posX = posX;
                this.posY = posY;


                this.direction = (SnakeDirection)rand.Next(4);
                this.alive = true;
                this.bodyParts = new List<Point>();
                this.bodyParts.Add(new Point(posX, posY));
                this.initialized = true;
                this.score = 0;
            }

            protected Bot[] enemies;

            internal Action GetInternalAction()
            {
                return GetAction();
            }
            protected abstract Action GetAction();

            internal void UpdateFood(List<Point> food)
            {
                this.food = food;
            }

            internal void Move(SnakeDirection dir)
            {
                if (Alive == false)
                    return;

                // Adapt direction
                if (Direction == SnakeDirection.Right || Direction == SnakeDirection.Left)
                {
                    if (dir == SnakeDirection.Up) direction = SnakeDirection.Up;
                    else if (dir == SnakeDirection.Down) direction = SnakeDirection.Down;
                }
                else if (Direction == SnakeDirection.Down || Direction == SnakeDirection.Up)
                {
                    if (dir == SnakeDirection.Right) direction = SnakeDirection.Right;
                    else if (dir == SnakeDirection.Left) direction = SnakeDirection.Left;
                }

                if (Direction == SnakeDirection.Right|| Direction == SnakeDirection.Left)
                    posX += Direction == SnakeDirection.Right ? 1 : -1;
                if (Direction == SnakeDirection.Up || Direction == SnakeDirection.Down)
                    posY += Direction == SnakeDirection.Down ? 1 : -1;

                this.bodyParts.Add(new Point(posX, posY));

            }

            internal bool CheckAlive()
            {
                if (Alive == false)
                    return this.Alive;

                if (HeadPosX < 0 || HeadPosX >= GridWidth ||HeadPosY < 0 || HeadPosY >= GridHeight)
                {
                    this.alive = false;
                    return this.alive;
                }

                // Check enemy collision
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i].bodyParts.Any(b => b.X == HeadPosX && b.Y == HeadPosY))
                    {
                        this.alive = false;
                        break;
                    }
                }

                // Check own colission
                if (BodyParts.Take(BodyLength - 1).Any(b => b.X == HeadPosX && b.Y == HeadPosY))
                    this.alive = false;


                return this.Alive;
            }


            internal string GetName() { return GetType().Name; }
            internal Color GetInternalColor() { return GetColor(); }
            
            protected abstract Color GetColor();
        }

        public struct Action
        {
            public SnakeDirection movementDirection;
        }

        public static Matrix matrix;

        Bot[] bots;
        List<Point> food;
        int gridWidth = 20;//28 / 2;
        int gridHeight = 20;//28 / 2;
        int delayMs = 500;

        public const int TOTALFRAMES = 0;//2 * 60 * 3 * 30; // 3min
        int frame = 0;
        int framesTillFood = 20;
        bool gameEnd = false;

        Stopwatch sw = new Stopwatch();
        Random rand = constRand;
        GraphicsDeviceManager graphics;
        GameBase game;

        SpriteFont font;
        Texture2D pixel;
        Texture2D texSnakeBody;
        Texture2D[] texSnakeHeads; // 0 = right; 1 = down; 2 = left; 3 = right;
        Texture2D texBackground;
        Texture2D texFood;

        public EnvSnake(Type[] botTypes)
        {
            bots = GetBots<Bot>(botTypes);

            for (int i = 0; i < bots.Length; i++)
            {
                List<Bot> botsList = bots.ToList();
                botsList.RemoveAt(i);

                Point nBotPos = new Point(-1,-1);
                while (botsList.Any(b => (b.initialized && b.HeadPosX == nBotPos.X && b.HeadPosY == nBotPos.Y) || nBotPos.X < 0 || nBotPos.Y < 0))
                    nBotPos = new Point(rand.Next(gridWidth),rand.Next(gridHeight));

                bots[i].Initialize(botsList.ToArray(), gridWidth, gridHeight,nBotPos.X,nBotPos.Y,rand);
            }
            
            food = new List<Point>();
            SpawnFood();
            sw.Start();
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
            for (int i = 0; i < scores.Length; i++)
            {
                scores[i] = bots[i].score;
            }
            return scores;
        }
        
        void MonoMethods.LoadContent(SpriteBatch spriteBatch, ContentManager Content, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;

            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            font = Content.Load<SpriteFont>("Fonts/lato-thin-mod_10");

            texSnakeBody = Content.Load<Texture2D>("Snake/Textures/snake_body_part");
            texBackground = Content.Load<Texture2D>("Snake/Textures/background");
            texFood = Content.Load<Texture2D>("Snake/Textures/food");

            texSnakeHeads = new Texture2D[4];
            texSnakeHeads[0] = Content.Load<Texture2D>("Snake/Textures/snake_head_right");
            texSnakeHeads[1] = Content.Load<Texture2D>("Snake/Textures/snake_head_down");
            texSnakeHeads[2] = Content.Load<Texture2D>("Snake/Textures/snake_head_left");
            texSnakeHeads[3] = Content.Load<Texture2D>("Snake/Textures/snake_head_up");
        }

        void MonoMethods.Update()
        {
            MyUpdate();
        }

        public static SnakeDirection lastPressedDirection, lastPressedDirection2;

        bool start = false;

        void MyUpdate()
        {
            if (!start)
            {
                if (Input.backSpace.pressed)
                    start = true;
                return;
            }

            if (Input.a.pressed)
                lastPressedDirection = SnakeDirection.Left;
            else if (Input.d.pressed)
                lastPressedDirection = SnakeDirection.Right;
            else if (Input.s.pressed)
                lastPressedDirection = SnakeDirection.Down;
            else if (Input.w.pressed)
                lastPressedDirection = SnakeDirection.Up;

            if (Input.left.pressed)
                lastPressedDirection2 = SnakeDirection.Left;
            else if (Input.right.pressed)
                lastPressedDirection2 = SnakeDirection.Right;
            else if (Input.down.pressed)
                lastPressedDirection2 = SnakeDirection.Down;
            else if (Input.up.pressed)
                lastPressedDirection2 = SnakeDirection.Up;

            float delay = delayMs / 4f;//Input.leftShift.down ? delayMs / 4f : delayMs;

            if (sw.ElapsedMilliseconds < delay && !Input.space.down && Visible)
                return;
            sw.Restart();

            if ((TOTALFRAMES > 0 && frame == TOTALFRAMES) || bots.Count(f => !f.Alive) >= bots.Length - 1) //either timeout or all bots dead but one
            {
                gameEnd = true;
            }

            if (gameEnd == true && Visible)
            {
                game.Exit();
            }
            else
            {
                Action[] actions = new Action[bots.Length];

                for (int j = 0; j < bots.Length; j++)
                {
                    actions[j] = bots[j].GetInternalAction();

                    actions[j].movementDirection = (SnakeDirection)((int)actions[j].movementDirection % 4);
                }

                ExecuteActions(actions);

                if (frame % framesTillFood == 0 && frame != 0)
                    SpawnFood();
                frame++;
            }
        }

        private void ExecuteActions(Action[] actions)
        {
            for (int i = 0; i < bots.Length; i++)
                bots[i].UpdateFood(this.food.ToList());

            for (int i = 0; i < bots.Length; i++)
            {
                if (bots[i].Alive)
                    bots[i].Move(actions[i].movementDirection);
            }

            // Check if food gets eaten
            bool[] botAteFood = new bool[bots.Length];
            for (int i = food.Count-1; i >= 0; i--)
            {
                bool eaten = false;
                for (int j = 0; j < bots.Length; j++)
                {
                    if (bots[j].Alive)
                    {
                        if (bots[j].HeadPosX == food[i].X && bots[j].HeadPosY == food[i].Y)
                        {
                            eaten = true;
                            botAteFood[j] = true;
                        }
                    }
                }
                if (eaten)
                    food.RemoveAt(i);
            }

            for (int i = 0; i < botAteFood.Length; i++)
                if (bots[i].Alive && !botAteFood[i])
                    bots[i].bodyParts.RemoveAt(0);
                else if (bots[i].Alive && botAteFood[i])
                    bots[i].score += framesTillFood;

            // Update snake alive status
            for (int i = 0; i < bots.Length; i++)
                bots[i].CheckAlive();

            // Remvoe snakes if dead
            for (int i = 0; i < bots.Length; i++)
                if (bots[i].alive == false && bots[i].BodyLength > 0)
                    bots[i].bodyParts.Clear();
                else if (bots[i].Alive)
                    bots[i].score++;            
                    
        }

        void MonoMethods.Draw(SpriteBatch spriteBatch)
        {
            float scale = Math.Min((float)graphics.PreferredBackBufferWidth / gridWidth, (float)graphics.PreferredBackBufferHeight / gridHeight);
            float texScale = scale / 12;

            float realW = graphics.PreferredBackBufferWidth / scale;
            float realH = graphics.PreferredBackBufferHeight / scale;

            Vector2 shift = new Vector2(realW, realH) / 2f - new Vector2(gridWidth, gridHeight) / 2f;

            matrix = Matrix.CreateTranslation(new Vector3(shift, 0)) * Matrix.CreateScale(scale);
            //DrawM.basicEffect.SetWorldAndInvTransp(matrix);
            DrawM.basicEffect.World = matrix;
            DrawM.scale = scale;

            spriteBatch.GraphicsDevice.Clear(new Color(128, 128, 128));

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, matrix);

            DrawM.Vertex.DrawRectangle(new M_Rectangle(0, 0, gridWidth, gridHeight), new Color(64,64,64));


            float border = 0.025f;
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    DrawM.Vertex.DrawRectangle(new M_Rectangle(x + border, y + border, 1 - border * 2f, 1 - border * 2f), Color.Black);
                    //spriteBatch.Draw(pixel, new Vector2(x, y), Color.Black);
                }
            }

            for (int i = 0; i < food.Count; i++)
            {
                spriteBatch.Draw(texFood, new Rectangle(food[i].X,food[i].Y, 1, 1), Color.White);
            }

            for (int i = 0; i < bots.Length; i++)
            {
                if (bots[i].Alive)
                {
                    for (int j = 0; j < bots[i].bodyParts.Count-1; j++)
                        spriteBatch.Draw(texSnakeBody, new Rectangle(bots[i].bodyParts[j].X, bots[i].bodyParts[j].Y, 1, 1), bots[i].GetInternalColor());
                    spriteBatch.Draw(texSnakeHeads[(int)bots[i].Direction], new Rectangle(bots[i].HeadPosX, bots[i].HeadPosY, 1, 1), bots[i].GetInternalColor());
                }
            }


            spriteBatch.End();

            spriteBatch.Begin();

            for (int i = 0; i < bots.Length; i++)
            {
                string text = bots[i].GetName();
                if (text.Length > 32) text = text.Substring(0, 32);
                text += " - " + bots[i].score;

                spriteBatch.DrawString(font, text, new Vector2(16, 16 + (22 * i)), bots[i].GetInternalColor(), 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }


        private void SpawnFood()
        {
            Point nFoodPos;
            bool collision = false;
            do
            {
                nFoodPos = new Point(rand.Next(gridWidth), rand.Next(gridHeight));
                collision = false;
                for (int i = 0; i < bots.Length; i++)
                    if (bots[i].bodyParts.Any(b => b.X == nFoodPos.X && b.Y == nFoodPos.Y))
                    {
                        collision = true;
                        break;
                    }
            } while (collision);

            this.food.Add(nFoodPos);
            for (int i = 0; i < bots.Length; i++)
                bots[i].UpdateFood(this.food.ToList()); // ???
        }
    }
}
