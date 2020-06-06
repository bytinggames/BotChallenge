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
    class HumanWASD : Bot
    {
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            if (Input.w.down)
                action.accelerate = 1f;
            if (Input.s.down)
                action.brake = 1f;
            if (Input.a.down)
                action.steer--;
            if (Input.d.down)
                action.steer++;
            
            return action;
        }

        protected override Color GetColor()
        {
            return Color.LightGray;
        }
    }
}
