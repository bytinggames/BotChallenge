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
    class Morpheus0 : EnvBumper.Bot
    {
        float FRICTION = 0.97f;

        protected override Color GetColor()
        {
            return Color.White;
        }

        public Morpheus0()
        {

        }

        private Tuple<Vector2, Vector2> _itr_dist(float _angle)
        {
            Vector2 dir = new Vector2((float)Math.Cos(_angle),(float)Math.Sin(_angle));

            Vector2 dist = dir * (EnvBumper.RADIUS- 3.0f) * 2;
            Vector2 scale = dist / 2.0f;
            int itr = 0;

            while (scale.Length() > 0.0001 && itr < 10000)
            {
                if ((Pos + (dist - scale)).Length() < (EnvBumper.RADIUS - 3.0f))
                {
                    scale /= 2.0f;
                }
                else
                {
                    dist -= scale;
                }
                itr++;

            }

            return new Tuple<Vector2,Vector2>(Pos+dist,dist);
        }

        float c_angle = -1;
        float l_angle = -1;
        float mid_point = -1;
        bool init = false;
        bool vel_x_neg = false;

        private void new_wall_target(bool keep_angle=false)
        {
            if (!keep_angle)
                c_angle = (float)(EnvBumper.constRand.NextDouble() * Math.PI * 2f);
            else
                c_angle += (float)(EnvBumper.constRand.NextDouble() * (Math.PI)) - (float)(Math.PI/2.0f);
            l_angle = c_angle;
            mid_point = _itr_dist(c_angle).Item2.Length()/2.0f;

            vel_x_neg = _itr_dist(c_angle).Item2.X < 0;

            //Console.WriteLine(mid_point+" - " + c_angle+" - "+ _itr_dist(c_angle).Item2.Length());
        }

        bool hard_reset = false;
        protected override EnvBumper.Action GetAction()
        {
            if (!init)
            {
                new_wall_target();
                init = true;
            }
            
            var _d = _itr_dist(l_angle);
            Vector2 dist = _d.Item2;
            bool move = true;
            

            if (mid_point != -1 && dist.Length() <= mid_point)
            {
                c_angle += (float)Math.PI;
                mid_point = -1;
            }
            else if (mid_point == -1 && ((Velocity.X < 0) != vel_x_neg) && Velocity.Length() > 0.01f)
            {
                move = false;
            }
            else if (mid_point == -1 && ((Velocity.X < 0) != vel_x_neg) && Velocity.Length() <= 0.01f)
            {
                //Console.WriteLine("New Wall Target");
                new_wall_target(true);
            }

            /*
            if (dist.Length() > EnvBumper.RADIUS)
            {
                c_angle = (float)Math.Atan2(-Pos.Y, -Pos.X);
                hard_reset = true;
            }
            if (hard_reset)
            {
                if (Velocity.Length() <= 0.01f)
                {
                    hard_reset = false;
                    new_wall_target();
                }
            }*/
            

            Console.WriteLine(dist.Length() + " - " + mid_point + " - " + l_angle + " - "+move + " - "+Velocity.Length());



            return new EnvBumper.Action()
            {
                angle = c_angle,//dir != Vector2.Zero ? (float)Math.Atan2(dir.Y, dir.X) : 0f,
                accelerate = move//dir != Vector2.Zero
            };
        }

    }
}
