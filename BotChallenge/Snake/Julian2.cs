using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.Snake;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Snake
{
    class Julian2 : EnvSnake.Bot
    {

        protected override EnvSnake.Action GetAction()
        {
            SnakeDirection dir = Direction;

            if (!Alive)
                return new EnvSnake.Action() { movementDirection = SnakeDirection.Right };

            int x = this.HeadPosX;
            int y = this.HeadPosY;

            List<SnakeDirection> dirs = GetPossibleDirections(Direction);
            List<SnakeDirection> freeDirs = new List<SnakeDirection>();

            float bestWall = 1;
            int bestIndex = 0;
            for (int i = 0; i < dirs.Count; i++)
            {
                int xdir, ydir;
                DirectionToXY(dirs[i], out xdir, out ydir);

                float wall = IsWall(x + xdir, y + ydir);
                if (wall == 0)
                    freeDirs.Add(dirs[i]);
                if (wall < bestWall)
                {
                    bestWall = wall;
                    bestIndex = i;
                }
            }

            if (freeDirs.Count <= 1)
                dir = dirs[bestIndex];
            else
            {
                //get direction to next food
                //add freedom

                float bestScore = 999999999;

                bestIndex = 0;
                for (int i = 0; i < freeDirs.Count; i++)
                {
                    //calculate distance to nearest food
                    int xdir, ydir;
                    DirectionToXY(freeDirs[i], out xdir, out ydir);

                    Vector2 newPos = new Vector2(x + xdir, y + ydir);
                    float bestDist = 99999999;

                    List<Point> track = new List<Point>();
                    track.AddRange(Food);
                    //track.AddRange(enemies.Select(f => f.Alive ? f.BodyParts[0] : new Point(9999, 9999)));

                    for (int j = 0; j < track.Count; j++)
                    {
                        float border = 4;
                        float borderWeight = 3f;
                        float newDist = Math.Abs(track[j].X - newPos.X) + Math.Abs(track[j].Y - newPos.Y);
                        if (track[j].X < border)
                            newDist += (border - track[j].X) * borderWeight;
                        if (track[j].Y < border)
                            newDist += (border - track[j].Y) * borderWeight;
                        if (track[j].X > GridWidth - border - 1)
                            newDist += (track[j].X - (GridWidth - border - 1)) * borderWeight;
                        if (track[j].Y > GridHeight - border - 1)
                            newDist += (track[j].Y - (GridHeight - border - 1)) * borderWeight;

                        if (newDist < bestDist)
                        {
                            bestDist = newDist;
                        }
                    }


                    //calculate freedom
                    int max = 100;
                    float count = FloodFillCount(new Point(x + xdir, y + ydir), max);
                    count *= count;
                    count *= 1f / (max * max);
                    if (count >= 1)
                        count = 1;
                    else
                    {

                    }

                    float score = bestDist - count * 10;//2114

                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestIndex = i;
                    }
                }

                dir = freeDirs[bestIndex];
            }

            return new EnvSnake.Action()
            {
                movementDirection = dir,
            };
        }

        protected override Color GetColor()
        {
            return Color.Orange;
        }

        float IsWall(int x, int y)
        {
            if (x < 0 || y < 0 || x >= GridWidth || y >= GridHeight)
                return 1;

            float nowall = 1;

            List<EnvSnake.Bot> bots = new List<EnvSnake.Bot>();
            bots.Add(this);
            bots.AddRange(enemies.ToList());

            for (int i = 0; i < bots.Count; i++)
            {
                List<Point> points = new List<Point>();
                if (i > 0)
                    points.AddRange(GetNextPossiblePoints(new Point(bots[i].HeadPosX, bots[i].HeadPosY), bots[i].Direction));
                points.AddRange(bots[i].BodyParts);
                for (int j = 0; j < points.Count; j++)
                {
                    if (points[j].X == x && points[j].Y == y)
                    {
                        if (j < 3)
                            nowall *= 2f / 3f;
                        else if (j == 3)
                            nowall *= 0.9f;
                        else
                            return 1;
                    }
                }
            }

            if (BodyLength > enemies.Max(f => f.BodyLength) + 1)
            {
                for (int j = 0; j < Food.Count; j++)
                {
                    if (Food[j].X == x && Food[j].Y == y)
                    {
                           nowall *= 0.9f;
                    }
                }
            }

            return 1 - nowall;
        }

        void DirectionToXY(SnakeDirection dir, out int xDir, out int yDir)
        {
            xDir = dir == SnakeDirection.Right ? 1 : dir == SnakeDirection.Left ? -1 : 0;
            yDir = dir == SnakeDirection.Down ? 1 : dir == SnakeDirection.Up ? -1 : 0;
        }

        List<SnakeDirection> GetPossibleDirections(SnakeDirection dir)
        {
            List<SnakeDirection> dirs = new List<SnakeDirection>();
            if (Env.constRand.Next(2) == 0)
            {
                dirs.Add((SnakeDirection)(((int)dir + 1) % 4));
                dirs.Add((SnakeDirection)(((int)dir + 3) % 4));
            }
            else
            {
                dirs.Add((SnakeDirection)(((int)dir + 3) % 4));
                dirs.Add((SnakeDirection)(((int)dir + 1) % 4));
            }
            dirs.Add(dir);

            return dirs;
        }

        List<Point> GetNextPossiblePoints(Point p, SnakeDirection d)
        {
            List<SnakeDirection> dirs = GetPossibleDirections(d);
            List<Point> points = new List<Point>();
            for (int i = 0; i < dirs.Count; i++)
            {
                int xdir, ydir;
                DirectionToXY(dirs[i], out xdir, out ydir);
                points.Add(new Point(p.X + xdir, p.Y + ydir));
            }
            return points;
        }

        int FloodFillCount(Point p, int max)
        {
            int count = 0;
            List<Point> open = new List<Point>() { p };
            List<Point> closed = new List<Point>();

            while (open.Count > 0)
            {
                List<Point> n = GetNeighbours(open[0]);
                for (int i = n.Count - 1; i >= 0; i--)
                {
                    if (IsWall(n[i].X, n[i].Y) > 0 || open.Contains(n[i]) || closed.Contains(n[i]))
                        n.RemoveAt(i);
                }

                count += n.Count * n.Count;
                if (count >= max)
                    return max;

                open.AddRange(n);

                closed.Add(open[0]);
                open.RemoveAt(0);
            }

            return count;
        }

        List<Point> GetNeighbours(Point p)
        {
            return new List<Point>()
            {
                new Point(p.X+1, p.Y),
                new Point(p.X, p.Y+1),
                new Point(p.X-1, p.Y),
                new Point(p.X, p.Y-1),
            };
        }
    }
}
