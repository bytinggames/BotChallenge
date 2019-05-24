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
    class Dominik2_1 : EnvSnake.Bot
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
                    int diff_x = enemies[i].BodyParts[j].X - HeadPosX;
                    int diff_y = enemies[i].BodyParts[j].Y - HeadPosY;
                    if (Math.Sqrt(Math.Pow(Math.Abs(diff_x), 2) + Math.Pow(Math.Abs(diff_y), 2)) <= 2)
                    {
                        if (diff_x > 0) // want to go left
                        {
                            if (diff_y > 0) // want to go up
                            {
                                if (choice.Contains(3))
                                {
                                    dir = SnakeDirection.Up;
                                }
                                else dir = SnakeDirection.Left;
                            }
                            else // want to go down
                            {
                                if (choice.Contains(1))
                                {
                                    dir = SnakeDirection.Down;
                                }
                                else dir = SnakeDirection.Left;
                            }
                        }
                        else // want to go right
                        {
                            if (diff_y > 0) // want to go up
                            {
                                if (choice.Contains(3))
                                {
                                    dir = SnakeDirection.Up;
                                }
                                else dir = SnakeDirection.Right;
                            }
                            else // want to go down
                            {
                                if (choice.Contains(1))
                                {
                                    dir = SnakeDirection.Down;
                                }
                                else dir = SnakeDirection.Right;
                            }
                        }
                    }
                }
            }

            // crash myself?
            if (BodyLength > 4)
            {
                for (int i = 4; i < BodyLength; i++)
                {
                    int diff_x = BodyParts[i].X - HeadPosX;
                    int diff_y = BodyParts[i].Y - HeadPosY;
                    if (Math.Sqrt(Math.Pow(Math.Abs(diff_x), 2) + Math.Pow(Math.Abs(diff_y), 2)) <= 2)
                    {
                        if (diff_x > 0) // want to go left
                        {
                            if (diff_y > 0) // want to go up
                            {
                                if (choice.Contains(3))
                                {
                                    dir = SnakeDirection.Up;
                                }
                                else dir = SnakeDirection.Left;
                            }
                            else // want to go down
                            {
                                if (choice.Contains(1))
                                {
                                    dir = SnakeDirection.Down;
                                }
                                else dir = SnakeDirection.Left;
                            }
                        }
                        else // want to go right
                        {
                            if (diff_y > 0) // want to go up
                            {
                                if (choice.Contains(3))
                                {
                                    dir = SnakeDirection.Up;
                                }
                                else dir = SnakeDirection.Right;
                            }
                            else // want to go down
                            {
                                if (choice.Contains(1))
                                {
                                    dir = SnakeDirection.Down;
                                }
                                else dir = SnakeDirection.Right;
                            }
                        }
                    }
                }
            }


                // crash wall?
                switch (dir)
            {
                case SnakeDirection.Right:
                    if (HeadPosX >= w - 3)
                    {
                        if (HeadPosY <= h - HeadPosY)
                        {
                            if (choice.Contains(1)) { dir = SnakeDirection.Down; }
                            else dir = SnakeDirection.Left;
                        }
                        else {
                            if (choice.Contains(3)) { dir = SnakeDirection.Up; }
                            else dir = SnakeDirection.Left;
                        }
                    }
                    break;
                case SnakeDirection.Down:
                    if (HeadPosY >= h - 3)
                    {
                        if (HeadPosX <= w - HeadPosX) {
                            if (choice.Contains(0)) { dir = SnakeDirection.Right; }
                            else dir = SnakeDirection.Up;
                        }
                        else {
                            if (choice.Contains(2)) { dir = SnakeDirection.Left; }
                            else dir = SnakeDirection.Up;
                        }
                    }
                    break;
                case SnakeDirection.Left:
                    if (HeadPosX <= 2) {
                        if (HeadPosY <= h - HeadPosY) {
                            if (choice.Contains(1)) dir = SnakeDirection.Down;
                            else dir = SnakeDirection.Right;
                        }
                        else {
                            if (choice.Contains(3)) dir = SnakeDirection.Up;
                            else dir = SnakeDirection.Right;
                        }
                    }
                    break;
                case SnakeDirection.Up:
                    if (HeadPosY <= 2)
                    {
                        if (HeadPosX <= w - HeadPosX)
                        {
                            if (choice.Contains(0)) { dir = SnakeDirection.Right; }
                            else dir = SnakeDirection.Down;
                        }
                        else
                        {
                            if (choice.Contains(2)) { dir = SnakeDirection.Left; }
                            else dir = SnakeDirection.Down;
                        }
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
