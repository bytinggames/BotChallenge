using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuliHelper;
using Microsoft.Xna.Framework;

namespace BotChallenge.CarRace
{
    public abstract class Bot
    {
        protected Random rand;
        protected EnvCarRace env;
        private int id;

        internal EnvCarRace.Action action;

        #region constant physic variables

        public readonly float length = 2.5f; // m
        public readonly float width = 1.7f; // m

        public readonly float power = 40f;
        public readonly float handling = 1f;

        public readonly float airFriction = 0.01f;
        public readonly float rollingFriction = 0.1f;

        #endregion

        #region information variables

        protected List<Vector2> goals;
        protected Vector2 currentGoalV => goals[goalIndex];

        #endregion

        public int goalIndex { get; private set; } = 0;

        
        #region dynamic physics variables

        public Vector2 velocityV { get; private set; } // m/s
        internal M_Polygon mask;
        public M_Polygon Mask { get { return (M_Polygon)mask.Clone(); } }

        public Vector2 orientationV { get; private set; }
        public Vector2 directionRightV { get; private set; }
        public float orientationAngularVelocity { get; private set; }

        private float _orientation;
        
        #endregion

        List<Vector2> maskSourceVertices;

        List<M_Polygon> driftLines = new List<M_Polygon>();

        #region accessors

        public Vector2 positionV
        {
            get { return mask.pos; }
            private set
            {
                mask.pos = value;
            }
        }
        public float positionX
        {
            get { return mask.pos.X; }
            private set
            {
                mask.X = value;
            }
        }
        public float positionY
        {
            get { return mask.pos.Y; }
            private set
            {
                mask.Y = value;
            }
        }

        internal float orientation
        {
            get { return _orientation; }
            private set
            {
                _orientation = value;
                mask.vertices = maskSourceVertices.ToList();
                mask.RotateRadians(_orientation);


                orientationV = Calculate.AngleToVector(orientation);
                directionRightV = Calculate.AngleToVector(orientation + MathHelper.PiOver2);
            }
        }

        public float goalRadius => env.goalRadius;
        public int frame => env.Frame;

        #endregion

        internal bool control;
        internal bool Alive = true;

        public Bot(bool control = true)
        {
            this.control = control;
            float w = length / 2f;
            float h = width / 2f;
            maskSourceVertices = new List<Vector2>() { new Vector2(w, h), new Vector2(-w, h), new Vector2(-w, -h), new Vector2(w, -h) };
            mask = new M_Polygon(Vector2.Zero, maskSourceVertices.ToList());

            if (!control)
                positionX += 10;
        }

        internal void Initialize(EnvCarRace env, Vector2 pos, float orientation, int id, List<Vector2> goals)
        {
            this.env = env;
            this.positionV = pos;
            this.orientation = orientation;
            this.id = id;
            this.goals = goals;
            
            rand = Env.constRand;
        }
        

        public Vector2 accelerationV { get; private set; }

        public bool staticFriction { get; private set; } = true;
        internal int frameTime;

