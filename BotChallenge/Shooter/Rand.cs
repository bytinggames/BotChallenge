using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.Shooter;
using Microsoft.Xna.Framework;

namespace Shooter
{
    class Rand : EnvShooter.Bot
    {
        bool l, r, u, d;

        protected override EnvShooter.Action GetAction()
        {
            int range = 30;

            if (rand.Next(range) == 0)
                l = !l;
            if (rand.Next(range) == 0)
                r = !r;
            if (rand.Next(range) == 0)
                d = !d;
            if (rand.Next(range) == 0)
                u = !u;

            return new EnvShooter.Action()
            {
                aim = (float)rand.NextDouble() * MathHelper.TwoPi,
                charge = rand.Next(100) > 0,
                right = r,
                left = l,
                up = u,
                down = d
            };
        }

        protected override Color GetColor()
        {
            return Color.Black;
        }
    }
}
