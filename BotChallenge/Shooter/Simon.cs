using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.Shooter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    class Simon : EnvShooter.Bot
    {
        protected override Color GetColor()
        {
            return Color.Red;
        }

        public Simon()
        {
            //Start
        }
        int lastDir = 4;
        int steps = 0;
        float LastX = 0;
        float LastY = 0;

        protected override EnvShooter.Action GetAction()
        {
            bool l = false;
            bool r = false;
            bool u = false;
            bool d = false;

            if (steps == 0)
            {
                lastDir = 2;
            }
            steps++;

           

            bool collected = false;
            double rot = 0;
            foreach(EnvShooter.Bot e in this.enemies)
            {
                if (LastX == 0 || LastY == 0) {
                    rot = Math.Atan2(e.Pos.Y - this.Pos.Y, e.Pos.X - this.Pos.X);
                 
                }
                else
                {
                    if(LastX < e.Pos.X)
                    {
                        rot = Math.Atan2(e.Pos.Y - this.Pos.Y, e.Pos.X + 1 - this.Pos.X);
                    } else if(LastX > e.Pos.X)
                    {
                        rot = Math.Atan2(e.Pos.Y - this.Pos.Y, e.Pos.X - 1 - this.Pos.X);

                    }
                    else if (LastY > e.Pos.Y)
                    {
                        rot = Math.Atan2(e.Pos.Y -1 - this.Pos.Y, e.Pos.X - this.Pos.X);

                    }
                    else if (LastY < e.Pos.Y)
                    {
                        rot = Math.Atan2(e.Pos.Y+1 - this.Pos.Y, e.Pos.X - 1 - this.Pos.X);

                    } else if(LastX == e.Pos.X && LastY == e.Pos.Y)
                    {
                        rot = Math.Atan2(e.Pos.Y - this.Pos.Y, e.Pos.X - this.Pos.X);
                    }
                }
                LastX = e.Pos.X;
                LastY = e.Pos.Y;

            }

            
            bool c = true;
            //double dist = Math.Sqrt(Math.Pow((this.enemies[0].Pos.X - this.Pos.X), 2) + Math.Pow((this.enemies[0].Pos.Y - this.Pos.Y), 2));
            
            if(this.Charge == 1)
            {
                c = rand.Next(200/this.Ammo) > 0;
            }
            if(rand.Next(100) == 0)
            {
                int k = rand.Next(4);
                if(k == 0)
                {
                    lastDir = 0;
                } else if(k == 1)
                {
                    lastDir = 1;
                }
                else if (k == 2)
                {
                    lastDir = 2;
                }
                else if (k == 3)
                {
                    lastDir = 3;
                }
            }
            if(this.Pos.X > env.Width - 2)
            {
                lastDir = 1;
            } else if(this.Pos.X < 2)
            {
                lastDir = 0;
            }
            else if (this.Pos.Y > env.Height - 2)
            {
                lastDir = 2;
            }
            else if (this.Pos.Y < 2)
            {
                lastDir = 3;
            }

            if (lastDir == 0)
            {
                r = true;
            } else if(lastDir == 1)
            {
                l = true;
            } else if (lastDir == 2)
            {
                u = true;
            } else if (lastDir == 3)
            {
                d = true;
            }

            foreach (Bullet b in env.Bullets)
            {
                double distToB = Math.Sqrt(Math.Pow((b.Pos.X - this.Pos.X), 2) + Math.Pow((b.Pos.Y - this.Pos.Y), 2));
                if (distToB < 3.5 && !b.Collectible)
                {
                    c = false;
                    if (b.Id != this.Id)
                    {

                        if (((b.Velocity.Y > 0 || b.Velocity.Y < 0) && b.Velocity.X < 0) && Math.Abs(b.Velocity.Y) > Math.Abs(b.Velocity.X))
                        {
                            lastDir = 0;
                            r = true;
                        }
                        else if (((b.Velocity.Y > 0 || b.Velocity.Y < 0) && b.Velocity.X > 0) && Math.Abs(b.Velocity.Y) > Math.Abs(b.Velocity.X))
                        {
                            lastDir = 1;
                            l = true;
                        }
                        else if (((b.Velocity.X > 0 || b.Velocity.X < 0) && b.Velocity.Y > 0) && Math.Abs(b.Velocity.Y) < Math.Abs(b.Velocity.X))
                        {
                            lastDir = 2;
                            u = true;
                        }
                        else if (((b.Velocity.X > 0 || b.Velocity.X < 0) && b.Velocity.Y < 0) && Math.Abs(b.Velocity.Y) < Math.Abs(b.Velocity.X))
                        {
                            lastDir = 3;
                            d = true;
                        }
                    }
                }
                else if (b.Collectible)
                {
                   
                    
                }
            } 
            //Loop
            
            return new EnvShooter.Action()
            {
               
                right = r,
                up = u,
                left = l,
                down = d,
                charge = c,
                aim = (float)rot
            };
        }

    }
}
