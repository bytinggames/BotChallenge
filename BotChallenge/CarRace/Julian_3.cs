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
    class Julian_4 : Bot
    {
        Vector2[] goalsTangents;
        Vector2[] goalsNormals, goalsBetween;
        float[] curvy;

        Vector2 startPos;

        List<Vector2> track = new List<Vector2>();
        int trackIndex = 0;

        bool reachedFirst = false;
        bool[] sCurve;

        const int trackPointsPerGoal = 100;

        protected override void InitializeCustom()
        {
            startPos = positionV;
            //track.Add(startPos);
            // initialization code - run once per start of a race

            goalsTangents = new Vector2[goals.Count];
            goalsNormals = new Vector2[goals.Count];
            goalsBetween = new Vector2[goals.Count];
            curvy = new float[goals.Count];

            for (int i = 0; i < goals.Count - 1; i++)
            {
                Vector2 toLast = (i > 0 ?goals[i - 1] : positionV) - goals[i];
                Vector2 toNext = goals[i + 1] - goals[i];
                toLast.Normalize();
                toNext.Normalize();
                goalsBetween[i] = ((toLast + toNext) / 2f);
                goalsBetween[i] = Vector2.Normalize(goalsBetween[i]);

                float toLastAngle = (float)Math.Atan2(toLast.Y, toLast.X);
                float toNextAngle = (float)Math.Atan2(toNext.Y, toNext.X);

                float angleDiff = (float)(toNextAngle - toLastAngle);

                if (angleDiff < 0)
                    angleDiff += MathHelper.TwoPi;
                if (angleDiff > MathHelper.Pi)
                    angleDiff = MathHelper.Pi - angleDiff;

                if (angleDiff > 0)
                    goalsNormals[i] = -goalsBetween[i];
                else
                    goalsNormals[i] = goalsBetween[i];

                goalsTangents[i] = new Vector2(goalsNormals[i].Y, -goalsNormals[i].X);

                curvy[i] = Vector2.Dot(toLast, toNext);
                curvy[i] *= 0f;
                curvy[i] += 1f;
                //if (curvy[i] < 0)
                //    curvy[i] = 0f;
            }

            // first goal tangent: mirror next tangent by dist to next goal
                Vector2 dist = Vector2.Normalize(goals[1] - goals[0]);
            Vector2 t = goalsTangents[1];
            // mirror t over dist:
            Vector2 tAlongDist = Vector2.Dot(dist, t) * dist;
            Vector2 tNormalDist = t - tAlongDist;
            t = tAlongDist - tNormalDist;
            //goalsTangents[0] = t;
            //goalsNormals[0] = new Vector2(-goalsTangents[0].Y, goalsTangents[0].X);

            // last goal tangent: get dist to previous goal negative
            goalsTangents[goals.Count - 1] = Vector2.Normalize(goals[goals.Count - 1] - goals[goals.Count - 2]);

            sCurve = new bool[goals.Count];

            for (int j = -1; j < goals.Count - 1; j++)
            {
                Vector2 g1 = j >= 0 ? goals[j] : positionV ;
                Vector2 g2 = goals[j + 1];
                Vector2 t1 = j >= 0 ? goalsTangents[j] : orientationV;
                Vector2 n1 = j >= 0 ? goalsNormals[j] : directionRightV;

                float curveLength = (g1 - g2).Length();
                Console.WriteLine("c: " + curveLength);
                //if (curveLength > 50)
                //    curveLength = 50;
                float curveStrength = curveLength * 0.1f;

                if (curveStrength < 10)
                    curveStrength = 10f;
                for (int i = 0; i < trackPointsPerGoal; i++)
                //int i = (int)(trackPointsPerGoal * 0.75f);
                {
                    float x = (float)i / trackPointsPerGoal;
                    float xForPos = x;
                    xForPos = (float)Math.Pow(x, 0.5f);
                    //float a = i * 4f;
                    //if (a > 2)
                    //    a = 2;
                    //a = -a;
                    //a += 1f;
                    //a = Math.Abs(1f);
                    //a = -a;
                    //a += 1f;
                    Vector2 d = g2 - g1;
                    Vector2 dn = Vector2.Normalize(new Vector2(-d.Y, d.X));
                    if (Vector2.Dot(t1, dn) < 0)
                        dn = -dn;

                    sCurve[j + 1] = Vector2.Dot(n1, d) * Vector2.Dot(goalsNormals[j + 1], d) > 0;

                    //if (sCurve[j + 1] && curvy[j + 1] > 0.8f)
                    //{
                    //    sCurve[j + 1] = false;
                    //    goalsNormals[j + 1] = -goalsNormals[j + 1];
                    //    goalsTangents[j + 1] = -goalsTangents[j + 1];
                    //}

                    if (sCurve[j + 1])
                    {
                        float a = x * 2f;
                        if (a > 1)
                            a = 1;
                        a = (float)Math.Sin(a * MathHelper.Pi);

                        float b = x * 2f - 1f;
                        if (b < 0)
                            b = 0;
                        b = (float)Math.Sin(b * MathHelper.Pi);
                        b = (float)Math.Pow(b, 0.5f);
                        if (b > 0)
                        { }

                        //if (x < 0.75f)
                        //    continue;

                        track.Add(g1 * (1f - xForPos) + g2 * xForPos + (dn * curveStrength * (j >= 0 ? curvy[j] : 0f) * a - dn * curveStrength * curvy[j + 1] * b));
                        //track.Add((g1 * i + g2 * (1f - i)));
                    }
                    else
                    {
                        float a = x;
                        a = (float)Math.Sin(a * MathHelper.Pi);

                        float b = 1f - x;
                        b = (float)Math.Sin(b * MathHelper.Pi);


                        //if (x < 0.75f)
                        //    continue;

                        track.Add(g1 * (1f - xForPos) + g2 * xForPos + (t1 * curveStrength * a - goalsTangents[j + 1] * curveStrength * b));

                        //track.Add((g1 * i + g2 * (1f - i)));
                    }
                }
            }

            for (int i = 0; i < goals.Count; i++)
            {
                goals[i] += goalsBetween[i] * 3;
            }
        }


        int nextIndex;

        bool tryCoolCurveOverGoal = false;
        bool reachedTrack = false;

        int tInArray;

            Vector2[] predictions = new Vector2[10];
        // this function is called every frame
        protected override EnvCarRace.Action GetAction()
        {
            // brake if prediction aims wrong
            prediction = positionV + velocityV * 0.7f;
            predictionForCheckpoint = positionV + velocityV * 0.75f;

            for (int i = 0; i < predictions.Length; i++)
            {
                Vector2 vN = Vector2.Normalize(new Vector2(-velocityV.Y, velocityV.X));
                float currentSteeringInComparisonToVelocity = Vector2.Dot(vN, orientationV);

                float ijau = ((float)i / predictions.Length);
                predictions[i] = positionV + velocityV * ijau;// + directionRightV * currentSteeringInComparisonToVelocity * ((float)Math.Pow(ijau, 1.2f)) * 10f;
            }

            EnvCarRace.Action action = new EnvCarRace.Action();

            // insanely awesome bot code goes here

            // set action.<> values to steer your bot
            //action.accelerate = rand.NextFloat();
            //action.steer = 1f;
            //action.brake = (float)Math.Round(0.2f); // use Math. for math calculations


            if (goalIndex > 0)
                reachedFirst = true;

            if (goalIndex + 1 > trackIndex)
            {
                reachedTrack = false;
            }

            if (goalIndex == goals.Count - 1)
                reachedTrack = true;


            Vector2 goal;
            if (!reachedTrack)
            {
                float scurvejau = sCurve[goalIndex] ? 0.75f : 0.5f;
                tInArray = trackIndex * trackPointsPerGoal + (int)(trackPointsPerGoal * scurvejau);

                Vector2 trackTangent = Vector2.Normalize(track[tInArray + 1] - track[tInArray]);
                Vector2 distToPlayer = predictionForCheckpoint - track[tInArray];

                if ((Vector2.Dot(distToPlayer, trackTangent) > 0)
                    || ((currentGoalV - positionV).Length() < (currentGoalV - track[tInArray]).Length()))
                {
                    //if (trackIndex / trackPointsPerGoal < goalIndex + 1)
                    //if (trackIndex < goalIndex + 2)
                    //{
                    trackIndex++;
                    //trackTangent = Vector2.Normalize(track[tInArray + 1] - track[tInArray]);
                    //distToPlayer = positionV - track[tInArray];
                    reachedTrack = true;
                    //}
                    //else
                    //    break;
                }

                //nextIndex = trackIndex;// (int)(trackIndex + velocityV.Length() / ((currentGoalV - (goalIndex > 0 ? goals[goalIndex - 1] : startPos)).Length() / 40f));

                    //if ((nextIndex / trackPointsPerGoal) > goalIndex)
                    //{
                    //    nextIndex = (goalIndex + 1) * trackPointsPerGoal;
                    //    tryCoolCurveOverGoal = true;
                    //}
                    //else
                    //    tryCoolCurveOverGoal = false;

                    Console.WriteLine(velocityV.Length());
                if (tInArray > track.Count - 1)
                {
                    tInArray = track.Count - 1;
                }

                goal = track[tInArray];// this.currentGoalV;
            }
            else
            {
                goal = currentGoalV;
            }
            Vector2 dist = goal - positionV;

            //float distAngle = (float)Math.Atan2(dist.Y, dist.X);

            //float angle = distAngle - orientation;
            //if (Math.Abs(angle) > MathHelper.Pi)
            //    angle -= MathHelper.TwoPi;


            Vector2 distNormalized = Vector2.Normalize(dist);

            float toRight = (Vector2.Dot(directionRightV, distNormalized));


            //Vector2 x1 = track[nextIndex] - track[nextIndex - 1];
            //Vector2 x2 = orientationV;//positionV - track[nextIndex - 1];
            //Vector2 n1 = new Vector2(-x1.Y, x1.X);

            //float toRight2 = Vector2.Dot(n1, x2);

            //if (toRight + toRight2 * 100 < 0)

            if (toRight >= 0)
                action.steer = 1f;
            else
                action.steer = -1f;




            bool overGoal = false;
            Vector2 goalToPos = positionV - currentGoalV;
            Vector2 goalToPrediction = prediction - currentGoalV;
            if (Vector2.Dot(goalToPos, goalToPrediction) < 0)
                overGoal = true;

            bool predictCol = false;

            foreach (var p in predictions)
            {
                if (Vector2.Distance(currentGoalV, p) < (env.goalRadius + width))
                {
                    predictCol = true;
                    break;
                }
            }

            if (overGoal && !predictCol)
                action.brake = 1f;




            float forward = (Vector2.Dot(orientationV, distNormalized));

            float d = dist.Length();

            if (goalIndex < goals.Count - 1)
            {
                //float curve = -Vector2.Dot(distNormalized, Vector2.Normalize(goals[goalIndex + 1] - goal));
                //if (curve > 0)
                //{
                //    float end = 10 + curve * 15;
                //    if (d < end)
                //    {
                //        d = 0;
                //        //d -= end * 2f / 3f;
                //    }
                //}
            }

            float targetVelocity = (float)(Math.Sign(forward) * Math.Abs(Math.Pow(forward, 10f))) * d * 7f;

            if (action.brake == 1f)
                targetVelocity = 0f;

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
            return Color.Orange; // determine your beautiful look
        }

        Vector2 prediction, predictionForCheckpoint;

        protected override void DrawCustom()
        {
            //DrawM.Vertex.DrawCircleOutline(positionV, 10f, Color.White * 0.5f, 8f);
            //DrawM.Vertex.DrawLineThin(positionV, this.currentGoalV, Color.White * 0.5f);

            for (int i = 0; i < goals.Count; i++)
            {
                DrawM.Vertex.DrawCircle(goals[i], 0.5f, Color.Red, 8f);
                DrawM.Vertex.DrawLineThin(goals[i], goals[i] + goalsTangents[i] * 3f, Color.White);
            }

            for (int i = 0; i < track.Count; i++)
            {
                DrawM.Vertex.DrawCircle(track[i], 0.5f, Color.Black, 8f);
            }

            DrawM.Vertex.DrawCircle(track[tInArray], 1f, Color.Red, 16f);
            DrawM.Vertex.DrawCircle(track[nextIndex], 1f, Color.Blue, 16f);
            // DrawM.Vertex....

            DrawM.Vertex.DrawCircle(prediction, 2f, Color.White * 0.5f, 16f);
            DrawM.Vertex.DrawCircle(predictionForCheckpoint, 2f, Color.Black * 0.5f, 16f);

            for (int i = 0; i < predictions.Length; i++)
            {
                DrawM.Vertex.DrawCircle(predictions[i], 1f, Color.White * 0.5f, 16f);
            }

        }
    }
}
