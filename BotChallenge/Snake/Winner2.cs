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
    class Winner2 : EnvSnake.Bot
    {    

        protected override EnvSnake.Action GetAction()
        {
            if (!Alive)
            {
                return new EnvSnake.Action()
                {
                    movementDirection = SnakeDirection.Down,
                };
            }

            Tuple<Point, SnakeDirection>[] dir_points = get_possible_directions();
            float[] point_dists = new float[dir_points.Length];
            int min_dist_idx = 0;
            float global_min_food_dist = float.MaxValue;

            bool[] head_collision = new bool[dir_points.Length];
            List<int> no_collision_indicies = new List<int>();
            int min_idx_no_head_collision = -1;
            float global_min_food_dist_no_head_collision = float.MaxValue;

            for (int i = 0; i < dir_points.Length; i++)
            {
                float min_food_dist = float.MaxValue;
                for (int j = 0; j < Food.Count; j++)
                {
                    float dist = calc_dist(Food[j],dir_points[i].Item1);
                    if (dist < min_food_dist)
                        min_food_dist = dist;
                }
                point_dists[i] = min_food_dist;
                if (min_food_dist < global_min_food_dist)
                {
                    global_min_food_dist = min_food_dist;
                    min_dist_idx = i;
                }

                if (check_no_head_collision(dir_points[i].Item1))
                {
                    if (min_food_dist < global_min_food_dist_no_head_collision || min_idx_no_head_collision == -1)
                    {
                        min_idx_no_head_collision = i;
                        global_min_food_dist_no_head_collision = min_food_dist;
                    }
                    no_collision_indicies.Add(i);
                }
                else
                {
                    head_collision[i] = true;
                }
            }
            
            if (dir_points.Length == 0)
            { 
                // No dir_points error
                return new EnvSnake.Action()
                {
                    movementDirection = Direction
                };
            }
            else
            {
                SnakeDirection dir = dir_points[min_dist_idx].Item2;

                if (min_idx_no_head_collision >= 0)
                {
                    if (Food.Count == 0)
                        dir = dir_points[no_collision_indicies[Env.constRand.Next(no_collision_indicies.Count)]].Item2;
                    else
                        dir = dir_points[min_idx_no_head_collision].Item2;
                }


                // No dir_points error
                return new EnvSnake.Action()
                {
                    movementDirection = dir,
                };
            }

        }
        
        private float calc_dist(Point p0, Point p1)
        {
            return (p0.ToVector2() - p1.ToVector2()).Length();
        }

        public Tuple<Point,SnakeDirection>[] get_possible_directions()
        {
            List<Tuple<Point, SnakeDirection>> back = new List<Tuple<Point, SnakeDirection>>();
            for (int i = 0; i < 4; i++)
            {
                if (is_dir_ok((SnakeDirection)i))
                    back.Add(new Tuple<Point, SnakeDirection>(dir2coord((SnakeDirection)i),(SnakeDirection)i));
            }
            return back.ToArray();
        }

        public bool is_dir_ok(SnakeDirection dir)
        {
            Point n_point = dir2coord(dir);

            // Wall collision
            if (n_point.X < 0 || n_point.X >= GridWidth || n_point.Y < 0 || n_point.Y >= GridHeight)
                return false;

            // Snake Collision
            for (int i = 0; i < enemies.Length; i++)
                if (enemies[i].Alive)
                {
                    if (enemies[i].BodyParts.Any(b => b.X == n_point.X && b.Y == n_point.Y))
                        return false;
                }
            if (BodyParts.Take(BodyLength - 1).Any(b => b.X == n_point.X && b.Y == n_point.Y))
                return false;

            if (((int)dir + 2) % 4 == (int)Direction)
                return false;

            return true;
        }

        private Point dir2coord(SnakeDirection dir)
        {
            Point pos = new Point(HeadPosX,HeadPosY);
            if (dir == SnakeDirection.Right) pos.X += 1;
            else if (dir == SnakeDirection.Down) pos.Y += 1;
            else if (dir == SnakeDirection.Left) pos.X -= 1;
            else if (dir == SnakeDirection.Up) pos.Y -= 1;
            return pos;

        }
        
        private bool check_no_head_collision(Point p)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                // Right
                if (p.X == enemies[i].HeadPosX + 1 && p.Y == enemies[i].HeadPosY)
                    return false;
                // Down
                else if (p.X == enemies[i].HeadPosX && p.Y == enemies[i].HeadPosY + 1)
                    return false;
                // Left
                else if (p.X == enemies[i].HeadPosX-1 && p.Y == enemies[i].HeadPosY)
                    return false;
                // Up
                else if (p.X == enemies[i].HeadPosX && p.Y == enemies[i].HeadPosY - 1)
                    return false;
            }
            return true;
        }

        protected override Color GetColor()
        {
            return Color.Green;
        }
    }
}
