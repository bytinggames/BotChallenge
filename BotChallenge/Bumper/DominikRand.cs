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
    class DominikRand : EnvBumper.Bot
    {
        protected override Color GetColor()
        {
            return Color.DarkMagenta;
        }

        public DominikRand()
        {

        }

        protected override EnvBumper.Action GetAction()
        {

            float angle_next = (float) Env.constRand.NextDouble() * MathHelper.TwoPi;
            bool accelerate_next = true;



            return new EnvBumper.Action()
            {
                angle = angle_next,
                accelerate = accelerate_next
            };
        }

    }
}
