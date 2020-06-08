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
    class SchleuderNator : Bot
    {
        private float steeringfactor = 0.1f;
        private List<Vector2> lastpos = new List<Vector2>();
        private List<EnvCarRace.Action> lastactions = new List<EnvCarRace.Action>();

        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            if (lastpos.Count < 1)
            {
                action.accelerate = 1f;
                steeringfactor = (float)new Random().NextDouble() * 2 - 1;
                return action;
            }
            lastpos.Add(this.positionV);

            if (distance(lastpos[0].X, lastpos[0].Y, currentGoalV.X, currentGoalV.Y) < distance(positionX, positionY, currentGoalV.X, currentGoalV.Y))
            {
                action = lastactions[0];
            } else
            {
                action = lastactions[0];
                action.accelerate = action.steer * -1;
            }
            action.accelerate = 1f;

            // insanely awesome bot code goes here
            Random random = new Random();
            random.Next(-1, 1);


            // set action.<> values to steer your bot
            action.accelerate = 1f;
            
            //action.steer = steerLeft() ? -steeringfactor : steeringfactor;
            action.steer = steeringfactor;

            action.brake = (float)Math.Round(0.2f); // use Math. for math calculations

            // type this. to see what you have access to
            //this.<>
            //this.currentGoalV f.ex.

            humanIntervene(action);
            lastpos.Clear();
            lastactions.Add(action);
            return action;
        }

        private float distance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        private bool steerLeft()
        {
            double delta_x = this.positionX - this.currentGoalV.X;
            double delta_y = this.positionY - this.currentGoalV.Y;
            double theta_radians = Math.Atan2(delta_y, delta_x);
            Console.WriteLine("carrad: " + this.orientation + "togoalrad: " + theta_radians);
            return this.orientation < theta_radians;
        }

        private void humanIntervene(EnvCarRace.Action action)
        {
            if (Input.up.down)
                action.accelerate = 1f;
            if (Input.down.down)
                action.brake = 1f;
            if (Input.left.down)
                action.steer--;
            if (Input.right.down)
                action.steer++;
        }


        protected override Color GetColor()
        {
            return new Color((uint)new Random().Next(0,120)); // determine your beautiful look
        }
    }
}
