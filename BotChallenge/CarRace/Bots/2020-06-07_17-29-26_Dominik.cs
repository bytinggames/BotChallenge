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
    class Dominik : Bot
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

            // insanely awesome bot code goes here
            float max_speed = 1.0f;  // speed=acceleration
            float brake_threshold = 10.0f;

            action.brake = 0.0f; // in [0, 1] -> normally dont brake

            // accelerate only if far away from the goal
            if (this.goalIndex != this.goals.Count)
            {
                if (goal_orientation.Length() < brake_threshold) // brake if near to goal
                {
                    action.accelerate = 0.0f;
                    action.brake = 1.0f;
                } else
                {
                    action.accelerate = max_speed; // in [0, 1]
                }
            } else
            {
                action.accelerate = max_speed; // in [0, 1]
            }
                





            /*
            Console.WriteLine(this.orientationV);
            Console.WriteLine(orientation_rot_left);
            Console.WriteLine(orientation_rot_right);
            Console.WriteLine();*/

            float angle_left = (float)Math.Acos(Vector2.Dot(orientation_rot_left, goal_orientation) / (orientation_rot_left.Length() * goal_orientation.Length()));  // norm ignored
            float angle_right = (float)Math.Acos(Vector2.Dot(orientation_rot_right, goal_orientation) / (orientation_rot_left.Length() * goal_orientation.Length()));  // norm ignored

            //Console.WriteLine(angle_left);
            //Console.WriteLine(angle_right);

            if (angle_left < angle_right)
            {
                action.steer = -1.0f;
            }
            else if (angle_left > angle_right)
            {
                action.steer = 1.0f;
            }
            else
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
            
            
            return action;
        }


        protected override Color GetColor()
        {
            return Color.Gray; // determine your beautiful look
        }
    }
}
