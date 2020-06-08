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
    class ZachiSlow_2 : Bot
    {
        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            _color = Color.White;
            EnvCarRace.Action action;// = new EnvCarRace.Action();
            Vector2 goal_vec = new Vector2(this.currentGoalV.X - this.positionV.X, this.currentGoalV.Y - this.positionV.Y);

            
            float steering_dist = 0f;
            float speed = 50f;

            var raw = vec_to_steer();
            action = raw.Item1;
            steering_dist = raw.Item2;


            speed = 200f;
            action.steer *= 1f;

            if (is_drifting())
            {
                speed = this.velocityV.Length() < 25f ? speed : 0f;
                action.steer *= this.velocityV.Length() < 50f ? .75f : .33f;

                this._color = Color.Red;
            }
            else if (goal_vec.Length() < 19f)
            {
                if (this.velocityV.Length() > 10f && next_goal_angle() < 2.5f)
                {
                    speed = 0;
                    action.brake = 0;
                }
            }
            else
            {
                if (this.velocityV.Length() > 7f && steering_dist > 0.15f)
                {
                    speed = 0;
                    action.brake = 1;
                    this._color = Color.Green;
                }
            }

            if (steering_dist < 0.1f)
                action.steer *= 0.5f;
            else if (steering_dist < 0.05f)
                action.steer *= 0.1f;
            //else if (steering_dist < 0.01f)
            //    action.steer *= 0f;



            // Aciton
            if (this.velocityV.Length() < speed)
                action.accelerate = 0.95f;
            //else if (this.velocityV.Length() > speed)
              //  action.brake = 1; // use Math. for math calculations
            
            // type this. to see what you have access to
            //this.<>
            //this.currentGoalV f.ex.

            return action;
        }

        private float next_goal_angle()
        {
            if (this.goalIndex + 1 >= this.goals.Count)
                return float.MinValue;
            Vector2 goal_vec = new Vector2(this.currentGoalV.X - this.positionV.X, this.currentGoalV.Y - this.positionV.Y);
            Vector2 next_goal_vec = new Vector2(this.goals[this.goalIndex + 1].X - this.currentGoalV.X, this.goals[this.goalIndex + 1].Y - this.currentGoalV.Y);
            //Console.WriteLine(Vector2.Dot(goal_vec, next_goal_vec) / (goal_vec.Length() * next_goal_vec.Length()));
            return (float)Math.Acos(Vector2.Dot(goal_vec,next_goal_vec) / (goal_vec.Length() * next_goal_vec.Length()));
        }

        private bool is_drifting()
        {
            if (velocityV.Length() == 0)
                return false;

            float velocity = this.velocityV.Length();
            float velocityLong = Vector2.Dot(this.velocityV, orientationV); // velocity in direction of car heading
            Vector2 velocityLongV = velocityLong * orientationV;
            Vector2 velocityLatV = this.velocityV - velocityLongV;
            float velocityLat = velocityLatV.Length(); // velocity to the right of the car direction

            return !(Math.Abs(velocityLong) > Math.Abs(velocityLat) * 1.2f && Math.Abs(orientationAngularVelocity * velocity) < 300f);
        }

        private Tuple<EnvCarRace.Action,float> vec_to_steer()
        {
            float steer_const = 1.0f;

            Vector2 goal_vec = new Vector2(this.currentGoalV.X- this.positionV.X, this.currentGoalV.Y - this.positionV.Y);

            float bot_dir = (float)Math.Atan2(-this.directionRightV.X, this.directionRightV.Y);
            if (!is_drifting())
                bot_dir = (float)Math.Atan2(this.velocityV.Y, this.velocityV.X);

            float goal_dir_rel_to_bot = (float)Math.Atan2(goal_vec.Y, goal_vec.X);

            // Fix over 0 rads
            if (bot_dir < goal_dir_rel_to_bot)
            {
                float dist = Math.Abs(goal_dir_rel_to_bot - bot_dir);
                float dist_padded = Math.Abs((bot_dir + (float)(Math.PI*2)) - goal_dir_rel_to_bot);

                bot_dir = dist < dist_padded ? bot_dir : bot_dir + (float)(Math.PI * 2.0f);
            }
            else
            {
                float dist = Math.Abs( bot_dir- goal_dir_rel_to_bot);
                float dist_padded = Math.Abs((goal_dir_rel_to_bot + (float)(Math.PI * 2)) - bot_dir);

                goal_dir_rel_to_bot = dist < dist_padded ? goal_dir_rel_to_bot : goal_dir_rel_to_bot + (float)(Math.PI * 2.0f);
            }

            EnvCarRace.Action act = new EnvCarRace.Action();

            float dist_out = Math.Abs(goal_dir_rel_to_bot - bot_dir) / (float)(Math.PI*2.0f);

            // Right
            if (goal_dir_rel_to_bot - bot_dir > 0)
                act.steer = 1.0f;
            // Left
            else if (bot_dir - goal_dir_rel_to_bot > 0)
                act.steer = -1.0f;

            return new Tuple<EnvCarRace.Action, float>(act, dist_out);
        }

        private Color _color = Color.White;

        protected override Color GetColor()
        {
            return Color.White; // determine your beautiful look
        }
    }
}
