using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JuliHelper.ThreeD
{
    public class Camera3D
    {
        public Vector3 pos, target, up;

        public float yaw, pitch, roll;

        Vector2 mouseMoveSave;

        Vector2 mbPosPast;

        public Matrix view, projection;

        //public Matrix view
        //{
        //    get { return Game1.effect.View; }
        //    set { Game1.effect.View = value; }
        //}
        //public Matrix projection
        //{
        //    get { return Game1.effect.Projection; }
        //    set { Game1.effect.Projection = value; }
        //}

        public float fieldOfView, nearClip, farClip;

        public float mouseSensitivity;

        public bool perspective = true;

        float aspectRatio
        {
            get
            {
                return (float)resX / resY;
            }
        }

        int resX, resY;

        public Camera3D(int resX, int resY, Action<int, int> ResChanged)
        {
            pos = new Vector3(0, 1.5f, 0);
            target = Vector3.Zero;
            up = Vector3.Up;


            fieldOfView = MathHelper.PiOver4;
            nearClip = 0.01f;
            farClip = 2000;

            this.ResChanged(resX, resY);

            ResChanged += this.ResChanged;



            mouseSensitivity = 0.004f;

            //CenterMouse(true);

            yaw = pitch = roll = 0;

            UpdateView();
            mouseMoveSave = Vector2.Zero;
            
        }

        public void UpdateBegin()
        {

        }

        public void UpdateEnd(int resX, int resY)
        {
            //Vector3 move = Vector3.Zero;
            //if (Input.a.down)
            //    move.X++;
            //if (Input.d.down)
            //    move.X--;
            //if (Input.w.down)
            //    move.Z++;
            //if (Input.s.down)
            //    move.Z--;
            //if (Input.space.down)
            //    move.Y++;
            //if (Input.leftShift.down)
            //    move.Y--;

            //if (move != Vector3.Zero)
            //{
            //    move.Normalize();
            //    move *= moveSpeed;

            //    if (Input.leftControl.down)
            //        move /= 10f;
            //    Vector2 moveXZ = new Vector2(move.X, move.Z);
            //    moveXZ = Vector2.Transform(moveXZ, Matrix.CreateRotationZ(yaw));
            //    move = new Vector3(moveXZ.X, move.Y, moveXZ.Y);
            //    pos += move;
            //}

            //view *= Matrix.CreateRotationY(0.01f);
            //Vector2 mbPos = Mouse.GetState().Position.ToVector2();

            //if (mbPos != Input.mbPos)
            //{
            //    Console.WriteLine("y");
            //}

            Vector2 mouseMove = Input.mbPos - mbPosPast;// new Vector2(resX, resY) / 2f;
            mouseMove *= mouseSensitivity;
            //view *= Matrix.CreateFromYawPitchRoll(mouseMove.X, mouseMove.Y, 0);

            //pos += new Vector3(mouseMove.X, 0, mouseMove.Y);

            mouseMoveSave += (mouseMove - mouseMoveSave) * 0.5f;

            yaw += mouseMoveSave.X;
            pitch += mouseMoveSave.Y;

            if (Math.Abs(pitch) > MathHelper.PiOver2 - 0.1f)
                pitch = Math.Sign(pitch) * (MathHelper.PiOver2 - 0.1f);

            mbPosPast = Input.mbPos;

            if (mouseMove != Vector2.Zero)
            {
                int border = 10;
                M_Rectangle rect = new M_Rectangle(border, border, resX - border * 2, resY - border * 2);
                if (!rect.ColVector(Input.mbPos))
                    CenterMouse();
                //Console.WriteLine("x");
            }
            else
            { }//Console.WriteLine("m");

            UpdateView();

        }

        public void ResChanged(int resX, int resY)
        {
            this.resX = resX;
            this.resY = resY;

            if (perspective)
                projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClip, farClip);
            else
                projection = Matrix.CreateOrthographicOffCenter(0,resX, resY,0, nearClip, farClip);
            CenterMouse();

        }

        public void CenterMouse(bool force = false)
        {
            if (force)// || Game1.isActive
            {
                //Input.mbPos = new Vector2(Game1.resX, Game1.resY) / 2f;
                //mbPosPast = Input.mbPos;

                Mouse.SetPosition(resX / 2, resY / 2);
                mbPosPast = Mouse.GetState().Position.ToVector2();
            }
        }

        Vector3 currentView;

        public Vector3 GetViewDir()
        {
            Vector3 targetView = Vector3.Transform(Vector3.UnitZ, Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(-yaw));
            currentView += (targetView - currentView) * 0.25f; //sanity
            return currentView;
        }

        private void UpdateView()
        {
            Vector3 camPos = GetRealCamPos();
            Vector3 lookAt = camPos + GetViewDir();
            Vector3 upTransf = Vector3.Transform(up, Matrix.CreateRotationZ(roll) * Matrix.CreateRotationY(-yaw));
            view = Matrix.CreateLookAt(camPos, lookAt, upTransf);
            //view = Matrix.CreateTranslation(pos) * Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

            //float factor = (float)Math.Sin((Game1.frame % 600) / 600f * MathHelper.TwoPi);

            //projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClip, farClip) * Matrix.CreateRotationZ(factor * 0.1f);

            //fieldOfView = MathHelper.PiOver4 + (float)Math.Sin((Game1.frame % 600) / 600f * MathHelper.TwoPi) * 0.1f;
            //float ratio = aspectRatio + aspectRatio * ((float)Math.Sin((Game1.frame % 600) / 600f * MathHelper.TwoPi)) * 0.1f;
            //projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, ratio, nearClip, farClip);
        }

        public Vector3 GetRealCamPos()
        {
            //Vector3 camPos = new Vector3(0, O_Player.height, 0);
            //camPos = Vector3.Transform(camPos, Matrix.CreateRotationZ(roll) * Matrix.CreateRotationY(-yaw));
            //camPos += new Vector3(pos.X, pos.Y - O_Player.height, pos.Z);
            return pos;

        }

        public Vector2 GetHeadLeaningDirection(float height, float cRoll)
        {
            Vector3 camPos = new Vector3(0, height, 0);
            camPos = Vector3.Transform(camPos, Matrix.CreateRotationZ(cRoll) * Matrix.CreateRotationY(-yaw));
            return new Vector2(camPos.X, camPos.Z);
        }

    }
}
