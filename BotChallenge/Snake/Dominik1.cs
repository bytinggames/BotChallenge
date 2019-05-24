using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.Snake;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BotChallenge;

namespace Snake
{
    class Dominik1 : EnvSnake.Bot
    {

        protected override EnvSnake.Action GetAction()
        {
            if (!Alive) return new EnvSnake.Action() { movementDirection = SnakeDirection.Right };
            int h = GridHeight;
            int w = GridWidth;
            int[] choice = null;
            switch (Direction)
            {
                case SnakeDirection.Right:
                    choice = new int[] { 0, 1, 3 };
                    break;
                case SnakeDirection.Down:
                    choice = new int[] { 0, 1, 2 };
                    break;
                case SnakeDirection.Left:
                    choice = new int[] { 1, 2, 3 };
                    break;
                case SnakeDirection.Up:
                    choice = new int[] { 0, 2, 3 };
                    break;
            }
            int r = Env.constRand.Next(3);
            SnakeDirection dir = (SnakeDirection)choice[r];

            //crash bot?
            for (int i = 0; i < enemies.Length; i++)
            {
                for (int j = 0; j < enemies[i].BodyLength; j++)
                {
                    if (Math.Pow(enemies[i].BodyParts[j].X - HeadPosX, 2) + Math.Pow(enemies[i].BodyParts[j].Y - HeadPosY, 2) == 1)
                    {
                        if (Direction == SnakeDirection.Down || Direction == SnakeDirection.Up)
                        {
                            if (HeadPosX <= w - HeadPosX)
                            {
                                dir = SnakeDirection.Right;
                            }
                            else dir = SnakeDirection.Left;
                        }
                        else
                        {
                            if (HeadPosY <= h - HeadPosY)
                            {
                                dir = SnakeDirection.Down;
                            }
                            else dir = SnakeDirection.Up;
                        }
                    }
                }
            }

            // crash wall?
            switch (dir) // achtung Direction!!!
            {
                case SnakeDirection.Right:
                    if (HeadPosX == w - 1)
                    {
                        if (HeadPosY <= h - HeadPosY)
                        {
                            dir = SnakeDirection.Down;
                        }
                        else dir = SnakeDirection.Up;
                    }
                    break;
                case SnakeDirection.Down:
                    if (HeadPosY == h - 1)
                    {
                        if (HeadPosX <= w - HeadPosX)
                        {
                            dir = SnakeDirection.Right;
                        }
                        else dir = SnakeDirection.Left;
                    }
                    break;
                case SnakeDirection.Left:
                    if (HeadPosX == 0)
                    {
                        if (HeadPosY <= h - HeadPosY)
                        {
                            dir = SnakeDirection.Down;
                        }
                        else dir = SnakeDirection.Up;
                    }
                    break;
                case SnakeDirection.Up:
                    if (HeadPosY == 0)
                    {
                        if (HeadPosX <= w - HeadPosX)
                        {
                            dir = SnakeDirection.Right;
                        }
                        else dir = SnakeDirection.Left;
                    }
                    break;
            }


            return new EnvSnake.Action()
            {
                movementDirection = dir,
            };
        }

        protected override Color GetColor()
        {
            return Color.DarkMagenta;
        }
    }
}
