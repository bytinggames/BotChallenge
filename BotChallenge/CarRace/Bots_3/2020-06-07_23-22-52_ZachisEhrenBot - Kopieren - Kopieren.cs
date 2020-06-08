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
    public class ZachisEhrenBot_3_2 : Bot
    {
        class Ranges
        {
            public int value_idx;
            public float start;
            public float end;
            public int acition;
            public Ranges(int value_idx, float start,float end, int action)
            {
                this.value_idx = value_idx;
                this.start = start;
                this.end = end;
                this.acition = action;
            }
        }

        Ranges[] ranges;
        int range_count = 25;
        float[,] values;
        int action_count = 7;
        float range_range = 0.25f;
        public int SEED;

        bool init = false;
        bool end = false;

        private void initialize()
        {
            values = new float[,]
            {
                { 0f, -1f,2f,0 },      // steering_dist
                { 0f, -66f, 300f,0 },   // goal_vec.Length()
                { 0f, -2f,2f,0 },     // action.steer
                { 0f, 0f,1f,1 },      // is_drifting()
                { 0f, -10f, 60f,0 },    // this.velocityV.Length()
                { 0f, -0.5f,(float)Math.PI+0.5f, 0},      // next_goal_angle()
                { 0f, -2f, 2f,0},       // steering
                { 0f, 0f, 1f,1},       // acceleration
                { 0f, 0f, 1f,1},       // brake
            };


            // Create ranges
            //Random _rand = new Random();
            //if (Program.TEST_BOT >= 0)
            //{
            //    if (Program.TEST_BOT > 100)
            //    {
            //        if (Program.AVG_FRAMES / 100f < Program.BEST_BOT_AND_SEED[0])
            //        {
            //            Program.BEST_BOT_AND_SEED[1] = Program.TEST_BOT_SEED;
            //            Program.BEST_BOT_AND_SEED[0] = (int)(Program.AVG_FRAMES / 100f);
            //            Console.WriteLine("[BEST] AVG_FRAMES: " + (Program.AVG_FRAMES / 100f) + ";  SEED: " + Program.BEST_BOT_AND_SEED[1]);
            //        }
            //        else
            //        {
            //            Console.WriteLine("AVG_FRAMES: " + (Program.AVG_FRAMES / 100f) + ";  SEED: " + Program.TEST_BOT_SEED);
            //        }
            //        Program.AVG_FRAMES = 0;
            //        Program.TEST_BOT = -1;
            //    }
            //    else
            //    {
            //        Program.TEST_BOT += 1;
            //        SEED = Program.TEST_BOT_SEED;
            //    }
            //}
            //if (Program.TEST_BOT == -1)
            //{
            //    SEED = _rand.Next();
            //    //SEED = 1785535071; // 5
            //    //SEED = 683401413; // 25
            //}
            SEED = 114193617;
            Random rand = new Random(SEED);

            ranges = new Ranges[range_count];
            for (int i = 0; i < range_count; i++)
            {
                int value_idx = rand.Next(values.GetLength(0));
                float start = 0;
                float end = 0;
                int action = rand.Next(action_count);

                // Normal range
                if (values[value_idx,3] == 0)
                {
                    float value_range = (values[value_idx, 2] - values[value_idx, 1])*range_range;
                    start = values[value_idx,1] + ((float)rand.NextDouble() * value_range * 3);
                    end = start + value_range;
                }
                // only use bigger then or equal to
                else
                {
                    float value_range = (values[value_idx, 2] - values[value_idx, 1]) * range_range;
                    value_range -= values[value_idx, 1];
                    start = (value_range / 2.0f) + values[value_idx, 1];
                    end = (float)rand.NextDouble();
                }
                ranges[i] = new Ranges(value_idx,start,end,action);
            }
            init = true;
        }

        private EnvCarRace.Action check_actions(EnvCarRace.Action action, Ranges range)
        {
            if (values[range.value_idx, 3] == 0)
            {
                if (values[range.value_idx, 0] >= range.start && values[range.value_idx, 0] <= range.end)
                {
                    action = apply_action(action, range.acition);
                }
            }
            else
            {
                if ((range.end > 0.5f && range.start > values[range.value_idx, 0]) ||
                    (range.end <= 0.5 && range.start <= values[range.value_idx, 0]))
                {
                    action = apply_action(action, range.acition);
                }
            }
            return action;
        }

        private EnvCarRace.Action apply_action(EnvCarRace.Action action,int action_int)
        {
            // Steer
            if (action_int == 0) action.steer = -1;
            else if (action_int == 1) action.steer = 0;
            else if (action_int == 2) action.steer = 1;
            else if (action_int == 3) action.accelerate = 0;
            else if (action_int == 4) action.accelerate = 1;
            else if (action_int == 5) action.brake = 0;
            else if (action_int == 6) action.brake = 1;

            values[6, 0] = action.steer;
            values[7, 0] = action.accelerate;
            values[8, 0] = action.brake;

            return action;

        }

        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            if (!init)
                initialize();

                

            _color = Color.White;
            EnvCarRace.Action action;// = new EnvCarRace.Action();
            Vector2 goal_vec = new Vector2(this.currentGoalV.X - this.positionV.X, this.currentGoalV.Y - this.positionV.Y);
            float steering_dist = 0f;
            var raw = vec_to_steer();
            action = raw.Item1;
            steering_dist = raw.Item2;

            //action.accelerate = 1.0f;

            //if (end == false && this.goalIndex+1 >= this.goals.Count)
            //{
            //    if (env.Frame < Program.BOT_MAX_FRAMES && Program.TEST_BOT == -1)
            //    {
            //        Program.TEST_BOT_SEED = SEED;
            //        Program.TEST_BOT = 0;
            //    }
            //    else
            //    {
            //        Program.AVG_FRAMES += env.Frame;
            //    }
            //    end = true;
            //}

            //if (env.Frame+2 >= 120*60)
            //{
            //    Program.TEST_BOT = -1;
            //}


            // Update values
            values[0, 0] = steering_dist;
            values[1, 0] = goal_vec.Length();
            values[2, 0] = action.steer;
            values[3, 0] = is_drifting() ? 1.0f : 0.0f;
            values[4, 0] = this.velocityV.Length();
            values[5, 0] = next_goal_angle();
            values[6, 0] = action.steer;
            values[7, 0] = action.accelerate;
            values[8, 0] = action.brake;

            // Check ranges
            for (int i = 0; i < ranges.Length; i++)
            {
                action = check_actions(action, ranges[i]);
            }

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
            return Color.Green; // determine your beautiful look
        }
    }
}
