using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.CarRace;
using JuliHelper;
using Microsoft.Xna.Framework;

namespace CarRace
{
    class SneakyBot : Bot
    {
        // config attributes
        private const double targetAngle = MathHelper.Pi;
        private double borderAngle = 0.5 * MathHelper.Pi; // bot aims to achieve targetAngle in the specified borders

        /// <summary>
        /// get angle between 3 points, where B is the pivot
        /// </summary>
        protected double getAngle(Vector2 pointA, Vector2 pointB, Vector2 pointC)
        {
            Vector2 vectorBA = Vector2.Subtract(pointA, pointB);
            Vector2 vectorBC = Vector2.Subtract(pointC, pointB);
            if (vectorBC.Length() == 0f)
                return MathHelper.Pi;
            return Vector2.Dot(Vector2.Normalize(vectorBA), Vector2.Normalize(vectorBC));
            //return Math.Atan2(vectorBC.Y - vectorBA.Y, vectorBC.X - vectorBA.X);
        }


        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            // goal after current goal:
            Vector2 nextGoalV = (this.goalIndex + 1 < this.goals.Count) ? this.goals[this.goalIndex + 1] : this.currentGoalV;

            // set action.<> values to steer your bot
            action.accelerate = 1f; // immer vollgas, keine rücksicht auf verluste
            // action.steer = 1f;
            // action.brake = (float)Math.Round(0.2f); // use Math. for math calculations

            // maybe add a steer penalty somewhere

            /*
            Vector2 scalarV = Vector2.Multiply(this.directionRightV, Vector2.Subtract(this.currentGoalV, this.positionV));
            float scalar = scalarV.X + scalarV.Y;
            if (scalar > 0)
            {
                action.steer = 1f;
            }
            else if (scalar < 0)
            {
                action.steer = -1f;
            }*/

            // secret approach: randomly try to get a nice curve, else straight at point
            if(this.rand.Next() % 10 < 4 && this.goalIndex != 0)
            {
                float steerPenalty = 0.5f;

                double currentAngle = getAngle(this.positionV, this.currentGoalV, nextGoalV);
                // angle in deg = currentAngle * 180 / MathHelper.Pi
                if (currentAngle < targetAngle - borderAngle)
                {
                    // steer away from current goal
                    action.steer = -steerPenalty;
                }
                else if (currentAngle < targetAngle)
                {
                    // steer towards current goal
                    action.steer = steerPenalty;
                }
                else if (currentAngle < targetAngle + borderAngle)
                {
                    // steer away from current goal (other direciton)
                    action.steer = steerPenalty;
                }
                else
                {
                    // steer towards current goal (other direciton)
                    action.steer = -steerPenalty;
                }
            } else
            {
                Vector2 scalarV = Vector2.Multiply(this.directionRightV, Vector2.Subtract(this.currentGoalV, this.positionV));
                float scalar = scalarV.X + scalarV.Y;
                if (scalar > 0)
                {
                    action.steer = 1f;
                }
                else if (scalar < 0)
                {
                    action.steer = -1f;
                }
            }

            

            // type this. to see what you have access to
            //this.<>

            return action;
        }


        protected override Color GetColor()
        {
            return Color.Teal; // determine your beautiful look
        }
    }
}
