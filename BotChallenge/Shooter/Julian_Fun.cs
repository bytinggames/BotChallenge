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
    class Julian_Fun : EnvShooter.Bot
    {
        EnvShooter.Action action = new EnvShooter.Action()
        {
            right = false,
            up = false,
            left = false,
            down = false,
            charge = false,
            aim = MathHelper.Pi
        };

        protected override Color GetColor()
        {
            return Color.Orange * 0.5f;
        }

        public Julian_Fun()
        {
            //Start
        }

        bool reverseDir = false;

        Vector2 lastPos;

        protected override EnvShooter.Action GetAction()
        {
            Vector2 move = lastPos - enemies[0].Pos;
            action.charge = enemies[0].Charge > 0;

            VelocityToAction(ref action, move);

            lastPos = enemies[0].Pos;


            Vector2 dist = enemies[0].Pos - Pos;

            action.aim = (float)Math.Atan2(dist.Y, dist.X);


            return action;
        }

        void VelocityToAction(ref EnvShooter.Action action, Vector2 velocity)
        {
            action.right = action.left = action.up = action.down = false;
            //if (Math.Abs(velocity.X) > RADIUS)
            {
                action.right = velocity.X > 0;
                action.left = velocity.X < 0;
            }
            //if (Math.Abs(velocity.Y) > RADIUS)
            {
                action.up = velocity.Y < 0;
                action.down = velocity.Y > 0;
            }
        }
    }
}
