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
    class Zachi : Bot
    {
        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action;// = new EnvCarRace.Action();
            float steering_dist = 0f;
            float speed = 50f;

            var raw = vec_to_steer();
            action = raw.Item1;
            steering_dist = raw.Item2;


            speed = 200f;
            action.steer *= 1f;

            if (is_drifting())
            {
                speed = this.velocityV.Length() < 10f ? speed : 0f;
                action.steer *= this.velocityV.Length() < 50f ? .5f : 0f;

            }

            if (steering_dist < 0.15f)
                action.steer *= 0.5f;
            else if (steering_dist < 0.05f)
                action.steer *= 0.1f;
            //else if (steering_dist < 0.01f)
            //    action.steer *= 0f;


            if (is_drifting())
                this._color = Color.Red;
            else
                this._color = Color.White;



            // Aciton
            if (this.velocityV.Length() < speed)
                action.accelerate = 1.0f;
            //else if (this.velocityV.Length() > speed)
              //  action.brake = 1; // use Math. for math calculations
            
            // type this. to see what you have access to
            //this.<>
            //this.currentGoalV f.ex.

            return action;
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

            float bot_dir = (float)Math.Atan2(this.velocityV.Y, this.velocityV.X);
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
            return Color.Gray; // determine your beautiful look
        }
    }
}
