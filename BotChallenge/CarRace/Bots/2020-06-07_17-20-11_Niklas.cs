using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.CarRace;
using JuliHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CarRace
{
    class Niklas : Bot
    {
        private enum CarState
        {
            Started,
            Collecting,
            Switching_Goal,
        }

        private CarState carState;
        private Vector2 total_dist_goal;
        private int goal_last_frame;

        protected override void InitializeCustom()
        {
            base.InitializeCustom();
            carState = CarState.Started;
            total_dist_goal = this.currentGoalV - this.positionV;
            goal_last_frame = 0;
        }

        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            action.accelerate = 1f;

            switch(carState)
            {
                case CarState.Started:
                    Start_Maneuver(ref action);
                    break;
                case CarState.Collecting:
                    Collect(ref action);
                    break;
            }

            return action;
        }


        protected override Color GetColor()
        {
            return Color.Gray; // determine your beautiful look
        }

        protected float Get_Direction(Vector2 target)
        {
            Vector2 offset = Vector2.Normalize(target - this.positionV);
            return Vector2.Dot(offset, this.directionRightV);
        }

        protected void Start_Maneuver(ref EnvCarRace.Action action)
        {
            float direction = Get_Direction(this.currentGoalV);

            if(direction < -0.01)
            {
                action.accelerate = 0.8f;
                action.steer = -1f;
            }
            else if(direction > 0.01)
            {
                action.accelerate = 0.8f;
                action.steer = 1f;
            }
            else
            {
                //this.carState = CarState.Collecting;
            }
        }

        protected void Collect(ref EnvCarRace.Action action)
        {
            int nextgoal = this.goalIndex + 1;
            if (nextgoal >= this.goals.Count)
                nextgoal = this.goals.Count - 1;
            float next_direction = Get_Direction(this.goals[nextgoal]);
            float direction = Get_Direction(this.currentGoalV);

            if (this.goal_last_frame != this.goalIndex)
            {
                this.goal_last_frame++;
                this.total_dist_goal = this.currentGoalV - this.positionV;
            }

            action.steer = Get_Steering(direction, next_direction);

            
            if ((this.currentGoalV - this.positionV).Length() < (this.total_dist_goal.Length() / 2))
            {
                if (next_direction < -0.01)
                    action.steer = -1f;
                else if (next_direction > 0.01)
                    action.steer = 1f;
            }
        }

        protected float Get_Steering(float direction, float next_direction)
        {
            float threshold;

            if (next_direction < 0)
                threshold = -0.1f;
            else if (next_direction > 0)
                threshold = 0.1f;
            else
                threshold = 0;

            if (direction < (threshold - 0.01))
                return -0.3f;
            else if (direction > (threshold - 0.01))
                return 0.3f;
            else
                return 0;
        }
    }
}
