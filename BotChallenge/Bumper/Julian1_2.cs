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
    class Julian1_2 : EnvBumper.Bot
    {
        int frames = 0;

        protected override Color GetColor()
        {
            return Color.Orange * 0.5f;
        }

        public Julian1_2()
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
            myAngle = angleToCenter + MathHelper.PiOver2 * lerp;


            //Vector2 predictedPos = Pos + Velocity * Math.Min(
            
            //float a = Velocity.X;
            //float b = Velocity.Y;
            //float x = Pos.X;
            //float y = Pos.Y;
            //float r = EnvBumper.RADIUS;
            //float t = (float)Math.Sqrt(a * a * (r * r - y * y) + 2 * a * b * x * y + b * b * (r * r - x * x) + a * x + b * y) / (a * a + b * b);
            //Console.WriteLine(t);

            if (frames > 60 * 3)
            {
                Vector2 target = enemies[0].Pos + enemies[0].Velocity * Math.Min(10, enemies[0].Velocity.Length() * 10f);

                dist = target - Pos;

                myAngle = (float)Math.Atan2(dist.Y, dist.X);
            }


            if (distToCenter > EnvBumper.RADIUS * 0.6f)
                myAngle = angleToCenter;
            else if (Velocity.Length() > 0.2f)
                myAngle = (float)Math.Atan2(-Velocity.Y, -Velocity.X);

            frames++;

            return new EnvBumper.Action()
            {
                angle = myAngle,
                accelerate = true
            };
        }

    }
}
