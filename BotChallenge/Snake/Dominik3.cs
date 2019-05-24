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
    class Dominik3 : EnvSnake.Bot
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

            //crash bot or self?
            int[] risk = new int[3];
            for (int i = 0; i < 3; i++)
            {
                SnakeDirection dir_temp = (SnakeDirection) choice[i];
                int place_temp_x = HeadPosX;
                int place_temp_y = HeadPosY;
                switch (dir_temp)
                {
                    case SnakeDirection.Right:
                        place_temp_x += 1;
                        break;
                    case SnakeDirection.Down:
                        place_temp_y += 1;
                        break;
                    case SnakeDirection.Left:
                        place_temp_x -= 1;
                        break;
                    case SnakeDirection.Up:
                        place_temp_y -= 1;
                        break;
                }
                if (place_temp_x < 0 || place_temp_y < 0 || place_temp_x >= GridWidth || place_temp_y >= GridHeight)
                    risk[i] += 4;

                for (int k = 0; k < enemies.Length; k++)
                {
                    if (enemies[k].Alive)
                    {
                        for (int j = 0; j < enemies[k].BodyLength - 1; j++)
                        {
                            if ((enemies[k].BodyParts[j].X == place_temp_x) & (enemies[k].BodyParts[j].Y == place_temp_y))
                            {
                                risk[i]+=2;
                            }
                        }
                        if (Math.Abs(enemies[k].BodyParts[enemies[k].BodyLength - 1].X - place_temp_x) + Math.Abs(enemies[k].BodyParts[enemies[k].BodyLength - 1].Y - place_temp_y) <= 2)
                        {
                            if (Math.Abs(enemies[k].BodyParts[enemies[k].BodyLength - 1].X - place_temp_x) + Math.Abs(enemies[k].BodyParts[enemies[k].BodyLength - 1].Y - place_temp_y) <= 1)
                                risk[i] += 2;
                            else risk[i] += 1;
                        }
                    }
                }
                if (BodyLength > 4)
                {
                    for (int j= 3; j < BodyLength-1; j++)
                    {
                        {
                            if ((BodyParts[j].X == place_temp_x) & (BodyParts[j].Y == place_temp_y))
                            {
                                risk[i] += 2;
                            }
                        }
                    }
                }
            }

            if (risk.Max() == 0)
                {
                    dir = (SnakeDirection) r;
                }
            else{
                for (int i = 0; i < 3; i++)
                {
                if (risk[i] == risk.Min())
                    {
                        dir = (SnakeDirection) choice[i];
                    }
                }
            }
            

            // crash wall?
            //switch (dir)
            //{
            //    case SnakeDirection.Right:
            //        if (HeadPosX >= w - 3)
            //        {
            //            if (HeadPosY <= h - HeadPosY)
            //            {
            //                if (choice.Contains(1)) { dir = SnakeDirection.Down; }
            //                else dir = SnakeDirection.Left;
            //            }
            //            else
            //            {
            //                if (choice.Contains(3)) { dir = SnakeDirection.Up; }
            //                else dir = SnakeDirection.Left;
            //            }
            //        }
            //        break;
            //    case SnakeDirection.Down:
            //        if (HeadPosY >= h - 3)
            //        {
            //            if (HeadPosX <= w - HeadPosX)
            //            {
            //                if (choice.Contains(0)) { dir = SnakeDirection.Right; }
            //                else dir = SnakeDirection.Up;
            //            }
            //            else
            //            {
            //                if (choice.Contains(2)) { dir = SnakeDirection.Left; }
            //                else dir = SnakeDirection.Up;
            //            }
            //        }
            //        break;
            //    case SnakeDirection.Left:
            //        if (HeadPosX <= 2)
            //        {
            //            if (HeadPosY <= h - HeadPosY)
            //            {
            //                if (choice.Contains(1)) dir = SnakeDirection.Down;
            //                else dir = SnakeDirection.Right;
            //            }
            //            else
            //            {
            //                if (choice.Contains(3)) dir = SnakeDirection.Up;
            //                else dir = SnakeDirection.Right;
            //            }
            //        }
            //        break;
            //    case SnakeDirection.Up:
            //        if (HeadPosY <= 2)
            //        {
            //            if (HeadPosX <= w - HeadPosX)
            //            {
            //                if (choice.Contains(0)) { dir = SnakeDirection.Right; }
            //                else dir = SnakeDirection.Down;
            //            }
            //            else
            //            {
            //                if (choice.Contains(2)) { dir = SnakeDirection.Left; }
            //                else dir = SnakeDirection.Down;
            //            }
            //        }
            //        break;
            //}


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
