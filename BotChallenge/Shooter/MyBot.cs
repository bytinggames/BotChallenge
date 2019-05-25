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
    class MyBot : EnvShooter.Bot
    {

        EnvShooter.Action lastAction;


        protected override Color GetColor()
        {
            return Color.Gray;
        }

        public MyBot()
        {
            //Start
        }

        protected override EnvShooter.Action GetAction()
        {
            //Loop
            
            //env.Bullets  - bullet array
            //this. eigene variablen (this ist nicht benötigt)

            lastAction = new EnvShooter.Action()
            {
                right = false,
                up = false,
                left = false,
                down = false,
                charge = false,
                aim = MathHelper.Pi
            };
            
            

            return lastAction;
        }

    }
}
