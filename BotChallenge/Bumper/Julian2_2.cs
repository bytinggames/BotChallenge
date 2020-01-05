using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.Bumper;
using JuliHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bumper
{
    class Julian2_2 : EnvBumper.Bot
    {
        int frames = 0;

        int goAway = 0;

        Vector2 lastVelocity;

        int attack = 0;

        float clockwise = 1f;

        int fleeStreak = 0;

        protected override Color GetColor()
        {
            return Color.Orange;
        }

        public Julian2_2()
        {

        }

        protected override EnvBumper.Action GetAction()
        {
            //Pos
            //Velocity
            //enemies[0].Pos
            //enemies[0].Velocity

            Vector2 dist = enemies[0].Pos - Pos;
            Vector2 distN = Vector2.Normalize(dist);

            float vDist = Vector2.Dot(distN, Velocity);
            float vEDist = Vector2.Dot(distN, enemies[0].Velocity);
            float resultVDist = vDist + vEDist;

            float myAngle = (float)Math.Atan2(dist.Y, dist.X);

            if (resultVDist > 0)
            {
                // start attack
                attack = 1;
            }
            else
            {
                float vChange = Vector2.Distance(lastVelocity, Velocity);
                //Console.WriteLine(vChange);

                if (vChange > 0.1f && Vector2.Dot(Velocity, dist) < 0)
                {
                    // end attack
                    attack = 0;
                }
            }
            attack = 1;
            if (attack == 1)
            {
                // attack
                Console.Write("a");

                Vector2 target;
                Vector2 distPredict;
                if (Vector2.Dot(Velocity, dist) > 0)
                {
                    float time = dist.Length() / (Vector2.Dot(distN, Velocity - enemies[0].Velocity));
                    time *= 0.8f;
                    target = Predict(enemies[0].Pos, enemies[0].Velocity, time);//Velocity.Length() * 70f);
                    distPredict = target - Pos;// Predict(Pos, Velocity, time);

                }
                else
                {
                    distPredict = enemies[0].Pos - Pos;

                }
                myAngle = Calculate.VectorToAngle(distPredict);

                fleeStreak--;

                if (fleeStreak < 180)
                {
                    attack = 0;
                    fleeStreak = 0;
                }
            }
            else
            {
                // flee
                Console.Write("f");
                myAngle += MathHelper.Pi;

                fleeStreak++;

                if (fleeStreak > 120)
                {
                    attack = 1;
                    fleeStreak = 0;
                }
            }

            float distToCenter = Pos.Length();
            float angleToCenter = (float)Math.Atan2(-Pos.Y, -Pos.X);
            /*



            float targetToCenter = EnvBumper.RADIUS * 0.5f;
            
            myAngle = (float)Math.Atan2(-Pos.Y, -Pos.X);
            myAngle += MathHelper.PiOver2 * 0.25f;

            float angleOrtho = angleToCenter + MathHelper.PiOver2;


            float lerp = Velocity.Length() * 6f;
            lerp = Math.Min(1, lerp);
            lerp = 1 - lerp;
            myAngle = angleToCenter - MathHelper.PiOver2 * lerp;


            Vector2 predictedPos = Predict(Pos, Velocity);
            
            //float a = Velocity.X;
            //float b = Velocity.Y;
            //float x = Pos.X;
            //float y = Pos.Y;
            //float r = EnvBumper.RADIUS;
            //float t = (float)Math.Sqrt(a * a * (r * r - y * y) + 2 * a * b * x * y + b * b * (r * r - x * x) + a * x + b * y) / (a * a + b * b);
            //Console.WriteLine(t);

            //if (frames > 60 * 5)
            {
e                if (goAway <= 0)
                {

                    Vector2 target = Predict(enemies[0].Pos, enemies[0].Velocity);// enemies[0].Pos + enemies[0].Velocity * Math.Min(200, enemies[0].Velocity.Length() * 80f);

                    dist = target - predictedPos;

                    myAngle = (float)Math.Atan2(dist.Y, dist.X);

                    float angleFromCenterToEnemy;

                    Vector2 myGoal = enemies[0].Pos * 0.5f;
                    Vector2 goalDist = myGoal - predictedPos;
                    angleFromCenterToEnemy = (float)Math.Atan2(goalDist.Y, goalDist.X);
                    float angleDist = Calculate.AngleDistance(myAngle, angleFromCenterToEnemy);
                    myAngle += angleDist * 0.25f;
                    float vChange = Vector2.Distance(lastVelocity, Velocity);
                    Console.WriteLine(vChange);
                    if (vChange > 0.1f && Vector2.Dot(Velocity, dist) < 0)
                    {
                        //goAway = 30;
                    }
                }
                else
                {
                   // myAngle = (float)Math.Atan2(-dist.Y, -dist.X);
                    goAway--;
                }

            }
            */

            //if (distToCenter > EnvBumper.RADIUS * 0.6f)
            if (distToCenter > EnvBumper.RADIUS * (0.7f - (Velocity.Length() * 2f)))
            {
                //myAngle = angleToCenter;


                float lerp = Velocity.Length() * 15f;
                lerp = Math.Min(1, lerp);
                lerp = 1 - lerp;
                myAngle = angleToCenter + MathHelper.PiOver2 * lerp * clockwise;

                if (Env.constRand.Next(120) == 0)
                    clockwise = -clockwise;

            }
            //else if (Velocity.Length() > 0.2f)
            //    myAngle = (float)Math.Atan2(-Velocity.Y, -Velocity.X);

            frames++;

            lastVelocity = Velocity;
            return new EnvBumper.Action()
            {
                angle = myAngle,
                accelerate = true
            };
        }


        Vector2 Predict(Vector2 pos, Vector2 v, float frames)
        {
            return pos + v * Math.Min(200, frames);
        }
    }
}
