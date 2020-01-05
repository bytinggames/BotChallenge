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
    class Julian1_1 : EnvBumper.Bot
    {
        protected override Color GetColor()
        {
            return Color.Orange;
        }

        public Julian1_1()
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




            //float a = Velocity.X;
            //float b = Velocity.Y;
            //float x = Pos.X;
            //float y = Pos.Y;
            //float r = EnvBumper.RADIUS;
            //float t = (float)Math.Sqrt(a * a * (r * r - y * y) + 2 * a * b * x * y + b * b * (r * r - x * x) + a * x + b * y) / (a * a + b * b);
            //Console.WriteLine(t);

            return new EnvBumper.Action()
            {
                angle = myAngle,
                accelerate = true
            };
        }

    }
}
