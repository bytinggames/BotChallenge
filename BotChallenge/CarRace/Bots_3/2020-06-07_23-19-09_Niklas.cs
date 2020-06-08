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
    class Niklas_3 : Bot
    {
        private float total_distance;

        protected override void InitializeCustom()
        {
            base.InitializeCustom();
            total_distance = (this.currentGoalV - this.positionV).Length();
        }

        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            float current_dist = (this.currentGoalV - this.positionV).Length();
            float direction = Get_Direction(this.currentGoalV);
            int next_dest = this.goalIndex + 1;
            if (next_dest >= this.goals.Count)
                next_dest = this.goalIndex;
            float next_direction = Get_Direction(this.goals[next_dest]);

            action.brake = 0f;

            if (direction < -0.01)
                action.steer = -1;
            else if (direction > 0.01)
                action.steer = 1;
            else
                action.steer = 0;

            action.accelerate = 1f;

            if ((Math.Abs(direction) - Math.Abs(next_direction)) > -0.9f)
            {
                if (this.velocityV.Length() > 40)
                {
                    if ((current_dist / currentGoalV.Length()) < (this.velocityV.Length() * 0.004))
                    {
                        if (next_direction < -0.01)
                            action.steer = -1;
                        else if (next_direction > 0.01)
                            action.steer = 1;
                        else
                            action.steer = 0;

                        action.accelerate = 0;
                    }
                }
            }
            else
            {
                action.brake = 1f;
            }

            return action;
        }

        protected override Color GetColor()
        {
            return Color.MidnightBlue; // determine your beautiful look
        }

        protected float Get_Direction(Vector2 target)
        {
            Vector2 offset = Vector2.Normalize(target - this.positionV);
            return Vector2.Dot(offset, this.directionRightV);
        }
    }
}
