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
    class Niklas : EnvShooter.Bot
    {
        private static double bulletCollsionRadius = 1.8;

        protected override Color GetColor()
        {
            return Color.Green;
        }

        public Niklas()
        {
            //Start
        }

        protected override EnvShooter.Action GetAction()
        {
            //Loop
            EnvShooter.Action test = new EnvShooter.Action()
            {
                right = false,
                up = false,
                left = false,
                down = false,
                charge = false,
                aim = MathHelper.Pi
            };

            Vector2 movementDirection = new Vector2(0, 0);
            Vector2 temp = new Vector2(0, 0);
            Bullet closestCollectible;
            bool isSet = false;

            if(this.env.Bullets.Length > 0)
            {
                closestCollectible = this.env.Bullets[0];

                foreach (Bullet bullet in this.env.Bullets)
                {
                    if (!bullet.Collectible)
                    {
                        
                        if (TestForCollision(bullet.Pos))
                        {
                            temp.X += (this.Pos.X - bullet.Pos.X);
                            temp.Y += (this.Pos.Y - bullet.Pos.Y);

                            movementDirection.X += temp.X;
                            movementDirection.Y += temp.Y;
                        }
                    }
                    else
                    {
                        if (getAbsoluteDistance(closestCollectible.Pos) < getAbsoluteDistance(bullet.Pos) || isSet == false)
                        {
                            closestCollectible = bullet;
                            isSet = true;
                            
                        }
                    }
                }
                /*
                if (isSet)
                {
                    movementDirection.X += this.Pos.X - closestCollectible.Pos.X;
                    movementDirection.Y += this.Pos.Y - closestCollectible.Pos.Y;
                }*/
            }

            //movementDirection = RotateVector(movementDirection, 90);

            movementDirection.Normalize();

            if(movementDirection.Y > 0)
            {
                test.right = false;
                test.left = true;
            }
            else if(movementDirection.Y == 0)
            {

            }
            else
            {
                test.right = true;
                test.left = false;
            }
            if(movementDirection.X > 0)
            {
                test.down = false;
                test.up = true;
            }
            else if(movementDirection.X == 0)
            {

            }
            else
            {
                test.down = true;
                test.up = false;
            }

            return test;
        }

        private double getDistance(Vector2 objectPosition)
        {
            return Math.Sqrt((objectPosition.X - this.Pos.X) * (objectPosition.X - this.Pos.X) + (objectPosition.Y - this.Pos.Y) * (objectPosition.Y - this.Pos.Y));
        }

        private double getAbsoluteDistance(Vector2 objectPosition)
        {
            return Math.Abs(getDistance(objectPosition));
        }

        private Vector2 RotateVector(Vector2 vector, float degrees)
        {
            Vector2 result = new Vector2(0, 0);
            result.X = (float)(vector.X * Math.Cos(degrees) - vector.Y * Math.Sin(degrees));
            result.Y = (float)(vector.X * Math.Sin(degrees) + vector.Y * Math.Cos(degrees));
            return result;
        }

        private bool TestForCollision(Vector2 objectPosition)
        {
            if(getAbsoluteDistance(objectPosition) < bulletCollsionRadius * 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
