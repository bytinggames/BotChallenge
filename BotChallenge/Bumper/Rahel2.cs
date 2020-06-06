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
    class Rahel2 : EnvBumper.Bot
    {
        protected override Color GetColor()
        {
            return Color.Black;
        }

        public Rahel2()
        {

        }

        Vector2 target = new Vector2(0, 0);

        protected override EnvBumper.Action GetAction()
        {

            Vector2 dir = -Pos;// new Vector2(-1,0);
            if (Pos.Length() < 2)
            {
                target=new Vector2(0,0);
            }
                

            if (Vector2.Distance(Pos, target) < 1)
            {
                target = new Vector2(-1, -1);
            }

            dir = target - Pos;
            /*
            if (Pos.Length() < 1)
            {
                dir = new Vector2(-1, -1);
                if (Pos.Length()>5)
                {
                    dir = new Vector2(1, 1);
                }
            }*/

            //EnvBumper.RADIUS;
              //if (Pos.Length()>5) {
                //dir = new Vector2(1, 1);
                //dir.X = Pos.X * -1;
                //dir.Y = Pos.Y * -1;



            //}





            return new EnvBumper.Action()
            {
                angle = (float)Math.Atan2(dir.Y, dir.X),
                accelerate = true
            };
        }

    }
}
