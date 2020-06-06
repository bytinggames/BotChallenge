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
    class Dominik2 : EnvBumper.Bot
    {
        protected override Color GetColor()
        {
            return Color.Magenta;
        }

<<<<<<< HEAD
        public Dominik2()
        {

        }

=======
>>>>>>> 88e0a94df6ec867171c7a3406eb0266d9760f99e
        protected float getAngle(Vector2 vec)
        {
            return (float) Math.Atan2(vec.Y, vec.X); ;
        }

        protected Vector2 rotate(Vector2 vec, float ang) // rotate gegen uhrzeigersinn
        {
            return new Vector2((float)Math.Cos(ang) * vec.X - (float)Math.Sin(ang) * vec.Y, (float)Math.Sin(ang) * vec.X + (float)Math.Cos(ang) * vec.Y);
        }

        protected Vector2 transform(Vector2 vec)
        {
            return new Vector2(vec.X, -vec.Y);
        }

        protected override EnvBumper.Action GetAction()
        {
            Vector2 distanceTo0 = - transform(this.Pos);

            float angle_next;
            bool accelerate_next;
            float distFrom0 = transform(this.Pos).Length();
            float outerDistCap = 7f;
            float maxVelocity = float.PositiveInfinity;
            float rotate_next = 0.1f;
            Vector2 mathVelocity = transform(this.Velocity);
            

            // velocity cap
            if (mathVelocity.Length() > maxVelocity) accelerate_next = false;
            else accelerate_next = true;

            // radius cap
            if (distFrom0 > outerDistCap)
            {
                rotate_next = -0.1f;
            }

            angle_next = -getAngle(rotate(distanceTo0, rotate_next));


            // attention!
            int movesPredict = 50;
            Vector2[] poss = new Vector2[movesPredict];
            if ((movesPredict * mathVelocity + transform(this.Pos)).Length() > EnvBumper.RADIUS)
            {
                //Console.Write("!");

                Vector2 vel_rotated = rotate(mathVelocity, (float)(3f * Math.PI) / 4f);
                if (Vector2.Dot(vel_rotated, mathVelocity) > 0)
                {
                    vel_rotated = -vel_rotated;
                }

                

                angle_next = -getAngle(distanceTo0);

                /*Console.Write(mathVelocity);
                Console.Write(-angle_next);*/
                accelerate_next = true;
            }


            return new EnvBumper.Action()
            {
                angle = angle_next,
                accelerate = accelerate_next
            };
        }

    }
}