        public void Update()
        {
            // seconds passed since last update (always the same to make replays work)
            float elapsedThisFrameS = 1f / 60f;

            // velocity helper variables
            float velocity = this.velocityV.Length();
            float velocityLong = Vector2.Dot(this.velocityV, orientationV); // velocity in direction of car heading
            Vector2 velocityLongV = velocityLong * orientationV;
            Vector2 velocityLatV = this.velocityV - velocityLongV;
            float velocityLat = velocityLatV.Length(); // velocity to the right of the car direction

            #region controls

            float throttlePedal = 0f;
            float brakePedal = 0f;
            float turnSpeed = 0f;
            bool handBrake = false;

            if (control)
            {
                throttlePedal = Math.Min(1f, Math.Max(0f, action.accelerate));

                brakePedal = Math.Min(1f, Math.Max(0f, action.brake));
                
                float cHandling = handling;
                if (velocity > 0)
                {
                    cHandling /= velocity;
                    cHandling *= 8f;
                    if (cHandling > handling)
                        cHandling = handling;

                    turnSpeed = Math.Min(1f, Math.Max(-1f, action.steer)) * cHandling * velocity;
                }
                
                //handBrake = Input.space.down;
            }
            else
                brakePedal = 1f;

            #endregion

            // determine if car is in static or kinetic friction
            bool oldStaticFriction = staticFriction;
            staticFriction = !handBrake
                && (Math.Abs(velocityLong) > Math.Abs(velocityLat) * 1.2f && Math.Abs(orientationAngularVelocity * velocity) < 300f);
            
            if (oldStaticFriction && !staticFriction)
            {
                // begin drift
                driftLines.Add(new M_Polygon(Vector2.Zero, new List<Vector2>(), false));
                driftLines.Add(new M_Polygon(Vector2.Zero, new List<Vector2>(), false));
            }

            if (!staticFriction)
            {
                // continue drift
                driftLines[driftLines.Count - 2].vertices.Add(positionV + mask.vertices[1]);
                driftLines[driftLines.Count - 1].vertices.Add(positionV + mask.vertices[2]);
            }

            // calculate thrust force
            float thrustForce = throttlePedal * power;

            if (brakePedal > 0f)
                thrustForce -= Math.Sign(velocityLong) * brakePedal * power * 0.5f;

            if (!staticFriction)
                thrustForce *= 0.8f; // kinetic friction provides less grip (80% of static friction)

            Vector2 thrustForceV = Vector2.Zero;
            if (!handBrake)
                thrustForceV = orientationV * thrustForce;


            // drag force
            Vector2 dragForceV = -airFriction * this.velocityV * this.velocityV.Length();

            Vector2 wheelLatForceV = Vector2.Zero;

            if (velocityLatV != Vector2.Zero)
                wheelLatForceV -= Vector2.Normalize(velocityLatV) * 20f;// 0.9f; 

            if (handBrake && velocityLongV != Vector2.Zero)
                wheelLatForceV += -Vector2.Normalize(velocityLongV) * 20f;


            // rolling resistance force
            Vector2 rollingResistanceForceV = -rollingFriction * velocityLongV;

            accelerationV = thrustForceV + dragForceV + rollingResistanceForceV + wheelLatForceV;
            
            if (staticFriction)
            {
                // steering is more accurate and less "spongy"
                float targetVelocity = turnSpeed * 15f * elapsedThisFrameS;
                orientationAngularVelocity += (targetVelocity - orientationAngularVelocity) * 0.5f;// * Math.Min(Math.Abs(velocity) * 0.01f, 1f) ;
            }
            else
            {
                // spongy steering
                orientationAngularVelocity += turnSpeed * elapsedThisFrameS; // turning)
                orientationAngularVelocity *= 0.97f; // decelerate angular velocity (friction)
            }

            orientation += orientationAngularVelocity * elapsedThisFrameS;

            Vector2 velocityVOld = this.velocityV;

            // increase velocity by acceleration
            this.velocityV += accelerationV * elapsedThisFrameS;

            if (Vector2.Dot(velocityVOld, this.velocityV) < 0) // if pointed at different direction, stop it first (prevent buggy back and forth motion)
                this.velocityV = Vector2.Zero;

            // increase pos by velocity
            positionV += this.velocityV * elapsedThisFrameS;

            // check if car collides with goals
            while (goalIndex < env.goals.Count && mask.ColCircle(new M_Circle(env.goals[goalIndex], env.goalRadius)))
            {
                goalIndex++;
                if (goalIndex == env.goalCount)
                {
                    frameTime = env.Frame;
                    control = false;
                }
            }
        }

        public void Draw()
        {
            Color driftColor = Color.Black * 0.5f;// Color.Lerp(Color.Black, GetColor(), 1f);
            for (int i = 0; i < driftLines.Count; i++)
            {
                driftLines[i].Draw(driftColor);
            }
            
            mask.Draw(GetColor());

            DrawM.Vertex.DrawLineThin(positionV, positionV + orientationV * length / 2f, Color.Black);
        }
        
        internal EnvCarRace.Action GetInternalAction()
        {
            return GetAction();
        }
        protected abstract EnvCarRace.Action GetAction();

        internal string GetName() { return GetType().Name; }
        internal Color GetInternalColor() { return GetColor(); }

        protected abstract Color GetColor();
    }
}
