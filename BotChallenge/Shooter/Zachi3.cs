
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
    class Zachi3 : EnvShooter.Bot
    {

        EnvShooter.Action lastAction;
        Color color = Color.DeepPink * 0.5f;


        protected override Color GetColor()
        {
            return color;
        }

        public Zachi3()
        {
        }

        protected override EnvShooter.Action GetAction()
        {
            Vector2 move_target = new Vector2(rand.Next(19), rand.Next(19));

            Vector2 aim_target = new Vector2(0, 0);


            EnvShooter.Bot[] bots = this.enemies.ToList().FindAll(x => x.Alive).ToArray();
            aim_target = bots[rand.Next(bots.Length)].Pos;

            bool do_charge = true;

            if (Charge >= 0.99)
                do_charge = false;

            Bullet[] active_bullets = this.env.Bullets.ToList().FindAll(x => !x.Collectible).ToArray();
            Vector2[,] bullet_poses = BulletSim.Get_Bullet_Poses(active_bullets, this.map, 120);

            int player_collision = BulletSim.CollidesWith(this.Pos, 0.4f, bullet_poses);
            int min_col = player_collision > -1 ? player_collision : int.MaxValue;

            if (min_col != int.MaxValue)
            {
                Vector2[] move_vecs = new Vector2[]
                {
                    new Vector2(1,-1),new Vector2(1,0), new Vector2(1,1),
                    new Vector2(0,1), new Vector2(-1,1),
                    new Vector2(-1,0),new Vector2(-1,-1), new Vector2(0,-1)
                };
                //for (int i = 0; i < move_vecs.Length; i++)
                //    move_vecs[i].Normalize();



                Vector2 point_out = Vector2.Zero;

                for (int i = 1; i < 2; i++)
                {
                    Vector2[] poses = new Vector2[move_vecs.Length];
                    for (int j = 0; j < move_vecs.Length; j++)
                        poses[j] = this.Pos + (move_vecs[j] * (float)i * 0.4f);

                    int[] col_steps = new int[poses.Length];
                    for (int j = 0; j < col_steps.Length; j++)
                    {
                        // Border check
                        if (poses[j].X - 0.4f < 1 || poses[j].X + 0.4f > 19 || poses[j].Y - 0.4f < 1 || poses[j].Y + 0.4f > 19)
                        {
                            col_steps[j] = 0;
                            continue;
                        }

                        // Bullet collision check
                        int min_bullet = BulletSim.CollidesWith(poses[j], 0.4f, bullet_poses);

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
                    }
                    else
                        break;
                }

                move_target = point_out;

                color = Color.Purple * 0.5f;
            }
            else
            {
                color = Color.HotPink * 0.5f;
                move_target = Pos;


                Bullet[] bullets = this.env.Bullets.ToList().FindAll(x => x.Collectible).ToArray();
                float min_len = int.MaxValue;
                int min_len_idx = -1;
                for (int i = 0; i < bullets.Length; i++)
                {

                    if ((bullets[i].Pos - this.Pos).Length() < min_len)
                    {
                        min_len = (bullets[i].Pos - this.Pos).Length();
                        min_len_idx = i;
                    }

                    if (min_len_idx > -1)
                    {
                        // Check if it is safe to move towards the desired location
                        int collision = BulletSim.CollidesWith(bullets[min_len_idx].Pos, 0.4f, bullet_poses);
                        move_target = collision == -1 ? bullets[min_len_idx].Pos : this.Pos;
                    }
                    else
                        move_target = this.Pos;
                }

            }

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
    }


    static class BulletSim
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
                    if (bullet_poses[i, j].X + 0.2f >= pos.X - radius && bullet_poses[i, j].X - 0.2f <= pos.X + radius &&
                        bullet_poses[i, j].Y + 0.2f >= pos.Y - radius && bullet_poses[i, j].Y - 0.2f <= pos.Y + radius)
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
                if (n_bullet.Pos.X + 0.2f >= pos.X - radius && n_bullet.Pos.X - 0.2f <= pos.X + radius &&
                    n_bullet.Pos.Y + 0.2f >= pos.Y - radius && n_bullet.Pos.Y - 0.2f <= pos.Y + radius)
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
        static void SetValue(object s, string name, object value)
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