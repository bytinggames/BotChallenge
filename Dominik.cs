using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.Shooter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    class Dominik : EnvShooter.Bot

    {

        protected override Color GetColor()
        {
            return Color.Aquamarine;
        }

        public Dominik()
        {
            //Start
        }

        protected override EnvShooter.Action GetAction()
        {
            //Loop

            EnvShooter.Action act = new EnvShooter.Action()
            {
                right = false,
                left = false,
                up = false,
                down = false,
                charge = false,
                aim = 0
            };

            int ind_enemy = 0;
            for (int i = 0; i < this.enemies.Length; i++)
            {
                if (this.enemies[i].Alive == true) {
                    ind_enemy = i; break;
                }
            }
            Vector2 vec = enemies[ind_enemy].Pos - this.Pos;


            // schießen, falls Munition vorhanden und Gegner nicht schießt
            if (this.Ammo > 0 && this.enemies[ind_enemy].Charge == 0.0f) act.charge = true;

            if (this.Charge == 1)
            {
                act.charge = false;
                act.aim = (float) Math.Atan2(vec.Y,vec.X);
            }

            if (enemies[ind_enemy].Charge > 0.0f) {
                act.charge = false;

                float wurzel = (float)(1f / Math.Sqrt(2));
                float[] test = new float[8]{ Math.Abs(vec.X), Math.Abs(wurzel * (vec.X) + wurzel * (vec.Y)), Math.Abs(vec.Y), Math.Abs(-wurzel * (vec.X) + wurzel * (vec.Y)),
                    Math.Abs(-vec.X), Math.Abs(-wurzel * (vec.X) - wurzel * (vec.Y)), Math.Abs(-vec.Y), Math.Abs(wurzel* (vec.X) - wurzel * (vec.Y))};

                int ind = 0;
                for (int i = 1; i < 8; i++)
                {
                    if (test[i] < test[ind]) { ind = i; break; }
                }

                int fix = Env.constRand.Next(2);
                switch (ind)
                {
                    case 0:
                        if (fix == 0) act.right = true;
                        else act.left = true;
                        break;
                    case 1:
                        if (fix == 0) { act.right = true; act.down = true;}
                        else {act.left = true; act.up = true;}
                        break;
                    case 2:
                        if (fix == 0) act.down = true;
                        else act.up = true;
                        break;
                    case 3:
                        if (fix == 0) { act.left = true; act.down = true; }
                        else { act.right = true; act.up = true; }
                        break;
                    case 4:
                        if (fix == 0) act.left = true; else act.right = true;
                        break;
                    case 5:
                        if (fix == 0) { act.left = true; act.up = true; }
                        else { act.right = true; act.down = true; }
                        break;
                    case 6:
                        if (fix == 0) act.up = true; else act.down = true;
                        break;
                    case 7:
                        if (fix == 0) { act.right = true; act.up = true; }
                        else { act.left = true; act.down = true; }
                        break;
                    default:
                        break;
                }

            }
            else
            {

                // collect bullets
                int ind = 0;
                if (env.Bullets.Length > 0)
                {
                    // search nearest bullet
                    for (int i = 0; i < env.Bullets.Length; i++)
                    {
                        if (env.Bullets[i].Collectible == true)
                        {
                            if (Vector2.Distance(this.Pos,env.Bullets[i].Pos) < Vector2.Distance(this.Pos, env.Bullets[ind].Pos))
                            {
                                ind = i;
                            }
                        }
                    }

                    // in which direction go first
                    if (env.Bullets[ind].Collectible == true)
                    {
                        Vector2 dist = env.Bullets[ind].Pos - this.Pos;
                        if (Math.Abs(dist.X) > Math.Abs(dist.Y))
                        {
                            if (dist.X > 0) act.right = true;
                            else act.left = true;
                        }
                        else
                        {
                            if (dist.Y > 0) act.down = true;
                            else act.up = true;
                        }
                    }
                    else
                    {
                        // Random
                        int dec = Env.constRand.Next(4);
                        act.right = (dec == 0);
                        act.left = (dec == 1);
                        act.up = (dec == 2);
                        act.down = (dec == 3);
                    }
                }
                else
                {
                    // Random
                    int dec = Env.constRand.Next(4);
                    act.right = (dec == 0);
                    act.left = (dec == 1);
                    act.up = (dec == 2);
                    act.down = (dec == 3);
                }
            }



            return act;
        }

    }
}
