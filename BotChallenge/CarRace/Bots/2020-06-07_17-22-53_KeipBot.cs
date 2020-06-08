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
    class KeipBot : Bot
    {
        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            // insanely awesome bot code goes here


            Vector2 goalDirection = currentGoalV - positionV;
            goalDirection.Normalize();
            float forward = Vector2.Dot(goalDirection, orientationV);

            Vector2 goaldirectionTotal = currentGoalV - positionV;

            float goalposition = goaldirectionTotal.Length();


            
            if ((goalposition > 20 && forward > 0 || velocityV.Length() < 10))
                action.accelerate = 1;
            else
                action.brake = 1f;


            float toRight = Vector2.Dot(goalDirection, directionRightV);

            if (toRight < 0)
                action.steer--;
            else
                action.steer++;

            

        // set action.<> values to steer your bot
           // action.accelerate = rand.NextFloat();
           // action.steer = 0f;        // steer >0 right; steer <0 left
           // action.brake = (float)Math.Round(0.2f); // use Math. for math calculations
            
            // type this. to see what you have access to
            //this.<>
            
            //this.currentGoalV f.ex.

            return action;
        }


        protected override Color GetColor()
        {
            
            return Color.Gray; // determine your beautiful look
        }
    }
}
