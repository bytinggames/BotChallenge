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
    class Niklas3 : EnvShooter.Bot
    {
        private static double bulletCollsionRadius = 7;

        protected override Color GetColor()
        {
            return Color.DarkGoldenrod;
        }

        public Niklas3()
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
            Vector2 closestCollectiblePosition = this.Pos;
            bool isSet = false;
            bool isDodging = false;
            int closestEnemeyindex = 0;
            int maxAmmo = 0;
            Vector2 dangerousPos = new Vector2();
            Vector2 dangerousVel = new Vector2();
            Vector2 dangerous2 = new Vector2();

            if(this.env.Bullets.Length > 0)
            {

                foreach (Bullet bullet in this.env.Bullets)
                {
                    if (!bullet.Collectible)
                    {

                        if (TestForCollision(bullet.Pos, bulletCollsionRadius * bullet.Velocity.Length() + 1.8))
                        {
                            isDodging = true;
                            movementDirection += RotateVector(bullet.Velocity, 90);
                        }
                        else if(TestForCollision(bullet.Pos, bulletCollsionRadius + 1) && bullet.Velocity.Length() > 0.8f)
                        {
                            isDodging = true;
                            movementDirection += RotateVector(bullet.Velocity, 90);
                        }
                        dangerousPos = bullet.Pos;
                        dangerousVel = bullet.Velocity;
                    }
                    else
                    {
                        if (isSet == false)
                        {
                            isSet = true;
                            closestCollectiblePosition = bullet.Pos;
                        }
                        else
                        {
                            if (getAbsoluteDistance(closestCollectiblePosition) > getAbsoluteDistance(bullet.Pos))
                            {
                                closestCollectiblePosition = bullet.Pos;
                            }
                        }
                    }
                }
            }
            /*
            for (int i = 0; i < this.enemies.Length; i++)
            {
                if(this.enemies[i].Alive)
                    maxAmmo += this.enemies[i].Ammo;
            }*/
            maxAmmo += this.Ammo;
            //maxAmmo += this.env.Bullets.Length;

            if (this.Ammo == maxAmmo)
            {
                test.charge = true;
            }

            
            if(this.Ammo == maxAmmo && this.Charge > 0.99)
            {
                for(int i = 0; i < this.enemies.Length; i++)
                {
                    if (!enemies[i].Alive)
                        continue;

                    if (getAbsoluteDistance(this.enemies[closestEnemeyindex].Pos) < getAbsoluteDistance(this.enemies[i].Pos))
                    {
                        closestEnemeyindex = i;
                    }
                }

                test.aim = (float)Math.Atan2(this.enemies[closestEnemeyindex].Pos.Y - this.Pos.Y, this.enemies[closestEnemeyindex].Pos.X - this.Pos.X);
                test.charge = false;
            }

            if(isDodging)
            {
                if(test.charge == true)
                {
                    for (int i = 0; i < this.enemies.Length; i++)
                    {
                        if (!enemies[i].Alive)
                            continue;

                        if (getAbsoluteDistance(this.enemies[closestEnemeyindex].Pos) < getAbsoluteDistance(this.enemies[i].Pos))
                        {
                            closestEnemeyindex = i;
                        }
                    }

                    test.aim = (float)Math.Atan2(this.enemies[closestEnemeyindex].Pos.Y - this.Pos.Y, this.enemies[closestEnemeyindex].Pos.X - this.Pos.X);
                    test.charge = false;
                }
                dangerous2 = dangerousPos * (dangerousVel * 100);
                if( 0 <= ((this.Pos.X - dangerousPos.X) * (dangerous2.Y - dangerousPos.Y) - (this.Pos.Y - dangerousPos.Y) * (dangerous2.X - dangerousPos.X)))
                    movementDirection = RotateVector(movementDirection, 90);
                else
                    movementDirection = RotateVector(movementDirection, 270);
            }
            else if(isSet)
            {
                movementDirection.X += closestCollectiblePosition.X - Pos.X;
                movementDirection.Y += closestCollectiblePosition.Y - Pos.Y;
            }

            if(!(movementDirection.X == 0) && !(movementDirection.Y == 0))
            {
                movementDirection.Normalize();
            }
            else
            {
                movementDirection.X = rand.Next() % 2 - 0.5f;
                movementDirection.Y = rand.Next() % 2 - 0.5f;
            }

            if(movementDirection.Y > 0.1)
            {
                test.down = true;
                test.up = false;
            }
            else if(movementDirection.Y < -0.1)
            {
                test.down = false;
                test.up = true;
            }
            else
            {

            }
            if(movementDirection.X > 0.1)
            {
                test.right = true;
                test.left = false;
            }
            else if(movementDirection.X < -0.1)
            {
                test.right = false;
                test.left = true;
            }
            else
            {

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

        private bool TestForCollision(Vector2 objectPosition, double radius)
        {
            if(getAbsoluteDistance(objectPosition) < radius * 2)
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
