using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace JuliHelper.Camera
{
    public class Camera
    {
        public Matrix matrix;
        public float targetZoom, resetZoom, targetRotation, resetRotation, moveSpeed, rotationSpeed, zoomSpeed, arrowMoveSpeed, zoom, rotation, zoomStep;
        public float? minZoom, maxZoom, zoomSpeedOut;
        public Vector2 pos, targetPos, resetPos, mousePos, mousePosPast, centerOffset, posRelative;
        public M_Rectangle view;
        public M_Rectangle roomBoundary;
        public float boundaryFactor; //thickness of the boundary which the camera is inside (between -1 and 1; 1 = inside, -1 = outside)

        public bool zoomControl, middleMouseMoveControl, centered, zoomControlToMouse, arrowMoveControl, boundsMouseMoveControl, partRotation, zoomIncrementWithShift, zoomUseWholeNumbersOverOne, resetWithDoubleMiddleMouse;

        public Func<bool> boundsMoveFunc;

        private int resX, resY;

        public Camera()
        {
            Initialize();
        }

        public Camera(int _resX, int _resY)
        {
            Initialize();
            UpdateEnd(_resX, _resY);
        }

        private void Initialize()
        {
            centered = zoomControlToMouse = partRotation = true;

            arrowMoveSpeed = 4f;
            moveSpeed = 1f;
            rotationSpeed = 1f;
            zoomSpeed = 1f;
            zoomStep = 1.5f;
            resetZoom = 1f;

            posRelative = Vector2.Zero;

            boundsMoveFunc = null;

            minZoom = maxZoom = null;

            boundaryFactor = 1;

            ResetTransform();
        }

        public void ResetTransform()
        {
            targetPos = pos = centerOffset = resetPos;
            targetZoom = zoom = resetZoom;
            targetRotation = rotation = resetRotation;

            UpdateMatrix();
        }

        public void UpdateBegin()
        {
            mousePosPast = Vector2.Transform(Input.mbPosPast, Matrix.Invert(matrix));
            mousePos = Vector2.Transform(Input.mbPos, Matrix.Invert(matrix));
        }

        public void UpdateEnd(int _resX, int _resY)
        {
            resX = _resX;
            resY = _resY;

            if (zoomControl)
            {
                float cZoom = 1f;

                if (Input.mbWheel > 0)
                {
                    if (!minZoom.HasValue || targetZoom > minZoom)
                    {
                        if ((!zoomIncrementWithShift || !Input.leftShift.down || targetZoom < 2) && (!zoomUseWholeNumbersOverOne || targetZoom < 2))
                            cZoom = 1 / zoomStep;
                        else
                            cZoom = (targetZoom - 1) / targetZoom;
                    }
                }
                else if (Input.mbWheel < 0)
                {
                    if (!maxZoom.HasValue || targetZoom < maxZoom)
                    {
                        if ((!zoomIncrementWithShift || !Input.leftShift.down || targetZoom < 1) && (!zoomUseWholeNumbersOverOne || targetZoom < 1))
                            cZoom = zoomStep;
                        else
                            cZoom = (targetZoom + 1) / targetZoom;
                    }
                }

                if (cZoom != 1)
                {
                    if (zoomControlToMouse)
                    {
                        Vector2 mouseDist = mousePos - pos;
                        mouseDist /= cZoom;
                        pos = targetPos = mousePos - mouseDist;
                    }

                    targetZoom *= cZoom;
                }
            }


            if (middleMouseMoveControl && Input.mbMiddle.down)
            {
                Vector2 camMove = mousePos - mousePosPast;

                targetPos -= camMove;
            }

            if (resetWithDoubleMiddleMouse)
            {
                if (Input.mbMiddle.pressedDouble)
                {
					ResetTransform();
                }
            }

            if (arrowMoveControl && !Input.leftControl.down)
            {
                if (Input.left.down)
                    targetPos.X -= arrowMoveSpeed / zoom;
                if (Input.right.down)
                    targetPos.X += arrowMoveSpeed / zoom;
                if (Input.up.down)
                    targetPos.Y -= arrowMoveSpeed / zoom;
                if (Input.down.down)
                    targetPos.Y += arrowMoveSpeed / zoom;
            }

            if (boundsMouseMoveControl && (boundsMoveFunc == null || boundsMoveFunc()))
            {
                if (Input.mbPos.X <= 0)
                    targetPos.X -= arrowMoveSpeed / zoom;
                if (Input.mbPos.X >= resX - 1)
                    targetPos.X += arrowMoveSpeed / zoom;
                if (Input.mbPos.Y <= 0)
                    targetPos.Y -= arrowMoveSpeed / zoom;
                if (Input.mbPos.Y >= resY - 1)
                    targetPos.Y += arrowMoveSpeed / zoom;
            }


            #region pos

            Vector2 move = targetPos - pos;
            pos += move * moveSpeed;

            #endregion

            float dist;

            #region rotation

            if (targetRotation < 0)
                targetRotation = MathHelper.TwoPi - (-targetRotation % MathHelper.TwoPi);

            dist = targetRotation - rotation;
            if (Math.Abs(dist) > Math.PI)
                dist = -Math.Sign(dist) * ((float)MathHelper.TwoPi - Math.Abs(dist));

            if (partRotation)
            {
                if (rotationSpeed < 1)
                {
                    rotation += dist * rotationSpeed;

                    if (rotation < 0)
                        rotation = MathHelper.TwoPi - (-rotation % MathHelper.TwoPi);
                    else if (rotation > MathHelper.TwoPi)
                        rotation = (rotation % MathHelper.TwoPi);
                }
                else
                    rotation = targetRotation;
            }
            else
            {
                if (dist != 0)
                {
                    rotation += rotationSpeed * Math.Sign(dist);

                    if (rotation < 0)
                        rotation = MathHelper.TwoPi - (-rotation % MathHelper.TwoPi);
                    else if (rotation > MathHelper.TwoPi)
                        rotation = (rotation % MathHelper.TwoPi);
                    
                    if (Math.Sign(Calculate.AngleDistance(rotation, targetRotation)) != Math.Sign(dist))
                        rotation = targetRotation;
                }
            }

            #endregion

            #region zoom

            if (minZoom.HasValue && targetZoom < minZoom.Value)
                targetZoom = minZoom.Value;
            if (maxZoom.HasValue && targetZoom > maxZoom.Value)
                targetZoom = maxZoom.Value;

            if (zoomSpeed < 1)
            {
                float dz = (targetZoom / zoom);

                if (dz > 1 || !zoomSpeedOut.HasValue)
                    dz = (float)Math.Pow(dz, zoomSpeed);
                else
                    dz = (float)Math.Pow(dz, zoomSpeedOut.Value);
                zoom *= dz;

                //dist = targetZoom - zoom;
                //zoom += dist * zoomSpeed;
            }
            else
                zoom = targetZoom;

            #endregion

            if (roomBoundary != null)
                SetCameraInBoundary();

            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            float roundZoom = (float)Math.Round(zoom * 1000f) / 1000f;

            Vector2 pos = this.pos + posRelative;

            matrix = Matrix.CreateTranslation(new Vector3(Calculate.RoundVector(-pos * roundZoom) / roundZoom, 0))
                    * Matrix.CreateScale(roundZoom, roundZoom, 1f)
                    * Matrix.CreateRotationZ(rotation);

            if (centered)
                    matrix *= Matrix.CreateTranslation(new Vector3(Calculate.RoundVector((new Vector2(resX, resY) / 2f + centerOffset / 2f) * roundZoom) / roundZoom, 0));

            UpdateView();
        }

        private void UpdateView()
        {
            //Vector2 p1 = pos - new Vector2(resX, resY) / 2f / zoom;
            //Vector2 p2 = pos + new Vector2(resX, resY) / 2f / zoom;

            //float scale = 0.5f;
            float border = 0;//256
            M_Polygon poly = new M_Polygon(Vector2.Zero, new List<Vector2>()
            {
                new Vector2(-border,-border),
                new Vector2(resX + border, -border),
                new Vector2(resX + border, resY + border),
                new Vector2(-border, resY + border)
            });

            poly.Transform(Matrix.Invert(matrix));

            float minX = poly.vertices[0].X;
            float maxX = poly.vertices[0].X;
            float minY = poly.vertices[0].Y;
            float maxY = poly.vertices[0].Y;

            for (int i = 1; i < poly.vertices.Count; i++)
            {
                if (poly.vertices[i].X < minX)
                    minX = poly.vertices[i].X;
                if (poly.vertices[i].X > maxX)
                    maxX = poly.vertices[i].X;
                if (poly.vertices[i].Y < minY)
                    minY = poly.vertices[i].Y;
                if (poly.vertices[i].Y > maxY)
                    maxY = poly.vertices[i].Y;
            }

            //minX = Math.Max(0, minX);
            //minY = Math.Max(0, minY);
            //maxX = Math.Min(mapW - 1, maxX);
            //maxY = Math.Min(mapH - 1, maxY);

            view = new M_Rectangle(minX, minY, maxX - minX, maxY - minY);

            //view = new M_Rectangle(p1, p2 - p1);
        }

        public Matrix GetNoZoomMatrix()
        {
            float roundZoom = (float)Math.Round(zoom * 1000f) / 1000f;

            Vector2 realPos = pos - new Vector2(resX, resY) / 2;

            //TODO: centered

            return Matrix.CreateTranslation(new Vector3(Calculate.RoundVector(-realPos * roundZoom) / roundZoom, 0))
                   * Matrix.CreateRotationZ(rotation)
                   * Matrix.CreateTranslation(new Vector3(Calculate.RoundVector(new Vector2(resX, resY) / 2f * roundZoom) / roundZoom, 0));
        }
        public Matrix GetRotationMatrix()
        {
            return Matrix.CreateRotationZ(rotation);
        }

        public void JumpToTarget()
        {
            pos = targetPos;
            rotation = targetRotation;
        }

        private void SetCameraInBoundary()
        {
            if (centered)
            {
                UpdateMatrix();
                float width = resX / zoom;
                float height = resY / zoom;
                M_Rectangle myView = new M_Rectangle(pos - new Vector2(width, height) / 2f, new Vector2(width, height));

                float w = myView.size.X / 2f * boundaryFactor;
                float h = myView.size.Y / 2f * boundaryFactor;

                w = Math.Min(w, roomBoundary.size.X / 2f);
                h = Math.Min(h, roomBoundary.size.Y / 2f);

                if (pos.X - w < roomBoundary.Left)
                    pos.X = roomBoundary.Left + w;
                if (pos.X + w > roomBoundary.Right)
                    pos.X = roomBoundary.Right - w;

                if (pos.Y - h < roomBoundary.Top)
                    pos.Y = roomBoundary.Top + h;
                if (pos.Y + h > roomBoundary.Bottom)
                    pos.Y = roomBoundary.Bottom - h;

                targetPos = pos;
            }
        }

        public float GetStepZoom(float zoom)
        {
            float n = (float)Math.Floor(Math.Log(zoom, zoomStep));
            return (float)Math.Pow(zoomStep, n);
        }

        public void CenterCamera(int levelW, int levelH, int displayW, int displayH)
        {
            float ratioX = (float)displayW / levelW;
            float ratioY = (float)displayH / levelH;

            float newZoom = Math.Min(ratioX, ratioY);// / 1.2f;

            zoom = targetZoom = GetStepZoom(newZoom);

            targetPos = new Vector2(levelW, levelH) / 2f;
            JumpToTarget();
        }
    }
}
