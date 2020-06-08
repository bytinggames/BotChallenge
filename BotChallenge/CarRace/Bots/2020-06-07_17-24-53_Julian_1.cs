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
    class Julian_1 : Bot
    {
        Vector2[] goalsTangents;
        Vector2[] goalsNormals;

        List<Vector2> track = new List<Vector2>();

        protected override void InitializeCustom()
        {
            // initialization code - run once per start of a race

            goalsTangents = new Vector2[goals.Count];
            goalsNormals = new Vector2[goals.Count];

            for (int i = 1; i < goals.Count - 1; i++)
            {
                Vector2 toLast = goals[i - 1] - goals[i];
                Vector2 toNext = goals[i + 1] - goals[i];
                toLast.Normalize();
                toNext.Normalize();
                goalsNormals[i] = ((toLast + toNext) / 2f);
                goalsNormals[i] = Vector2.Normalize(goalsNormals[i]);

                goalsTangents[i] = new Vector2(goalsNormals[i].Y, -goalsNormals[i].X);
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
            goalsTangents[goals.Count - 1] = Vector2.Normalize(goalsTangents[goals.Count - 1] - goalsTangents[goals.Count - 2]);

            Vector2 g1 = goals[0];
            Vector2 g2 = goals[1];

            float curveStrength = 10f;// (g1 -g2).Length();

            for (float i = 0; i < 1; i += 0.1f)
            {
                float a = i * 4f;
                a = 1f - a;

                track.Add(g1 * i + (1f - i) * g2 +  (goalsTangents[0] * curveStrength * (i * 2f) + goalsTangents[1] * curveStrength * (1f - i)));
                //track.Add((g1 * i + g2 * (1f - i)));
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


            Vector2 goal =  this.currentGoalV;

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
                float curve = -Vector2.Dot(distNormalized, Vector2.Normalize(goals[goalIndex + 1] - currentGoalV));
                float end = 15 + curve * 30;
                if (d < end)
                {
                    d -= end * 2f / 3f;
                }
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
