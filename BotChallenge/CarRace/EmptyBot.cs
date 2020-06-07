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
        protected override void InitializeCustom()
        {
            // initialization code - run once per start of a race
        }

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
            //this.currentGoalV f.ex.

            return action;
        }


        protected override Color GetColor()
        {
            return Color.White; // determine your beautiful look
        }

        protected override void DrawCustom()
        {
            DrawM.Vertex.DrawCircleOutline(positionV, 10f, Color.White * 0.5f, 8f);
            DrawM.Vertex.DrawLineThin(positionV, this.currentGoalV, Color.White * 0.5f);
            // DrawM.Vertex....
        }
    }
}
