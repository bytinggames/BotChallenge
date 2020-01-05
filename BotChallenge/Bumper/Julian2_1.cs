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
    class Julian2_1 : EnvBumper.Bot
    {
        int frames = 0;

        int goAway = 0;

        Vector2 lastVelocity;

        protected override Color GetColor()
        {
            return Color.Orange;
        }

        public Julian2_1()
        {

        }

        protected override EnvBumper.Action GetAction()
        {
            //Pos
            //Velocity
            //enemies[0].Pos
            //enemies[0].Velocity

            Vector2 dist = enemies[0].Pos - Pos;
            

            float myAngle = (float)Math.Atan2(dist.Y, dist.X);

            float distToCenter = Pos.Length();

            float targetToCenter = EnvBumper.RADIUS * 0.5f;
            
            myAngle = (float)Math.Atan2(-Pos.Y, -Pos.X);
            myAngle += MathHelper.PiOver2 * 0.25f;

            float angleToCenter = (float)Math.Atan2(-Pos.Y, -Pos.X);
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
                if (goAway <= 0)
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


            if (distToCenter > EnvBumper.RADIUS * 0.6f)
                myAngle = angleToCenter;
            else if (Velocity.Length() > 0.2f)
                myAngle = (float)Math.Atan2(-Velocity.Y, -Velocity.X);

            frames++;

            lastVelocity = Velocity;
            return new EnvBumper.Action()
            {
                angle = myAngle,
                accelerate = true
            };
        }


        Vector2 Predict(Vector2 pos, Vector2 v)
        {
            return pos + v * Math.Min(200, v.Length() * 80f);
        }
    }
}
