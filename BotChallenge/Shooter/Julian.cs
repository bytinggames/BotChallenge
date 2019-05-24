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
    class Julian : EnvShooter.Bot
    {
        EnvShooter.Action action = new EnvShooter.Action()
        {
            right = false,
            up = false,
            left = false,
            down = false,
            charge = false,
            aim = MathHelper.Pi
        };

        protected override Color GetColor()
        {
            return Color.Orange;
        }

        public Julian()
        {
            //Start
        }

        protected override EnvShooter.Action GetAction()
        {
            //Loop

            //this.enemies[0].

            //action = new EnvShooter.Action()
            //{
            //    right = false,
            //    up = false,
            //    left = false,
            //    down = false,
            //    charge = false,
            //    aim = MathHelper.Pi
            //};

            action.right = action.left = action.up = action.down = false;

            float border = 1f / 4f;

            bool wall = false;
            if (Pos.X < env.Width * border)
                action.right = true;
            if (Pos.X > env.Width * (1 - border))
                action.left = true;
            if (Pos.Y < env.Height * border)
                action.down = true;
            if (Pos.Y > env.Height * (1 - border))
                action.up = true;

            if (action.right || action.left || action.up || action.down)
                wall = true;

            Vector2 enemyPos = enemies[0].Pos;

            Vector2 dist = enemyPos - Pos;

            action.aim = (float)Math.Atan2(dist.Y, dist.X);



            Bullet nearestFriendly = null;
            float nearestFriendlyDist = -1f;
            Bullet nearestEnemy = null;
            float nearestEnemyDist = -1f;
            bool normalEvasion = false;
            bool anyFriendly = env.Bullets.Any(f => f.Collectible);
            for (int i = 0; i < env.Bullets.Length; i++)
            {
                dist = env.Bullets[i].Pos - Pos;
                float d = dist.Length();
                if (nearestFriendlyDist != -2 && env.Bullets[i].Collectible)
                {
                    if (nearestFriendlyDist == -1f || d < nearestFriendlyDist)
                    {
                        nearestFriendlyDist = d;
                        nearestFriendly = env.Bullets[i];
                    }
                }
                else
                {
                    //if (d < 1f)
                    if (d < 2f)
                    {
                        if (nearestEnemyDist == -1f || d < nearestEnemyDist)
                        {
                            nearestEnemyDist = d;
                            nearestEnemy = env.Bullets[i];
                        }
                    }
                    else if (!anyFriendly && d > 3f)
                    {
                        if (nearestFriendlyDist == -1f || d < nearestFriendlyDist)
                        {
                            nearestFriendlyDist = d;
                            nearestFriendly = env.Bullets[i];
                        }
                    }


                    if (Vector2.Dot(-dist, env.Bullets[i].Velocity) > 1)
                    {
                        nearestFriendlyDist = -2f;
                        nearestFriendly = null;
                        Vector2 evade = new Vector2(-dist.Y, dist.X);
                        Vector2 nVelocity = new Vector2(-env.Bullets[i].Velocity.Y, env.Bullets[i].Velocity.X);

                        if (Vector2.Dot(nVelocity, dist) < 0)
                            evade = -evade;

                        VelocityToAction(ref action, evade);
                        normalEvasion = true;
                        break;
                    }
                }
            }


            if (enemies.Any(f => f.Ammo == 0) || env.Frame > 5 * 60)
            {
                action.charge = true;
            }

            if (Charge >= 1f)
                action.charge = false;

            float noobDist = -1f;

            if (!normalEvasion)
            {
                EnvShooter.Bot nearestBot = null;
                EnvShooter.Bot nearestNoob = null;
                float botDist = -1f;
                for (int i = 0; i < enemies.Length; i++)
                {
                    float d = (Pos - enemies[i].Pos).Length();
                    if (enemies[i].Ammo > 0)
                    {
                        if (d < 6f && (botDist == -1f || d < botDist))
                        {
                            nearestBot = enemies[i];
                            botDist = d;
                        }
                    }
                    else if (Ammo > 0)
                    {
                        // aggro
                        if (noobDist == -1f || d < noobDist)
                        {
                            noobDist = d;
                            nearestNoob = enemies[i];
                        }
                    }
                }

                if (nearestBot != null)
                {
                    VelocityToAction(ref action, Pos - nearestBot.Pos);
                }
                else if (nearestNoob != null)
                {
                    VelocityToAction(ref action, nearestNoob.Pos - Pos);
                }
            }


            // collect bullets
            if (noobDist == -1f && nearestFriendly != null)
            {
                dist = nearestFriendly.Pos - Pos;

                VelocityToAction(ref action, dist);
            }

            if (!normalEvasion && nearestEnemy != null)
            {
                VelocityToAction(ref action, Pos - nearestEnemy.Pos);
            }
            

            return action;
        }


        void VelocityToAction(ref EnvShooter.Action action, Vector2 velocity)
        {
            action.right = action.left = action.up = action.down = false;
            if (Math.Abs(velocity.X) > RADIUS)
            {
                action.right = velocity.X > 0;
                action.left = velocity.X < 0;
            }
            if (Math.Abs(velocity.Y) > RADIUS)
            {
                action.up = velocity.Y < 0;
                action.down = velocity.Y > 0;
            }
        }
    }
}
