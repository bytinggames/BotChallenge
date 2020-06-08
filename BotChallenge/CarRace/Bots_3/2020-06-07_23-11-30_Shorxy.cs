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
    class Shorxy_3 : Bot
    {

        static int TRIGGER_DISTANCE_TO_GOAL = 10;

        Vector2 currentGoal;
        Vector2? nextGoal;
        Vector2? lastGoal;
        Vector2 currentPosition;
        Vector2 meToGoal;
        Vector2? meToNextGoal;
        Vector2? meToLastGoal;
        Vector2? currentGoalToNextGoal;
        Vector2 lineOfView;
        Vector2 lineOfViewOrthogonalRight;
        Vector2 lineOfViewOrthogonalLeft;

        float angleCurrentDirectionToCurrentGoal;
        float? angleCurrentGoalToNextGoal;

        float distanceToCurrentGoal;
        float? distanceToNextGoal;
        float? distanceToLastGoal;

        float directionToCurrentGoal;
        float? directionToNextGoal;

        bool currentGoalInCurrentDirection;
        bool? nextGoalInCurrentDirection;

        protected override void InitializeCustom()
        {
            // initialization code - run once per start of a race
        }

        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            currentGoal = this.currentGoalV;
            currentPosition = this.positionV;
            meToGoal = currentGoal - currentPosition;
            meToGoal.Normalize();

            if (this.goalIndex < this.goals.Count - 1)
            {
                nextGoal = this.goals[goalIndex + 1];
            }
            else
            {
                nextGoal = null;
            }

            if(nextGoal != null) {
                meToNextGoal = nextGoal.Value - currentPosition;
                meToNextGoal = Vector2.Normalize(meToNextGoal.Value);

                currentGoalToNextGoal = nextGoal - currentGoal;
                currentGoalToNextGoal = Vector2.Normalize(currentGoalToNextGoal.Value);
            }
            else {
                meToNextGoal = null;
            }

            if(this.goalIndex > 0) {
                lastGoal = this.goals[goalIndex - 1];
            }
            else {
                lastGoal = null;
            }

            if(lastGoal != null) {
                meToLastGoal = lastGoal - currentPosition;
                meToLastGoal = Vector2.Normalize(meToLastGoal.Value);
            }
            else {
                meToLastGoal = null;
            }

            lineOfView = new Vector2 {
                X = this.directionRightV.Y,
                Y = -this.directionRightV.X
            };
            lineOfView.Normalize();

            lineOfViewOrthogonalRight = directionRightV;
            lineOfViewOrthogonalRight.Normalize();

            lineOfViewOrthogonalLeft = Vector2.Negate(lineOfViewOrthogonalRight);
            lineOfViewOrthogonalLeft.Normalize();
           

            distanceToCurrentGoal = Vector2.Distance(currentGoal, currentPosition);

            directionToCurrentGoal = Vector2.Dot(directionRightV, meToGoal);

            if(nextGoal != null) {
                directionToNextGoal = Vector2.Dot(directionRightV, meToNextGoal.Value);

                angleCurrentGoalToNextGoal = (Vector2.Dot(meToGoal, currentGoalToNextGoal.Value) / (meToGoal.Length() * currentGoalToNextGoal.Value.Length()));


                //Console.WriteLine(directionToCurrentGoal + " " + directionToNextGoal + " " + angleCurrentGoalToNextGoal + "\n");
                
                nextGoalInCurrentDirection = angleCurrentGoalToNextGoal > .75;
            }


            angleCurrentDirectionToCurrentGoal = (Vector2.Dot(meToGoal, lineOfView) / (meToGoal.Length() * lineOfView.Length()));

            currentGoalInCurrentDirection = angleCurrentDirectionToCurrentGoal > .75;


            if(distanceToLastGoal == null) {
                distanceToLastGoal = -100000;
            }

            if (directionToCurrentGoal > 0)
            {
                if ((distanceToCurrentGoal < TRIGGER_DISTANCE_TO_GOAL && !nextGoalInCurrentDirection.Value) || (distanceToLastGoal.Value < TRIGGER_DISTANCE_TO_GOAL && !currentGoalInCurrentDirection))
                {
                    action.accelerate = .1f;
                    action.brake = 1f;
                }
                else
                {
                    action.accelerate = 1f;
                    action.brake = 0f;
                }
            }
            else
            {
                action.accelerate = .2f;
            }


            float toGoalDir = Vector2.Dot(directionRightV, meToGoal);

            if (distanceToCurrentGoal > TRIGGER_DISTANCE_TO_GOAL)
            {
                if(toGoalDir < 0) {
                    action.steer = -1f;
                }
                else {
                    action.steer = 1f;
                }
                
            }
            else
            {

                if (nextGoal != null)
                {
                    float toNextGoal = Vector2.Dot((Vector2)meToNextGoal, directionRightV);
                    if(toNextGoal < 0) {
                        action.steer = -.5f;
                    }
                    else {
                        action.steer = .5f;
                    }
                }
                
            }
            
            return action;
        }

        private enum DrivingState
        {
            DRIVE, BRAKE, STEER
        }

        private enum PositionState
        {
            CLOSE_TO_GOAL, CLOSE_FROM_GOAL, HIGHWAY, CLOSE_TO_FINISH
        }



        protected override Color GetColor()
        {
            return Color.DeepPink; // determine your beautiful look
        }

        protected override void DrawCustom()
        {
            DrawM.Vertex.DrawCircleOutline(positionV, 10f, Color.White * 0.5f, 8f);
            DrawM.Vertex.DrawLineThin(positionV, this.currentGoalV, Color.White * 0.5f);
        }
    }
}
