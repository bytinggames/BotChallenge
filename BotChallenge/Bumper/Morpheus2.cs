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
    class Morpheus2 : EnvBumper.Bot
    {
        float FRICTION = 0.97f;
        Color c = Color.White;

        protected override Color GetColor()
        {
            return c;
        }

        public Morpheus2()
        {

        }

        bool move = false;
        bool pussy = true;
        protected override EnvBumper.Action GetAction()
        {
            Vector2 dir = -Velocity;//new Vector2(Pos.Y,-Pos.X);
            if (Pos.Length() < 5)
            {
                if (!pussy)
                    dir += enemies[0].Pos - Pos * (10.0f / Pos.Length());
                else
                {
                    bool x = EnvBumper.constRand.NextDouble() < 0.5;
                    Vector2 vec = enemies[0].Pos - Pos;
                    Vector2 orth = new Vector2((x ? 1 : -1) * vec.Y, (x ? -1 : 1) * vec.X);
                    dir += orth * (10.0f / Pos.Length());
                }
            }
            else
            {
                dir += -Pos;
            }


            if (EnvBumper.constRand.NextDouble() < 0.01)
                move = !move;
            if (EnvBumper.constRand.NextDouble() < 0.005)
            {
                pussy = !pussy;
                c = pussy ? Color.Pink : Color.White; 
            }

            if ((Pos - enemies[0].Pos).Length() < 3.5f || Pos.Length() > 6.666f)
                move = true;
            


            return new EnvBumper.Action()
            {
                angle = dir != Vector2.Zero ? (float)Math.Atan2(dir.Y, dir.X) : 0f,
                accelerate = move//dir != Vector2.Zero
            };
        }

    }
}
