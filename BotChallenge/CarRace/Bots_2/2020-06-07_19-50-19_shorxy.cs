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
    class shorxy_2 : Bot
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
            //action.accelerate = rand.NextFloat();
            //action.steer = 1f;
            //action.brake = (float)Math.Round(0.2f); // use Math. for math calculations

            // type this. to see what you have access to
            //this.<>
            //this.currentGoalV f.ex.

            int nextGoalIndex = this.goalIndex + 1;

            Vector2? nextGoal;
            if (this.goalIndex < this.goals.Count - 1)
            {
                nextGoal = this.goals[nextGoalIndex];
            }
            else
            {
                nextGoal = null;
            }

            Vector2 currGoal = this.currentGoalV;
            Vector2 currPos = this.positionV;
            Vector2 vectorToGoal = currGoal - currPos;
            vectorToGoal.Normalize();

            Vector2 lineOfView = new Vector2
            {
                X = this.directionRightV.Y,
                Y = -this.directionRightV.X
            };

            lineOfView.Normalize();

            Vector2? currentGoalToNextGoal = nextGoal - currGoal;


            Vector2 vectorToGoalOrthogonal = new Vector2
            {
                X = -vectorToGoal.Y,
                Y = vectorToGoal.X
            };

            

            float distanceToCurrGoal = Vector2.Distance(currGoal, currPos);

            float toGoal = Vector2.Dot(lineOfView, vectorToGoal);

            if (toGoal > 0)
            {
                if (distanceToCurrGoal > 20)
                {
                    action.accelerate = 1f;
                    action.brake = 0f;
                }
                else
                {
                    action.accelerate = .7f;
                    action.brake = 1f;
                }
            }
            else
            {
                action.accelerate = .1f;
            }


            float toGoalDir = Vector2.Dot(directionRightV, vectorToGoal);

            //Console.WriteLine(toGoalDir);


            if (distanceToCurrGoal > 10)
            {
                action.steer = toGoalDir;
            }
            else
            {

                if (nextGoal != null)
                {
                    Vector2 meToNextGoal = currPos - (Vector2)nextGoal;
                    meToNextGoal.Normalize();
                    bool nextGoalRight = true;

                    float toNextGoal = Vector2.Dot((Vector2)meToNextGoal, directionRightV);
                    action.steer = -toNextGoal;
                }
                
            }
            



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
        }
    }
}
