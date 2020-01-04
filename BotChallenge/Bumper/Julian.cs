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
    class Julian : EnvBumper.Bot
    {
        protected override Color GetColor()
        {
            return Color.Gray;
        }

        public Julian()
        {

        }

        protected override EnvBumper.Action GetAction()
        {
            //Pos
            //Velocity
            //enemies[0].Pos
            //enemies[0].Velocity

            return new EnvBumper.Action()
            {
                angle = MathHelper.Pi,
                accelerate = true
            };
        }

    }
}
