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
    class Dominik0 : EnvBumper.Bot
    {
        protected override Color GetColor()
        {
            return Color.DarkMagenta;
        }

        public Dominik0()
        {

        }

        protected float getAngle(Vector2 vec)
        {
            return (float) Math.Atan2(vec.Y, vec.X); ;
        }

        protected Vector2 rotate(Vector2 vec, float ang) // rotate gegen uhrzeigersinn
        {
            return new Vector2((float)Math.Cos(ang) * vec.X - (float)Math.Sin(ang) * vec.Y, (float)Math.Sin(ang) * vec.X + (float)Math.Cos(ang) * vec.Y);
        }

        protected override EnvBumper.Action GetAction()
        {
            Vector2 center = new Vector2(0, 0);
            Vector2 distance = center - this.Pos;

            float angle_next;
            bool accelerate_next;
            float distFrom0 = this.Pos.Length();
            float outerDistCap = 5f;
            float maxVelocity = 0.25f;
            float rotate_next = 0.1f;


            // Random angle
            //angle_next = (float) Env.constRand.NextDouble() * MathHelper.TwoPi;

            angle_next = getAngle(distance);

            // cap velocity
            if (this.Velocity.Length() > maxVelocity) accelerate_next = false;
            else accelerate_next = true;

            
            if (distFrom0 > outerDistCap)
            {
                rotate_next = -0.1f;
            }

            angle_next = getAngle(rotate(distance, rotate_next));

            return new EnvBumper.Action()
            {
                angle = angle_next,
                accelerate = accelerate_next
            };
        }

    }
}
