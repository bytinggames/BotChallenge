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
        public EnvCarRace.Action action;

        const float power = 40f;
        const float handling = 1f;
        const float drag = 0.01f;//4257f;

        public bool Alive = true;

        public int goalIndex { get; private set; } = 0;


        #region constants

        const float length = 2.5f; // m
        const float massCenterFromBack = 1f; // from the back of the car
        const float massCenterHeight = 0.5f; // from the ground
        const float width = 1.7f; // m
        const float mass = 1500f; // kg
        const float wheelMass = 7.5f; // kg
        const float wheelRadius = 0.34f;



        // friction
        const float rollingResistance = 0.1f;
        const float rollingFriction = 0.01f;
        const float braking = 50000f;
        const float engine = 10000f;
        const float rubberFrictionMax = 1f;

        const float engineTorque = 448f; // TODO:
        const float gearRatio = 2.66f;
        const float differentialRatio = 3.42f;
        const float transmissionEfficiency = 0.7f;

        const float tractionConstant = 100000f; // how sharp is the wheel grip curve


        // gravity
        const float g = 9.8f; // m/s^2



        #endregion

        #region dynamic physics variables

        public Vector2 velocity2; // m/s
        public M_Polygon mask;

        Vector2 dir2 { get => Calculate.AngleToVector(orientation); }
        Vector2 dirRight2 { get => Calculate.AngleToVector(orientation + MathHelper.PiOver2); }
        public float orientationAngularVelocity;

        private float _orientation;

        float rearWheelAngularVelocity = 81.7f; // 0: left, 1: right
        float rearWheelAngle = MathHelper.PiOver2;

        #endregion

        List<Vector2> maskSourceVertices;

        List<M_Polygon> driftLines = new List<M_Polygon>();

        #region accessors

        public Vector2 pos
        {
            get { return mask.pos; }
            set
            {
                mask.pos = value;
            }
        }
        public float posX
        {
            get { return mask.pos.X; }
            set
            {
                mask.X = value;
            }
        }
        public float posY
        {
            get { return mask.pos.Y; }
            set
            {
                mask.Y = value;
            }
        }

        internal float orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                mask.vertices = maskSourceVertices.ToList();
                mask.RotateRadians(_orientation);
            }
        }

        #endregion

        internal bool control;

        protected Random rand;
        protected EnvCarRace env;
        private int id;

        public Bot(bool control = true)
        {
            this.control = control;
            float w = length / 2f;
            float h = width / 2f;
            maskSourceVertices = new List<Vector2>() { new Vector2(w, h), new Vector2(-w, h), new Vector2(-w, -h), new Vector2(w, -h) };
            mask = new M_Polygon(Vector2.Zero, maskSourceVertices.ToList());

            if (!control)
                posX += 10;
        }

        internal void Initialize(EnvCarRace env, Vector2 pos, float orientation, int id)
        {
            this.env = env;
            this.pos = pos;
            this.orientation = orientation;
            this.id = id;
            
            rand = Env.constRand;
        }


        float slipRatio;

        Vector2 acceleration2;

        bool staticFriction = true;
        internal int frameTime;

        public void Update()
        {
            float elapsedThisFrameS = 1f / 60f;

            if (elapsedThisFrameS == 0)
                return;

            float velocity = velocity2.Length();

            float velocityLong = Vector2.Dot(velocity2, dir2);
            Vector2 velocityLong2 = velocityLong * dir2;
            Vector2 velocityLat2 = velocity2 - velocityLong2;
            float velocityLat = velocityLat2.Length();

            #region force to wheels
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

                //if (turnSpeed == 0)
                //    turnSpeed = -Math.Sign(orientationAngularVelocity) * 10f;

                handBrake = Input.space.down;
            }
            else
                brakePedal = 1f;
            #endregion

            bool oldStaticFriction = staticFriction;

            staticFriction = !handBrake
                && (Math.Abs(velocityLong) > Math.Abs(velocityLat) * 1.2f && Math.Abs(orientationAngularVelocity * velocity) < 300f);

            //Console.WriteLine(orientationAngularVelocity * velocity);

            if (oldStaticFriction && !staticFriction)
            {
                // begin drift
                driftLines.Add(new M_Polygon(Vector2.Zero, new List<Vector2>(), false));
                driftLines.Add(new M_Polygon(Vector2.Zero, new List<Vector2>(), false));
            }


            if (!staticFriction)
            {
                driftLines[driftLines.Count - 2].vertices.Add(pos + mask.vertices[1]);
                driftLines[driftLines.Count - 1].vertices.Add(pos + mask.vertices[2]);
            }


            float thrustForce = throttlePedal * power;

            if (brakePedal > 0f)
                thrustForce -= Math.Sign(velocityLong) * brakePedal * power * 0.5f;

            if (!staticFriction)
                thrustForce *= 0.8f;

            Vector2 thrustForce2 = Vector2.Zero;
            if (!handBrake)
                thrustForce2 = dir2 * thrustForce;


            // drag force
            Vector2 dragForce2 = -drag * velocity2 * velocity2.Length();

            Vector2 wheelLatForce = Vector2.Zero;

            if (velocityLat2 != Vector2.Zero)
                wheelLatForce -= Vector2.Normalize(velocityLat2) * 20f;// 0.9f; 

            if (handBrake && velocityLong2 != Vector2.Zero)
                wheelLatForce += -Vector2.Normalize(velocityLong2) * 20f;


            // rolling resistance force
            Vector2 rollingResistanceForce2 = -rollingResistance * velocityLong2;

            Vector2 force2 = thrustForce2 + dragForce2 + rollingResistanceForce2 + wheelLatForce;

            //Console.WriteLine(thrustForce2.Length() + " " + dragForce2.Length() + " " + rollingResistanceForce2.Length());

            if (!staticFriction)
            {
                orientationAngularVelocity += turnSpeed * elapsedThisFrameS;
                orientationAngularVelocity *= 0.97f;
            }
            else
            {
                float targetVelocity = turnSpeed * 15f * elapsedThisFrameS;
                orientationAngularVelocity += (targetVelocity - orientationAngularVelocity) * 0.5f;// * Math.Min(Math.Abs(velocity) * 0.01f, 1f) ;
            }

            orientation += orientationAngularVelocity * elapsedThisFrameS;

            acceleration2 = force2;// / mass;

            //if (handBrake)
            //    velocityLong2 *= 0.99f;
            //velocityLat2 *= 0.9f; // side rubber friction
            //velocityLat2 -= 

            //velocity2 = velocityLong2 + velocityLat2;

            Vector2 vOld = velocity2;

            velocity2 += acceleration2 * elapsedThisFrameS;

            if (Vector2.Dot(vOld, velocity2) < 0) // if pointed at different direction, stop it first
                velocity2 = Vector2.Zero;

            pos += velocity2 * elapsedThisFrameS;

            while (goalIndex < env.goals.Count && mask.ColCircle(new M_Circle(env.goals[goalIndex], EnvCarRace.GOALRADIUS)))
            {
                goalIndex++;
                if (goalIndex == EnvCarRace.GOALCOUNT)
                {
                    frameTime = env.Frame;
                    control = false;
                }
            }

            //if (posX - length / 2f > G.camera.view.Right)
            //    posX = G.camera.view.Left - length / 2f;
            //if (posX + length / 2f < G.camera.view.Left)
            //    posX = G.camera.view.Right + length / 2f;

            //if (posY - length / 2f > G.camera.view.Bottom)
            //    posY = G.camera.view.Top - length / 2f;
            //if (posY + length / 2f < G.camera.view.Top)
            //    posY = G.camera.view.Bottom + length / 2f;

        }

        public void Draw()
        {
            //DrawM.Vertex.DrawCircleOutline(pos + mask.vertices[1], wheelRadius, Color.Black, 16);// dir2 * (-length / 2f + massCenterFromBack), 0.1f, Color.Black, 8f);
            //DrawM.Vertex.DrawLineThin(pos + mask.vertices[1], pos + mask.vertices[1] + Calculate.AngleToVector(rearWheelAngle) * wheelRadius, Color.Black);

            //Vector2 offset = mask.vertices[1] + new Vector2(0, wheelRadius);
            //for (int i = 0; i < 10; i++)
            //{
            //    DrawM.Vertex.DrawCircle(offset, 0.02f, Color.Black, 8);

            //    offset.X += MathHelper.TwoPi * wheelRadius;
            //}
            //ContentLoader.fonts["lato-thin-mod_10"].Draw($"slipRatio: {slipRatio}\nkm/h: {velocity2.Length() * 60f * 60f / 1000f}", Anchor.TopLeft(G.camera.view.pos), Color.Black, new Vector2(0.05f * G.camera.zoom));


            for (int i = 0; i < driftLines.Count; i++)
            {
                driftLines[i].Draw(Color.Black);
            }


            mask.Draw(GetColor());
            DrawM.Vertex.DrawLineThin(pos + dir2 * (-length / 2f + massCenterFromBack), pos + dir2 * length / 2f, Color.Black);
            //DrawM.Vertex.DrawCircle(pos + dir2 * (-length / 2f + massCenterFromBack), 0.1f, Color.Black, 8f);
        }

        float lookupTorqueCurve(float rpm, int gear)
        {
            switch (gear)
            {
                case 1: return 4000f;
                case 2: return 2700f;
                case 3: return 2000f;
                case 4: return 1500f;
                case 5: return 1100f;
                case 6: return 750f;
                default: return 0f;
            }
        }

        float getTractionRatio(float slipRatio)
        {
            //if (slipRatio > 1f)
            //    slipRatio = 1f;
            //if (slipRatio < -1f)
            //    slipRatio = -1f;
            float traction = 0.06f / slipRatio;

            if (traction > 1f)
                traction = 1f;
            else if (traction < -1f)
                traction = -1f;

            return traction;

            //if (slipRatio > 0.06f)
            //    return 1f / (0.06f / slipRatio);
            //    //slipRatio = 0.06f;
            //return 1f;// tractionConstant * slipRatio;
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
