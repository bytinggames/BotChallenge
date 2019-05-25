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
    class Dominik3 : EnvShooter.Bot

    {

        protected override Color GetColor()
        {
            return Color.Yellow;
        }

        public Dominik3()
        {
            //Start
        }

        bool[] getDirection(Vector2 vec, Vector2 enemyPos)
        {
            bool[] dir = new bool[4];

            float wurzel = (float)(1f / Math.Sqrt(2));
            float[] test = new float[8]{ Math.Abs(vec.X), Math.Abs(wurzel * (vec.X) + wurzel * (vec.Y)), Math.Abs(vec.Y), Math.Abs(-wurzel * (vec.X) + wurzel * (vec.Y)),
                    Math.Abs(-vec.X), Math.Abs(-wurzel * (vec.X) - wurzel * (vec.Y)), Math.Abs(-vec.Y), Math.Abs(wurzel* (vec.X) - wurzel * (vec.Y))};

            int ind = 0;
            for (int i = 1; i < 4; i++)
            {
                if (test[i] < test[ind]) { ind = i; break; }
            }
            
            switch (ind)
            {
                case 0:
                    if (Vector2.Distance(this.Pos + EnvShooter.Bot.SPEED*(new Vector2(1,0)),enemyPos) > Vector2.Distance(this.Pos - EnvShooter.Bot.SPEED * (new Vector2(1, 0)), enemyPos))
                        dir[0] = true;
                    else dir[2] = true;
                    break;
                case 1:
                    if (Vector2.Distance(this.Pos + EnvShooter.Bot.SPEED * (new Vector2(wurzel, wurzel)), enemyPos) > Vector2.Distance(this.Pos - EnvShooter.Bot.SPEED * (new Vector2(wurzel, wurzel)), enemyPos))
                        { dir[0] = true; dir[1] = true; }
                    else { dir[2] = true; dir[3] = true; }
                    break;
                case 2:
                    if (Vector2.Distance(this.Pos + EnvShooter.Bot.SPEED * (new Vector2(0, 1)), enemyPos) > Vector2.Distance(this.Pos - EnvShooter.Bot.SPEED * (new Vector2(0, 1)), enemyPos)) dir[1] = true;
                    else dir[3] = true;
                    break;
                case 3:
                    if (Vector2.Distance(this.Pos + EnvShooter.Bot.SPEED * (new Vector2(-wurzel, wurzel)), enemyPos) > Vector2.Distance(this.Pos - EnvShooter.Bot.SPEED * (new Vector2(-wurzel, wurzel)), enemyPos))
                        { dir[2] = true; dir[1] = true; }
                    else { dir[0] = true; dir[3] = true; }
                    break;
                default:
                    break;
            }
            return dir;
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
            act.aim = (float)Math.Atan2(vec.Y, vec.X);

            // schießen, falls Munition vorhanden und Gegner nicht schießt
            if (this.Ammo > 0 && this.enemies[ind_enemy].Charge <= 0.0f)
                act.charge = true;

            if (this.Charge == 1)
                act.charge = false;
            

            // Berechne nächste Bullet
            bool bln_foundDanger = false;
            int ind_nearestBullet = 0;

            if (env.Bullets.Length > 1) {
                for (int i = 1; i < env.Bullets.Length; i++){
                    if (bln_foundDanger){
                        if ((Vector2.Dot(env.Bullets[i].Velocity,env.Bullets[i].Pos - this.Pos) < 0) && 
                            (env.Bullets[i].Collectible == false) &&
                            (Vector2.Distance(env.Bullets[i].Pos, this.Pos) < Vector2.Distance(env.Bullets[ind_nearestBullet].Pos, this.Pos)))
                        {
                            ind_nearestBullet = i;
                        }
                    }
                    else
                    {
                        if ((Vector2.Dot(env.Bullets[i].Velocity, env.Bullets[i].Pos - this.Pos) < 0) &&
                            (env.Bullets[i].Collectible == false))
                            {
                                ind_nearestBullet = i;
                                bln_foundDanger = true;
                            }
                    }
                    
                }
            }

            // wird eine Kugel gefährlich?
            if (bln_foundDanger && (env.Bullets.Length > 0) && (Vector2.Distance(env.Bullets[ind_nearestBullet].Pos, this.Pos) < 5))
            {
                bool[] dir = getDirection(env.Bullets[ind_nearestBullet].Velocity, env.Bullets[ind_nearestBullet].Pos);
                act.right = dir[0]; act.down = dir[1]; act.left = dir[2]; act.up = dir[3];
            }
            // charged ein Gegner gerade?
            else if ((enemies[ind_enemy].Charge > 0.50f)) {
                act.charge = false;

                bool[] dir = getDirection(vec, enemies[ind_enemy].Pos);
                act.right = dir[0]; act.down = dir[1]; act.left = dir[2]; act.up = dir[3];
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
