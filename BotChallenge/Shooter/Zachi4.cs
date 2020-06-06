
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.Shooter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    class Zachi4 : EnvShooter.Bot
    {
        EnvShooter.Action lastAction;
        Color color = Color.DeepPink;

        float danger_dist = 5;
        float no_charge_safe_zone = 7;
        int frames_till_hit_max = (int)(12 / 0.5f);


        protected override Color GetColor()
        {
            return color;
        }

        public Zachi4()
        {
        }

        protected override EnvShooter.Action GetAction()
        {
            Vector2 move_target = new Vector2(this.Pos.X, 20);
            Vector2 aim_target = new Vector2(0, 0);


            if (env.Bullets.Length == 0)
                move_target.Y = this.Pos.Y;


            EnvShooter.Bot[] bots = this.enemies.ToList().FindAll(x => x.Alive).ToArray();
            aim_target = bots[rand.Next(bots.Length)].Pos;

            bool do_charge = false;//this.Id == 0;


            Bullet[] collectible_bullets = this.env.Bullets.ToList().FindAll(x => x.Collectible).ToArray();
            Bullet[] active_bullets = this.env.Bullets.ToList().FindAll(x => !x.Collectible).ToArray();
            Vector2[,] bullet_poses = BulletSim3.Get_Bullet_Poses(active_bullets, this.map, frames_till_hit_max);

            /*  MOVEMENT START  */
            MovementDecision md = new MovementDecision(bullet_poses);

            // Add the most desired position first and least desired one last

            // 1. Get out of danger Zone
            if (InMortalDanger(this.Pos, enemies[0].Pos))
            {
                Vector2 rel_vec = -(enemies[0].Pos - this.Pos);
                if (!IsPosInWall(this.Pos + rel_vec))
                    md.desired_poses.Add(TargetToNextFramePos(this.Pos + rel_vec));
            }

            // 2. Get to collectable
            List<Tuple<float, Vector2>> collectible_dists = collectible_bullets.Select(x => new Tuple<float, Vector2>(Vector2.Distance(x.Pos, this.Pos), x.Pos)).ToList();
            collectible_dists.OrderBy(x => x.Item1);
            for (int i = 0; i < collectible_dists.Count; i++)
                if (!InMortalDanger(collectible_dists[i].Item2, enemies[0].Pos))
                    md.desired_poses.Add(TargetToNextFramePos(collectible_dists[i].Item2));


            //if (!InMortalDanger(new Vector2(9, 9), enemies[0].Pos) &&
            //    !InNoChargeSafeZone(new Vector2(9, 9), enemies[0].Pos))
            //    md.desired_poses.Add(TargetToNextFramePos(new Vector2(9, 9)));

            // 3. Get to edge of no charge safe zone
            if (InSafety(this.Pos, enemies[0].Pos))
            {
                Vector2 rel_vec = (enemies[0].Pos - this.Pos);

                if (!IsPosInWall(this.Pos + rel_vec))
                    md.desired_poses.Add(TargetToNextFramePos(this.Pos + rel_vec));
            }
            else if (InNoChargeSafeZone(this.Pos, enemies[0].Pos))
            {
                Vector2 rel_vec = -(enemies[0].Pos - this.Pos);

                if (!IsPosInWall(this.Pos + rel_vec))
                    md.desired_poses.Add(TargetToNextFramePos(this.Pos + rel_vec));
            }



            // Add backup targets
            Vector2[] move_vecs = new Vector2[]
            {
                new Vector2(1,-1),new Vector2(1,0), new Vector2(1,1),
                new Vector2(0,1), new Vector2(-1,1),
                new Vector2(-1,0),new Vector2(-1,-1), new Vector2(0,-1)
            };
            for (int i = 0; i < move_vecs.Length; i++)
                md.desired_poses.Add(TargetToNextFramePos(this.Pos + move_vecs[i]));
            md.desired_poses.Add(Vector2.Zero);

            move_target = md.GetOptimalPos(this.Pos);

            /*  MOVEMENT END  */

            color = md.IsMovementTargetSafe == true ? Color.DeepPink : Color.Purple;

            /*  CHARGE START  */
            if (InMortalDanger(move_target, enemies[0].Pos) || md.IsMovementTargetSafe == false)
            {
                do_charge = false;
            }
            else
            {
                // Fully charged
                if (this.Charge >= 0.95)
                {
                    // Only shoot in no charge safety zone
                    if (InNoChargeSafeZone(this.Pos, enemies[0].Pos))
                        do_charge = false;
                    else
                        do_charge = true;
                }
                else
                {
                    do_charge = true;
                }
            }


            /*  CHARGE END  */

            //if (this.Id == 0)
            //    do_charge = false;

            EnvShooter.Action action = GoToXY(move_target);
            action.aim = AimToPos(aim_target);
            action.charge = do_charge;
            return action;
        }


        private Vector2 VecToPos(Vector2 pos)
        {
            Vector2 vec = pos - this.Pos;
            return vec;
        }

        private Vector2 TargetToNextFramePos(Vector2 target_pos, bool do_charge = true)
        {
            Vector2 vec = VecToPos(target_pos);
            Vector2 bin_vec = Vector2.Zero;
            // Binarize
            if (vec.X > 0) bin_vec.X = 1;
            else if (vec.X < 0) bin_vec.X = -1;

            if (vec.Y > 0) bin_vec.Y = 1;
            else if (vec.Y < 0) bin_vec.Y = -1;

            if (bin_vec.X != 0 || bin_vec.Y != 0)
            {
                bin_vec.Normalize();
                bin_vec *= EnvShooter.Bot.SPEED * 2.0f * (do_charge ? 0.5f : 1.0f);
            }

            return bin_vec;
        }

        private EnvShooter.Action GoToXY(Vector2 pos)
        {
            Vector2 vec = VecToPos(pos);

            return new EnvShooter.Action()
            {
                right = vec.X > 0,
                up = vec.Y < 0,
                left = vec.X < 0,
                down = vec.Y > 0,
                charge = false,   // 60 frames 
                aim = MathHelper.Pi
            };
        }

        private float AimToPos(Vector2 pos)
        {
            return (float)Math.Atan2(pos.Y - this.Pos.Y, pos.X - this.Pos.X);
        }

        private bool IsPosInWall(Vector2 pos)
        {
            return pos.X - 0.4f < 1 || pos.X + 0.4f > 19 || pos.Y - 0.4f < 1 || pos.Y + 0.4f > 19;
        }

        private bool InMortalDanger(Vector2 pos, Vector2 bot)
        {
            float dist = Vector2.Distance(pos, bot);
            return dist <= danger_dist;
        }

        private bool InNoChargeSafeZone(Vector2 pos, Vector2 bot)
        {
            float dist = Vector2.Distance(pos, bot);
            return dist > danger_dist && dist <= no_charge_safe_zone;
        }

        private bool InSafety(Vector2 pos, Vector2 bot)
        {
            float dist = Vector2.Distance(pos, bot);
            return dist > no_charge_safe_zone;
        }

        class MovementDecision
        {
            public List<Vector2> desired_poses;
            public Vector2[,] bullet_poses;
            public bool? IsMovementTargetSafe = null;

            public MovementDecision(Vector2[,] bullet_poses)
            {
                this.desired_poses = new List<Vector2>();
                this.bullet_poses = bullet_poses;
            }

            public MovementDecision(Vector2[,] bullet_poses, Vector2[] desired_poses)
            {
                this.desired_poses = desired_poses.ToList();
                this.bullet_poses = bullet_poses;
            }


            public Vector2 GetOptimalPos(Vector2 pos)
            {
                Vector2 point_out = Vector2.Zero;

                for (int i = 1; i < 10; i++)
                {
                    Vector2[] poses = new Vector2[desired_poses.Count];
                    for (int j = 0; j < desired_poses.Count; j++)
                        poses[j] = pos + (desired_poses[j] * (float)i);

                    int[] col_steps = new int[desired_poses.Count];
                    for (int j = 0; j < col_steps.Length; j++)
                    {
                        // Border check
                        if (poses[j].X - 0.4f < 1 || poses[j].X + 0.4f > 19 || poses[j].Y - 0.4f < 1 || poses[j].Y + 0.4f > 19)
                        {
                            col_steps[j] = 4;
                            continue;
                        }

                        // Bullet collision check
                        int min_bullet = BulletSim3.CollidesWith(poses[j], 0.6f, bullet_poses);

                        // No collisions
                        if (min_bullet == -1)
                        {
                            col_steps[j] = -1;
                            break;
                        }
                        else
                            col_steps[j] = min_bullet;
                    }

                    int max_dist = -1;
                    int max_dist_idx = -1;
                    bool minus_one_found = false;
                    for (int j = 0; j < col_steps.Length; j++)
                    {
                        if (col_steps[j] == -1)
                        {
                            point_out = poses[j];
                            minus_one_found = true;
                            break;
                        }
                        else if (col_steps[j] > max_dist)
                        {
                            max_dist = col_steps[j];
                            max_dist_idx = j;
                        }
                    }

                    if (minus_one_found == false)
                    {
                        point_out = poses[max_dist_idx];
                        this.IsMovementTargetSafe = false;
                    }
                    else
                    {
                        this.IsMovementTargetSafe = true;
                        break;
                    }
                }

                return point_out;
            }
        }

    }


    static class BulletSim3
    {

        public static Vector2[,] Get_Bullet_Poses(Bullet[] bullets, bool[,] map, int steps = 120)
        {
            EnvShooter env = create_env();
            Bullet[] n_bullets = new Bullet[bullets.Length];
            Vector2[,] bullet_positions = new Vector2[steps, n_bullets.Length];

            for (int i = 0; i < bullets.Length; i++)
                n_bullets[i] = new Bullet(bullets[i].Pos, bullets[i].Velocity, 2982397 + i, bullets[i].Color, env);

            SetValue(env, "map", map);

            for (int j = 0; j < n_bullets.Length; j++)
                for (int i = 0; i < steps; i++)
                {
                    call(n_bullets[j], "Move");

                    bullet_positions[i, j] = new Vector2(n_bullets[j].Pos.X, n_bullets[j].Pos.Y);

                    Vector2 velocity = n_bullets[j].Velocity * 0.99f;
                    SetValue(n_bullets[j], "velocity", velocity);

                    if (velocity != Vector2.Zero)
                    {
                        if (velocity.Length() < 0.1f)
                        {
                            // NOTHING HIT!
                            for (int y = i; y < steps; y++)
                                bullet_positions[y, j] = new Vector2(n_bullets[j].Pos.X, n_bullets[j].Pos.Y);
                            break;
                        }

                        if (velocity.Length() < 0.01f)
                            velocity = Vector2.Zero;
                    }

                }
            return bullet_positions;

        }

        public static int CollidesWith(Vector2 pos, float radius, Vector2[,] bullet_poses)
        {
            for (int i = 0; i < bullet_poses.GetLength(0); i++)       // Steps
                for (int j = 0; j < bullet_poses.GetLength(1); j++)     // Bullets
                {
                    if (bullet_poses[i, j].X + 0.2f + 0.1f >= pos.X - radius && bullet_poses[i, j].X - (0.2f + 0.1f) <= pos.X + radius &&
                        bullet_poses[i, j].Y + 0.2f + 0.1f >= pos.Y - radius && bullet_poses[i, j].Y - (0.2f + 0.1f) <= pos.Y + radius)
                        return i;
                }
            return -1;
        }

        public static int CollidesWith(Vector2 pos, float radius, Bullet b, bool[,] map, int steps = 60)
        {
            EnvShooter env = create_env();
            Bullet n_bullet = new Bullet(b.Pos, b.Velocity, 2982397, b.Color, env);

            SetValue(env, "map", map);


            for (int i = 0; i < steps; i++)
            {
                call(n_bullet, "Move");

                // Check collision
                if (n_bullet.Pos.X + 0.2f + 0.1f >= pos.X - radius && n_bullet.Pos.X - (0.2f + 0.1f) <= pos.X + radius &&
                    n_bullet.Pos.Y + 0.2f + 0.1f >= pos.Y - radius && n_bullet.Pos.Y - (0.2f + 0.1f) <= pos.Y + radius)
                    return i;

                Vector2 velocity = n_bullet.Velocity * 0.99f;
                SetValue(n_bullet, "velocity", velocity);

                if (velocity != Vector2.Zero)
                {
                    if (velocity.Length() < 0.1f)
                    {
                        // NOTHING HIT!
                        break;
                    }

                    if (velocity.Length() < 0.01f)
                        velocity = Vector2.Zero;
                }

            }
            return -1;
        }

        private static EnvShooter create_env()
        {
            return new EnvShooter(new Type[0]);
        }

        public static object call(this object o, string methodName, params object[] args)
        {
            var mi = o.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mi != null)
            {
                return mi.Invoke(o, args);
            }
            return null;
        }

        /*
        public static void SetValue(object inputObject, string propertyName, object propertyVal)
        {
            //find out the type 
            Type type = inputObject.GetType();

            //get the property information based on the type
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);

            //find the property type
            Type propertyType = propertyInfo.PropertyType;

            //Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyVal = Convert.ChangeType(propertyVal, targetType);

            //Set the value of the property
            propertyInfo.SetValue(inputObject, propertyVal, null);

        }
        */
        public static void SetValue(object s, string name, object value)
        {
            var prop = s.GetType().GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prop.SetValue(s, value);

        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }


}