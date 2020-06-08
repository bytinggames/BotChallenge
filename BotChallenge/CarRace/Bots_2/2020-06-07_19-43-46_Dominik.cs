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
    class Dominik_2 : Bot
    {
        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            // create rotation matrix (0, 2pi) gegen uhrzeigersinn
            Vector2 rotateV(Vector2 vec, double alpha)
            {
                Vector2 vec_rot = new Vector2();
                vec_rot.X = (float)Math.Cos(alpha) * vec.X + (float)Math.Sin(alpha) * vec.Y;
                vec_rot.Y = -(float)Math.Sin(alpha) * vec.X + (float)Math.Cos(alpha) * vec.Y;
                return vec_rot;
            }

            Vector2 goal_orientation = this.currentGoalV - this.positionV;
            Vector2 orientation_rot_left = rotateV(this.orientationV, 0.5f * Math.PI);
            Vector2 orientation_rot_right = rotateV(this.orientationV, 1.5f * Math.PI);
            float orient_angle = (float)Math.Acos(Vector2.Dot(this.orientationV, goal_orientation) / (this.orientationV.Length() * goal_orientation.Length()));
            float angle_left = (float)Math.Acos(Vector2.Dot(orientation_rot_left, goal_orientation) / (orientation_rot_left.Length() * goal_orientation.Length()));
            float angle_right = (float)Math.Acos(Vector2.Dot(orientation_rot_right, goal_orientation) / (orientation_rot_left.Length() * goal_orientation.Length()));

            bool next_target_bad_angle;
            
            if (this.goalIndex < this.goals.Count-1)
            {
                Vector2 next_goal_orientation = this.goals[this.goalIndex + 1] - this.positionV;
                
                float next_orient_angle = (float)Math.Acos(Vector2.Dot(this.orientationV, next_goal_orientation) / (this.orientationV.Length() * next_goal_orientation.Length()));

                float next_goal_brake_th = 2*MathHelper.Pi / 4; // angle
                next_target_bad_angle = next_orient_angle > next_goal_brake_th;

                // target the next goal if next to current goal
                float target_change_th = 5.0f;
                if (goal_orientation.Length() < target_change_th)
                {
                    float next_angle_left = (float)Math.Acos(Vector2.Dot(orientation_rot_left, next_goal_orientation) / (orientation_rot_left.Length() * next_goal_orientation.Length()));
                    float next_angle_right = (float)Math.Acos(Vector2.Dot(orientation_rot_right, next_goal_orientation) / (orientation_rot_left.Length() * next_goal_orientation.Length()));

                    angle_left = next_angle_left;
                    angle_right = next_angle_right;
                }
            }
            else
            {
                next_target_bad_angle = false;
            }

            
            

            // insanely awesome bot code goes here
            float max_acc = 1.0f;  // maximum acceleration
            float brake_threshold = 30.0f;  // 30 is a reasonable value for the distance to goal

            action.brake = 0.0f; // -> normally dont brake


            // break if near goal and next goal is not directly assessed
            
            if (this.goalIndex != this.goals.Count-1)
            {
                if (goal_orientation.Length() < brake_threshold && next_target_bad_angle) // brake if near to goal
                {
                    action.accelerate = 0.0f;
                    action.brake = 1.0f;
                } else
                {
                    action.accelerate = max_acc; // in [0, 1]
                }
            } else
            {
                action.accelerate = max_acc; // in [0, 1]
            }


            
            // best steering
            float angle_brake_th = 4 * MathHelper.Pi / 8;
            float acc_cap = 0.0f;
            float steering_brake = 1.0f;
            if (angle_left < angle_right) // steer left
            {
                action.steer = -1.0f;
                if (orient_angle > angle_brake_th)
                {
                    action.accelerate = 0.0f;
                    action.brake = steering_brake;
                }
            }
            else if (angle_left > angle_right) // steer right
            {
                action.steer = 1.0f;
                if (orient_angle > angle_brake_th)
                {
                    action.accelerate = 0.0f;
                    action.brake = steering_brake;
                }
            }
            else // driving away or towards goal
            {
                if (Vector2.Dot(this.orientationV, this.currentGoalV - this.positionV) > 0) // driving towards the goal
                {
                    //Console.WriteLine("driving towards the goal");
                    action.steer = 0.0f;
                }
                else  // driving away from the goal
                {
                    action.steer = 1.0f;  // choose either direction
                }
            }
            

            // correct if too slow
            float min_speed_fast = 22.5f; // 
            float min_speed_slow = 10.0f; // if orient_angle is larger than pi/2
            if (orient_angle > MathHelper.Pi/2 && this.velocityV.Length() < min_speed_slow)
            {
                action.accelerate = 1.0f;
                action.brake = 0.0f;
            } else if (orient_angle <= MathHelper.Pi/2 && this.velocityV.Length() < min_speed_fast)
            {
                action.accelerate = 1.0f;
                action.brake = 0.0f;
            }

            return action;
        }


        protected override Color GetColor()
        {
            return Color.Purple; // determine your beautiful look
        }
    }
}
