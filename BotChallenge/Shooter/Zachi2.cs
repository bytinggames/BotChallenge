
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
    class Zachi2 : EnvShooter.Bot
    {

        EnvShooter.Action lastAction;
        Color color = Color.DeepPink;


        protected override Color GetColor()
        {
            return color;
        }

        public Zachi2()
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



            int min_col = int.MaxValue;
            int col_idx = -1;

            for (int i = 0; i < this.env.Bullets.Length; i++)
            {
                if (this.env.Bullets[i].Collectible)
                    continue;
                int player_collision = BulletSim2.CollidesWith(this.Pos, 0.4f, this.env.Bullets[i], this.map, 120);
                if (player_collision > -1)
                {
                    if (player_collision < min_col)
                    {
                        min_col = player_collision;
                        col_idx = i;
                    }

                }
            }

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
                        if (poses[j].X - 0.4f < 1 || poses[j].X + 0.4f > 19 || poses[j].Y - 0.4f < 1 || poses[j].Y + 0.4f > 19)
                        {
                            col_steps[j] = 0;
                            continue;
                        }

                        int min_bullet = int.MaxValue;
                        int min_bullet_idx = -1;
                        for (int y = 0; y < this.env.Bullets.Length; y++)
                        {
                            int c_steps = BulletSim2.CollidesWith(poses[j], 0.4f, this.env.Bullets[y], this.map, 120);
                            // closest bullet death found
                            if (c_steps > -1 && c_steps < min_bullet)
                            {
                                min_bullet = c_steps;
                                min_bullet_idx = y;
                            }
                        }

                        // No collisions
                        if (min_bullet == int.MaxValue)
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
                        else if (col_steps[i] > max_dist)
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

                color = Color.Purple;
            }
            else
            {
                color = Color.HotPink;
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
                        move_target = bullets[min_len_idx].Pos;
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


    static class BulletSim2
    {


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