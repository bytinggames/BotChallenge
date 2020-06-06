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
    class Human : Bot
    {
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            if (Input.up.down)
                action.accelerate = 1f;
            if (Input.down.down)
                action.brake = 1f;
            if (Input.left.down)
                action.steer--;
            if (Input.right.down)
                action.steer++;

            return action;
        }

        protected override Color GetColor()
        {
            return Color.White;
        }
    }
}
