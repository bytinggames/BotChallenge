using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.CarRace;
using JuliHelper;
using Microsoft.Xna.Framework;

namespace CarRace
{
    class EmptyBot : Bot
    {
        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            // insanely awesome bot code goes here

            // set action.<> values to steer your bot
            action.accelerate = rand.NextFloat();
            action.steer = 1f;
            action.brake = (float)Math.Round(0.2f); // use Math. for math calculations
            
            // type this. to see what you have access to
            //this.<>

            return action;
        }


        protected override Color GetColor()
        {
            return Color.White; // determine your beautiful look
        }
    }
}
