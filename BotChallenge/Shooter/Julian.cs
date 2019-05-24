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
    class Julian : EnvShooter.Bot
    {
        protected override Color GetColor()
        {
            return Color.Gray;
        }

        public Julian()
        {
            //Start
        }

        protected override EnvShooter.Action GetAction()
        {
            //Loop
            
            return new EnvShooter.Action()
            {
                right = false,
                up = false,
                left = false,
                down = false,
                charge = false,
                aim = MathHelper.Pi
            };
        }

    }
}
