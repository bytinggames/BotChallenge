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
    class Julian4 : EnvShooter.Bot
    {
        const float dangerDist = 6;
        const float chargeDist = 4;


        Color color = Color.Orange;

        EnvShooter.Action action = new EnvShooter.Action()
        {
            right = false,
            up = false,
            left = false,
            down = false,
            charge = false,
            aim = MathHelper.Pi
        };

        enum State
        {
            EvadeBullet,
            //AvoidEnemyCharge,
            FleeInsideEnemy,
            CollectBullets,
            TerrorizeNoob, // enemy has no ammo
            Flee, // no ammo, enemy has ammo
            Attack,
            None,
        }

        State state;


        protected override Color GetColor()
        {
            return color;
        }

        public Julian4()
        {
            //Start
            state = State.None;
        }

        bool reverseDir = false;


        public float Raycast(Vector2 origin, Vector2 v)
        {
            int iterations = 10;
            for (int i = 0; i < iterations; i++)
            {
                origin += v;

                if (Vector2.Distance(origin, v) < (RADIUS + Bullet.RADIUS) * 1.5f)
                    return ((float)iterations - i) / iterations * v.Length();

                v *= 0.99f;
            }

            return 0f;
        }

        class Nearest<T>
        {
            public T obj;
            public float dist = -1;

            public bool SetIfNearer(float d, T obj)
            {
                if (dist == -1 || d < dist)
                {
                    dist = d;
                    this.obj = obj;
                    return true;
                }
                return false;
            }

            public bool Available()
            {
                return dist != -1;
            }
        }

        protected override EnvShooter.Action GetAction()
        {
            color = Color.Orange;

            action.right = action.left = action.up = action.down = false;
            action.charge = false;

            if (Keyboard.GetState().IsKeyDown(Keys.E))
            { }

            #region determine state

            #region gather bullet data

            Vector2 dist;

            Vector2 distToEnemy = enemies[0].Pos - Pos;
            float dToEnemy = distToEnemy.Length();

            Nearest<Bullet> nearestFriendlyBullet = new Nearest<Bullet>();
            Nearest<Bullet> nearestEvilBullet = new Nearest<Bullet>();
            Nearest<Bullet> nearestEvilBulletFocus = new Nearest<Bullet>();

            for (int i = 0; i < env.Bullets.Length; i++)
            {
                dist = env.Bullets[i].Pos - Pos;
                float d = dist.Length();
                if (env.Bullets[i].Collectible)
                {
                    //float danger = Raycast(env.Bullets[i].Pos, env.Bullets[i].Velocity);

                    nearestFriendlyBullet.SetIfNearer(d, env.Bullets[i]);
                }
                else
                {
                    float maxFriendlyBulletSpeed = 0.3f;

                    if (d < EnvShooter.Bot.RADIUS + Bullet.RADIUS + SPEED * 2f)
                    {
                        nearestEvilBullet.SetIfNearer(d, env.Bullets[i]);
                    }
                    else if (d > 3f && env.Bullets[i].Velocity.Length() < maxFriendlyBulletSpeed) // 0.1f is neutralize speed
                    {
                        float v = env.Bullets[i].Velocity.Length();
                        v -= 0.1f;
                        v /= maxFriendlyBulletSpeed - 0.1f;

                        nearestFriendlyBullet.SetIfNearer(d * (1f + v), env.Bullets[i]);
                    }
                    if (d < dangerDist)
                    {
                        float dot = Vector2.Dot(-Vector2.Normalize(env.Bullets[i].Pos - Vector2.Normalize(env.Bullets[i].Velocity) * 1f - Pos), env.Bullets[i].Velocity);
                        if (dot > 0f)
                        {
                            float projection = Vector2.Dot(dist, Vector2.Normalize(new Vector2(-env.Bullets[i].Velocity.Y, env.Bullets[i].Velocity.X)));

                            if (Math.Abs(projection) < RADIUS + Bullet.RADIUS + 0.1f * 2f) // check if it will hit eventually (also consider, that he will move in this direction (*2f)
                            {
                                //color = Color.Red;

                                nearestEvilBulletFocus.SetIfNearer(d, env.Bullets[i]);
                            }
                        }
                    }
                }
            }

            #endregion



            State s = state;

            if (nearestEvilBulletFocus.Available() || nearestEvilBullet.Available())// if attacking bullet near
                state = State.EvadeBullet;
            else if (enemies[0].Charge > 0 && dToEnemy < dangerDist && Charge < 1f && (Health < 0.5f || enemies[0].Health > 0.25f))// && dToEnemy - RADIUS * 1.5f < (1f - enemies[0].Charge) / EnvShooter.Bot.CHARGESPEED * SPEED * 0.75f)
            {
                if (dToEnemy - RADIUS * 1.5f < (1f - enemies[0].Charge) / EnvShooter.Bot.CHARGESPEED * SPEED * 0.75f) // 0.75f because he can only walk in 8 directions
                    state = State.FleeInsideEnemy;
                else
                    state = State.Flee;
            }
            else if (nearestFriendlyBullet.Available() && nearestFriendlyBullet.dist < dToEnemy * 0.5f) // bullet to collect near
                state = State.CollectBullets;
            else if (enemies[0].Ammo == 0 && Ammo > 0)
                state = State.TerrorizeNoob;
            else if (Ammo == 0 && enemies[0].Ammo > 0)
                state = State.Flee;
            else if (Ammo > 0)
                state = State.Attack;
            else if (nearestFriendlyBullet.Available())
                state = State.CollectBullets;
            else
                state = State.Flee;

            //if (s != state)
            //    Console.WriteLine(env.Frame + " " + state);

            #endregion

            #region general

            if (dToEnemy < chargeDist)
            {
                if (enemies[0].Charge == 0 || Charge > 0.2f) // only charge if you already started, or if your enemy isn't charging
                {
                    action.charge = true;
                }
                float distFraction = 0.75f;
                if (enemies[0].Charge >= 1f)
                    distFraction = 0.9f;
                else if (enemies[0].Charge > 0f)
                    distFraction = 1f;
                if (Charge >= 1f && dToEnemy < chargeDist * distFraction && OutsideOfEnemy(dToEnemy - 0.5f)) // 0.5f for shooting distance
                    action.charge = false;
            }

            #endregion

            switch (state)
            {
                case State.EvadeBullet:

                    if (nearestEvilBulletFocus.Available())
                    {
                        dist = nearestEvilBulletFocus.obj.Pos - Pos;
                        Vector2 evade = new Vector2(-dist.Y, dist.X);
                        Vector2 nVelocity = new Vector2(-nearestEvilBulletFocus.obj.Velocity.Y, nearestEvilBulletFocus.obj.Velocity.X);
                        if (Vector2.Dot(nVelocity, dist) < 0)
                            evade = -evade;

                        VelocityToAction(ref action, evade);

                        // stop charging in order to evade
                        if (action.charge && (Charge > 0.7f || Charge < 0.2f))
                            action.charge = false;
                    }
                    else
                    {
                        VelocityToAction(ref action, Pos - nearestEvilBullet.obj.Pos);
                    }

                    break;
                case State.CollectBullets:

                    VelocityToAction(ref action, nearestFriendlyBullet.obj.Pos - Pos);

                    break;
                case State.TerrorizeNoob:

                    if (OutsideOfEnemy(dToEnemy - 1f))
                    {
                        VelocityToAction(ref action, distToEnemy);
                    }
                    else
                        VelocityToAction(ref action, -distToEnemy);


                    break;

                case State.FleeInsideEnemy:
                    VelocityToActionExact(ref action, distToEnemy);
                    break;

                case State.Flee:

                    Vector2 center = new Vector2(env.Width, env.Height) / 2f;

                    float distToCenter = Vector2.Distance(center, Pos);

                    distToCenter /= env.Width / 2f - 1f - RADIUS; // 0 - 1 (max distance to center in x OR y direction)

                    if (distToCenter > 0.9f)
                    {
                        //TODO: check in which direction to go
                        Vector2 a = Pos - center;
                        Vector2 b = enemies[0].Pos - center;
                        b = new Vector2(-b.Y, b.X);
                        a.Normalize();
                        b.Normalize();

                        float dot = Vector2.Dot(a, b);
                        if (Math.Abs(dot) > 0.01f)
                        {
                            if (dot < 0f)
                                VelocityToAction(ref action, new Vector2(-distToEnemy.Y, distToEnemy.X));
                            else
                                VelocityToAction(ref action, new Vector2(distToEnemy.Y, -distToEnemy.X));
                        }
                    }
                    else
                    {
                        VelocityToAction(ref action, -distToEnemy);
                    }

                    break;

                case State.Attack:

                    if (dToEnemy > chargeDist * 0.7f)
                    {
                        VelocityToAction(ref action, distToEnemy);
                    }
                    else
                        VelocityToAction(ref action, -distToEnemy);

                    break;
            }

            if (!action.charge)
            {
                action.aim = (float)Math.Atan2(distToEnemy.Y, distToEnemy.X);

                float definitiveDistMax = chargeDist * 0.5f;

                if (dToEnemy < definitiveDistMax)
                {
                    Vector2 enemyBorderDist = enemies[0].Pos + Vector2.Normalize(new Vector2(-distToEnemy.Y, distToEnemy.X)) * EnvShooter.Bot.RADIUS;
                    enemyBorderDist = enemyBorderDist - Pos;

                    float maxAngle = (float)Math.Atan2(enemyBorderDist.Y, enemyBorderDist.X);
                    float angleDist1 = action.aim - maxAngle;
                    float angleDist2 = action.aim + MathHelper.TwoPi;
                    if (Math.Abs(angleDist2) < Math.Abs(angleDist1))
                        angleDist1 = angleDist2;

                    angleDist1 *= 0.8f; // just in case the evasion of the enemy is very good

                    float min = RADIUS + Bullet.RADIUS + 0.5f + 0.5f;
                    float max = definitiveDistMax;

                    float d = dToEnemy;
                    d -= min;
                    d /= max - min;

                    action.aim += angleDist1 * (1f - d);//(float)Math.Pow((1f - (dToEnemy / definitiveDistMax)) * 2f, 0.5f) * angleDist1;
                }
            }


            if (!action.charge && Charge == 0f && rand.Next(60 * 5) == 0)
                action.charge = true;

            //Loop

            //action.right = action.left = action.up = action.down = false;

            //float border = 1f / 4f;

            //bool wall = false;
            //if (Pos.X < env.Width * border)
            //    action.right = true;
            //if (Pos.X > env.Width * (1 - border))
            //    action.left = true;
            //if (Pos.Y < env.Height * border)
            //    action.down = true;
            //if (Pos.Y > env.Height * (1 - border))
            //    action.up = true;

            //if (action.right || action.left || action.up || action.down)
            //    wall = true;

            //Vector2 enemyPos = enemies[0].Pos;

            //Vector2 dist = enemyPos - Pos;





            //if (enemies.Any(f => f.Ammo == 0) || Vector2.Distance(enemies[0].Pos, Pos) < 6f || env.Frame > 5 * 60 )
            //{
            //    action.charge = true;
            //}

            //if (Charge >= 1f)
            //{
            //    if (rand.Next(300) == 0 || enemies[0].Charge > 0 || Vector2.Distance(enemies[0].Pos, Pos) < 5f)
            //        action.charge = false;
            //}

            //Bullet nearestFriendly = null;
            //float nearestFriendlyDist = -1f;
            //Bullet nearestEnemy = null;
            //float nearestEnemyDist = -1f;
            //bool bulletEvasion = false;
            //bool anyFriendly = env.Bullets.Any(f => f.Collectible);

            //for (int i = 0; i < env.Bullets.Length; i++)
            //{
            //    dist = env.Bullets[i].Pos - Pos;
            //    float d = dist.Length();
            //    if (nearestFriendlyDist != -2 && env.Bullets[i].Collectible)
            //    {
            //        float danger = Raycast(env.Bullets[i].Pos, env.Bullets[i].Velocity);

            //        if (nearestFriendlyDist == -1f || danger > nearestFriendlyDist)//d < nearestFriendlyDist)
            //        {
            //            nearestFriendlyDist = d;
            //            nearestFriendly = env.Bullets[i];
            //        }
            //    }
            //    else
            //    {
            //        //if (d < 1f)
            //        if (d < 2f)
            //        {
            //            if (nearestEnemyDist == -1f || d < nearestEnemyDist)
            //            {
            //                nearestEnemyDist = d;
            //                nearestEnemy = env.Bullets[i];
            //            }
            //        }
            //        else if (!anyFriendly && d > 3f)
            //        {
            //            if (nearestFriendlyDist == -1f || d < nearestFriendlyDist)
            //            {
            //                nearestFriendlyDist = d;
            //                nearestFriendly = env.Bullets[i];
            //            }
            //        }


            //        if (Vector2.Dot(-dist, env.Bullets[i].Velocity) > 1)
            //        {
            //            nearestFriendlyDist = -2f;
            //            nearestFriendly = null;
            //            Vector2 evade = new Vector2(-dist.Y, dist.X);
            //            Vector2 nVelocity = new Vector2(-env.Bullets[i].Velocity.Y, env.Bullets[i].Velocity.X);

            //            if (Vector2.Dot(nVelocity, dist) < 0)
            //                evade = -evade;

            //            VelocityToAction(ref action, evade);
            //            bulletEvasion = true;

            //            if (Charge > 0.7f)
            //                action.charge = false;
            //            break;
            //        }
            //    }
            //}

            //bool wallSlide = false;

            //if (!bulletEvasion && env.Bullets.Length == 0 && Ammo == 0)
            //{
            //    // emergency mode! -> evasion extreme
            //    action.left = action.right = action.up = action.down = false;


            //    if (Pos.X < env.Width * border)
            //        action.right = true;
            //    if (Pos.X > env.Width * (1 - border))
            //        action.left = true;
            //    if (Pos.Y < env.Height * border)
            //        action.down = true;
            //    if (Pos.Y > env.Height * (1 - border))
            //        action.up = true;

            //    bool u = action.up;
            //    action.up = action.right;
            //    action.right = action.down;
            //    action.down = action.left;
            //    action.left = u;

            //    Vector2 dir = new Vector2(action.up ? -1f : action.down ? 1f : 0, action.right ? 1f : action.left ? -1f : 0f);

            //    if (rand.Next(60) == 0 && reverseDir != Vector2.Dot(dir, enemies[0].Pos - Pos) > 0f)
            //        reverseDir = !reverseDir;

            //        if (reverseDir) 
            //    {
            //        bool a = action.up;
            //        action.up = action.down;
            //        action.down = a;

            //        a = action.right;
            //        action.right = action.left;
            //        action.left = a;
            //    }

            //    /*
            //    if (action.right)
            //    {


            //        if (action.up)
            //            action.left = true;
            //        else
            //            action.up = true;
            //    }

            //    if (Pos.Y > env.Height * (1 - border))
            //        action.right = true;
            //    else if (Pos.Y < env.Height * border)
            //        action.left = true;
            //    else if (Pos.X > env.Width * (1 - border))
            //        action.up = true;
            //    else if (Pos.X < env.Width * border)
            //        action.down = true;*/


            //    if (action.right || action.left || action.up || action.down)
            //        wallSlide = true;
            //}

            //if (!wallSlide)
            //{
            //    float noobDist = -1f;

            //    if (!bulletEvasion)
            //    {
            //        EnvShooter.Bot nearestBot = null;
            //        EnvShooter.Bot nearestNoob = null;
            //        float botDist = -1f;
            //        for (int i = 0; i < enemies.Length; i++)
            //        {
            //            float d = (Pos - enemies[i].Pos).Length();

            //            if (enemies[i].Ammo > 0)
            //            {
            //                if ((d < 10f || (enemies[i].Charge > 0 && d < 15f)) && (botDist == -1f || d < botDist))
            //                {
            //                    nearestBot = enemies[i];
            //                    botDist = d;
            //                }
            //            }
            //            else if (Ammo > 0)
            //            {
            //                //// aggro
            //                //if (noobDist == -1f || (d < noobDist && d > 2f))
            //                //{
            //                //    noobDist = d;
            //                //    nearestNoob = enemies[i];
            //                //}
            //            }
            //        }

            //        if (nearestBot != null)
            //        {
            //            VelocityToAction(ref action, Pos - nearestBot.Pos);


            //        }
            //        else if (nearestNoob != null)
            //        {
            //            VelocityToAction(ref action, nearestNoob.Pos - Pos);
            //        }
            //    }

            //    // collect bullets
            //    if (noobDist == -1f && nearestFriendly != null)
            //    {
            //        dist = nearestFriendly.Pos - Pos;

            //        VelocityToAction(ref action, dist);
            //    }
            //}



            //if (!bulletEvasion && nearestEnemy != null)
            //{
            //    VelocityToAction(ref action, Pos - nearestEnemy.Pos);
            //}


            return action;
        }

        private bool OutsideOfEnemy(float dToEnemy)
        {
            return dToEnemy > RADIUS + Bullet.RADIUS + 0.5f; // 0.5f space
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

            SetVelocityAccordingToWalls(ref action);
        }
        void VelocityToActionExact(ref EnvShooter.Action action, Vector2 velocity)
        {
            action.right = velocity.X > 0;
            action.left = velocity.X < 0;
            action.up = velocity.Y < 0;
            action.down = velocity.Y > 0;

            SetVelocityAccordingToWalls(ref action);
        }

        void SetVelocityAccordingToWalls(ref EnvShooter.Action action)
        {
            if (action.right && Pos.X > env.Width - 1f - RADIUS - SPEED)
                action.right = false;
            if (action.left && Pos.X < 1f + RADIUS + SPEED)
                action.left = false;

            if (action.down && Pos.Y > env.Height - 1f - RADIUS - SPEED)
                action.down = false;
            if (action.up && Pos.Y < 1f + RADIUS + SPEED)
                action.up = false;


        }
    }
}
