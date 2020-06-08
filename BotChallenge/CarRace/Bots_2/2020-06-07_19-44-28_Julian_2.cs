using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.CarRace;
using JuliHelper;
using Microsoft.Xna.Framework;

namespace CarRace
{
    class Julian_2 : Bot
    {
        Vector2[] goalsTangents;
        Vector2[] goalsNormals;
        float[] curvy;

        List<Vector2> track = new List<Vector2>();
        int trackIndex = 0;

        bool reachedFirst = false;

        protected override void InitializeCustom()
        {
            // initialization code - run once per start of a race

            goalsTangents = new Vector2[goals.Count];
            goalsNormals = new Vector2[goals.Count];
            curvy = new float[goals.Count];

            for (int i = 1; i < goals.Count - 1; i++)
            {
                Vector2 toLast = goals[i - 1] - goals[i];
                Vector2 toNext = goals[i + 1] - goals[i];
                toLast.Normalize();
                toNext.Normalize();
                goalsNormals[i] = ((toLast + toNext) / 2f);
                goalsNormals[i] = Vector2.Normalize(goalsNormals[i]);

                float toLastAngle = (float)Math.Atan2(toLast.Y, toLast.X);
                float toNextAngle = (float)Math.Atan2(toNext.Y, toNext.X);

                float angleDiff = (float)(toNextAngle - toLastAngle);

                if (angleDiff < 0)
                    angleDiff += MathHelper.TwoPi;
                if (angleDiff > MathHelper.Pi)
                    angleDiff = MathHelper.Pi - angleDiff;

                if (angleDiff > 0)
                    goalsNormals[i] = -goalsNormals[i];

                goalsTangents[i] = new Vector2(goalsNormals[i].Y, -goalsNormals[i].X);

                curvy[i] = Vector2.Dot(toLast, toNext);

            }

            // first goal tangent: mirror next tangent by dist to next goal
            Vector2 dist = Vector2.Normalize(goals[1] - goals[0]);
            Vector2 t = goalsTangents[1];
            // mirror t over dist:
            Vector2 tAlongDist = Vector2.Dot(dist, t) * dist;
            Vector2 tNormalDist = t - tAlongDist;
            t = tAlongDist - tNormalDist;
            goalsTangents[0] = t;
            goalsNormals[0] = new Vector2(-goalsTangents[0].Y, goalsTangents[0].X);

            // last goal tangent: get dist to previous goal negative
            goalsTangents[goals.Count - 1] = Vector2.Normalize(goals[goals.Count - 1] - goals[goals.Count - 2]);



            for (int j = 0; j < goals.Count - 1; j++)
            {
                Vector2 g1 = goals[j];
                Vector2 g2 = goals[j + 1];
                float curveStrength = (g1 - g2).Length() * 0.2f;
                for (float i = 0; i < 1; i += 0.01f)
                {
                    //float a = i * 4f;
                    //if (a > 2)
                    //    a = 2;
                    //a = -a;
                    //a += 1f;
                    //a = Math.Abs(1f);
                    //a = -a;
                    //a += 1f;

                    if (Vector2.Dot(goalsTangents[j], goalsTangents[j + 1]) > 0)
                    {
                        float a = i * 2f;
                        if (a > 1)
                            a = 1;
                        a = (float)Math.Sin(a * MathHelper.Pi);

                        float b = i * 2f - 1f;
                        if (b < 0)
                            b = 0;
                        b = (float)Math.Sin(b * MathHelper.Pi);
                        b = (float)Math.Pow(b, 0.5f);
                        if (b > 0)
                        { }

                        track.Add(g1 * (1f - i) + g2 * i + (goalsTangents[j] * curveStrength * curvy[j] * a - goalsTangents[j + 1] * curveStrength * curvy[j + 1] * b));
                        //track.Add((g1 * i + g2 * (1f - i)));
                    }
                    else
                    {
                        float a = i;
                        a = (float)Math.Sin(a * MathHelper.Pi);

                        float b = 1f - i;
                        b = (float)Math.Sin(b * MathHelper.Pi);


                        track.Add(g1 * (1f - i) + g2 * i + (goalsTangents[j] * curveStrength * a - goalsTangents[j + 1] * curveStrength * b));

                        //track.Add((g1 * i + g2 * (1f - i)));
                    }
                }
            }
        }



        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            EnvCarRace.Action action = new EnvCarRace.Action();

            // insanely awesome bot code goes here

            // set action.<> values to steer your bot
            //action.accelerate = rand.NextFloat();
            //action.steer = 1f;
            //action.brake = (float)Math.Round(0.2f); // use Math. for math calculations


            if (goalIndex > 0)
                reachedFirst = true;


            Vector2 goal;
            if (reachedFirst)
            {
                Vector2 trackTangent = Vector2.Normalize(track[trackIndex + 1] - track[trackIndex]);
                Vector2 distToPlayer = positionV - track[trackIndex];

                while (Vector2.Dot(distToPlayer, trackTangent) > 0)
                {
                    trackIndex++;
                    trackTangent = Vector2.Normalize(track[trackIndex + 1] - track[trackIndex]);
                    distToPlayer = positionV - track[trackIndex];
                }

                int nextIndex = trackIndex + 10;

                if (nextIndex > track.Count - 1)
                {
                    nextIndex = track.Count - 1;
                }

                goal = track[nextIndex];// this.currentGoalV;
            }
            else
            {
                goal = goals[0];
            }
            Vector2 dist = goal - positionV;

            //float distAngle = (float)Math.Atan2(dist.Y, dist.X);

            //float angle = distAngle - orientation;
            //if (Math.Abs(angle) > MathHelper.Pi)
            //    angle -= MathHelper.TwoPi;


            Vector2 distNormalized = Vector2.Normalize(dist);

            float toRight = (Vector2.Dot(directionRightV, distNormalized));

            if (toRight >= 0)
                action.steer = 1f;
            else
                action.steer = -1f;

            float forward = (Vector2.Dot(orientationV, distNormalized));

            float d = dist.Length();

            if (goalIndex < goals.Count - 1)
            {
                float curve = -Vector2.Dot(distNormalized, Vector2.Normalize(goals[goalIndex + 1] - goal));
                //float end = 15 + curve * 30;
                //if (d < end)
                //{
                //    d -= end * 2f / 3f;
                //}
            }

            float targetVelocity = (float)(Math.Sign(forward) * Math.Abs(Math.Pow(forward, 10f))) * d * 5f;

            if (targetVelocity < 10f)
                targetVelocity = 10f;

            if (velocityV.Length() < targetVelocity)
                action.accelerate = 1f;
            else
                action.brake = 1f;

            // type this. to see what you have access to
            //this.<>
            //this.currentGoalV f.ex.

            return action;
        }


        protected override Color GetColor()
        {
            return Color.White; // determine your beautiful look
        }

        protected override void DrawCustom()
        {
            //DrawM.Vertex.DrawCircleOutline(positionV, 10f, Color.White * 0.5f, 8f);
            //DrawM.Vertex.DrawLineThin(positionV, this.currentGoalV, Color.White * 0.5f);

            for (int i = 0; i < goals.Count; i++)
            {
                DrawM.Vertex.DrawLineThin(goals[i], goals[i] + goalsTangents[i] * 3f, Color.White);
            }

            for (int i = 0; i < track.Count; i++)
            {
                DrawM.Vertex.DrawCircle(track[i], 0.5f, Color.Black, 8f);
            }
            // DrawM.Vertex....


        }
    }
}
