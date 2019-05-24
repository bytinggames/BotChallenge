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
    class Dominik : EnvShooter.Bot

    {
        private bool dir_right, dir_left, dir_up, dir_down;

        protected override Color GetColor()
        {
            return Color.Aquamarine;
        }

        public Dominik()
        {
            //Start
        }

        protected override EnvShooter.Action GetAction()
        {
            //Loop

            EnvShooter.Action act = new EnvShooter.Action()
            {
                right = false,
                left = false,
                up = false,
                down = false,
                charge = false,
                aim = -MathHelper.Pi / 2
            };
            
            int dec = Env.constRand.Next(4);

            act.right = (dec == 0);
            act.left = (dec == 1);
            act.up = (dec == 2);
            act.down = (dec == 3);

            Vector2 vec = enemies[0].Pos - this.Pos;

            if (this.Ammo > 0) act.charge = true;
            if (this.Charge == 1)
            {
                act.charge = false;
                act.aim = (float) Math.Atan2(vec.Y,vec.X);
            }

            float wurzel = (float) (1 / Math.Sqrt(2));
            float[] test = new float[8]{ vec.X, wurzel * (vec.X) + wurzel * (vec.Y), vec.Y, -wurzel * (vec.X) + wurzel * (vec.Y), -vec.X, -wurzel * (vec.X) - wurzel * (vec.Y), -vec.Y, wurzel* (vec.X) - wurzel * (vec.Y)};;
            

            /*
            if (Math.Abs(vec.X) > Math.Abs(vec.Y))
            {
                act.right = false;
                act.left = false;
                if (vec.Y > 0) { act.up = true; act.down = false; }
                else { act.up = false; act.down = true; }
            }
            else
            {
                act.up = false;
                act.down = false;
                if (vec.X > 0) { act.left = true; act.right = false; }
                else { act.left = false; act.right = true; }
            }
            */



            return act;
        }

    }
}
