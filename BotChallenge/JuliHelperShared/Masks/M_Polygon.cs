using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class M_Polygon : Mask
    {
        public List<Vector2> vertices;

        public bool closed, startCorner, endCorner; //start and end corner when polygon is open

        public M_Polygon(Vector2 pos, List<Vector2> vertices, bool closed = true)
        {
            this.pos = pos;
            this.vertices = vertices;
            this.closed = closed;
            startCorner = endCorner = true;
        }

        public override Vector2 GetSize()
        {
            return GetRectangle().size;
        }

        public override Vector2 GetSizeTransformed()
        {
            return GetSize();
        }

        public Mask ApplyPosition()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] += pos;
            }
            pos = Vector2.Zero;
            return this;
        }

        public override Vector2 GetRealPos() //TODO: check if this function was used before 2017 and if it is still working old: return pos;
        {
            return pos;
            //Vector2 min = Vector2.Zero;
            //if (vertices.Count > 0)
            //{
            //    min = vertices[0];
            //    for (int i = 1; i < vertices.Count; i++)
            //    {
            //        if (vertices[i].X < min.X)
            //            min.X = vertices[i].X;
            //        if (vertices[i].Y < min.Y)
            //            min.Y = vertices[i].Y;
            //    }
            //}
            //return pos + min;
        }

        public override M_Rectangle GetNoMatrixRectangle()
        {
            return GetRectangle();
        }

        public override M_Rectangle GetRectangle()
        {
            Vector2 minPos, maxPos;
            if (vertices.Count > 0)
            {
                minPos = maxPos = vertices[0];
                for (int i = 1; i < vertices.Count; i++)
                {
                    minPos = Calculate.MinVector(minPos, vertices[i]);
                    maxPos = Calculate.MaxVector(maxPos, vertices[i]);
                }

                return new M_Rectangle(pos + minPos, maxPos - minPos);
            }
            else
                return new M_Rectangle(pos, Vector2.Zero);
        }

        public List<Vector2> GetEdges()
        {
            List<Vector2> edges = new List<Vector2>();
            if (vertices.Count > 1)
            {
                for (int i = 0; i < vertices.Count - 1; i++)
                    edges.Add(vertices[i + 1] - vertices[i]);

                //if (vertices.Count > 2)
                if (closed)
                    edges.Add(vertices[0] - vertices[vertices.Count - 1]);
            }
            return edges;
        }

        public List<Vector2> GetClosedEdges()
        {
            List<Vector2> edges = new List<Vector2>();
            if (vertices.Count > 1)
            {
                for (int i = 0; i < vertices.Count - 1; i++)
                    edges.Add(vertices[i + 1] - vertices[i]);
                edges.Add(vertices[0] - vertices[vertices.Count - 1]);
            }
            return edges;
        }

        public override void Draw(SpriteBatch spriteBatch, Color color, float depth = 0f)
        {
            Draw(color);

            if (!closed)
            {
                if (startCorner)
                    DrawM.Sprite.DrawCircle(spriteBatch, pos + vertices[0], 2f / DrawM.scale, Color.Red, 0.1f);
                if (endCorner)
                    DrawM.Sprite.DrawCircle(spriteBatch, pos + vertices[1], 2f / DrawM.scale, Color.Red, 0.1f);
            }
        }

        public void Draw(Color color)
        {
            //if (!closed)
            //    color = Color.Yellow;

            if (vertices.Count > 1)
            {
                if (closed)
                    DrawM.Vertex.DrawPolygon(pos, vertices, color);
                else
                    DrawM.Vertex.DrawPolygonOutlineOpen(pos, vertices, color);
            }
            //else
            //    DrawM.Sprite.DrawCircle(spriteBatch, pos, 2, color, depth);



            //else if (vertices.Count == 1)
            //spriteBatch.Draw(DrawM.pixel, new Rectangle((int)pos.X, (int)pos.Y, 4, 4), null, color, 0f, Vector2.Zero, SpriteEffects.None, depth); 
        
        }
        public void DrawOutline(Color color)
        {
            if (closed)
                DrawM.Vertex.DrawPolygonOutlineClosed(pos, vertices, color);
            else
                DrawM.Vertex.DrawPolygonOutlineOpen(pos, vertices, color);
        }
        public void DrawSpriteBatch(SpriteBatch spriteBatch, Color color)
        {
            ////if (!closed)
            ////    color = Color.Yellow;

            //if (vertices.Count > 2)
            //{
            //    if (closed)
            //        DrawM.Vertex.DrawPolygon(pos, vertices, color);
            //    else
            //    {
            //        for (int i = 0; i < vertices.Count - 1; i++)
            //        {
            //            DrawM.Sprite.DrawLine(spriteBatch, pos + vertices[i], pos + vertices[i + 1], color, 2f / DrawM.zoom);
            //        }
            //    }
            //}
            //else 
            if (vertices.Count >= 2)
            {
                if (!closed)
                {
                    if (startCorner)
                        DrawM.Sprite.DrawCircle(spriteBatch, pos + vertices[0], 2f / DrawM.scale, Color.Yellow * 0.5f, 0.1f);
                    if (endCorner)
                        DrawM.Sprite.DrawCircle(spriteBatch, pos + vertices[vertices.Count - 1], 2f / DrawM.scale, Color.Red * 0.5f, 0.1f);
                }
            }
            //else if (vertices.Count == 1)
            //    DrawM.Sprite.DrawCircle(spriteBatch, pos, 2, color, depth);
            ////spriteBatch.Draw(DrawM.pixel, new Rectangle((int)pos.X, (int)pos.Y, 4, 4), null, color, 0f, Vector2.Zero, SpriteEffects.None, depth); 

        }

        public M_Polygon Transform(Matrix transform)
        {
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = Vector2.Transform(vertices[i], transform);

            return this;
        }

        public void RotateDegrees(float angle)
        {
            if (angle < 0)
                angle = 360 + angle % 360;
            if (angle > 360)
                angle = angle % 360;

            if (angle % 90 == 0)
            {
                //right angle rotation
                if (angle == 90)
                    for (int i = 0; i < vertices.Count; i++)
                        vertices[i] = new Vector2(-vertices[i].Y, vertices[i].X);
                if (angle == 180)
                    for (int i = 0; i < vertices.Count; i++)
                        vertices[i] = new Vector2(-vertices[i].X, -vertices[i].Y);
                if (angle == 270)
                    for (int i = 0; i < vertices.Count; i++)
                        vertices[i] = new Vector2(vertices[i].Y, -vertices[i].X);
            }
            else
                RotateRadians(angle * MathHelper.TwoPi / 360f);
        }
        public void RotateDegrees(float angle, Vector2 center)
        {
            if (angle < 0)
                angle = 360 + angle % 360;
            if (angle > 360)
                angle = angle % 360;

            if (angle % 90 == 0)
            {
                //right angle rotation
                if (angle == 90)
                    for (int i = 0; i < vertices.Count; i++)
                        vertices[i] = center + new Vector2(-vertices[i].Y + center.Y, vertices[i].X - center.X);
                if (angle == 180)
                    for (int i = 0; i < vertices.Count; i++)
                        vertices[i] = center + new Vector2(-vertices[i].X + center.X, -vertices[i].Y + center.Y);
                if (angle == 270)
                    for (int i = 0; i < vertices.Count; i++)
                        vertices[i] = center + new Vector2(vertices[i].Y - center.Y, -vertices[i].X + center.X);
            }
            else
                RotateRadians(angle * MathHelper.TwoPi / 360f, center);
        }
        public void RotateRadians(float angle)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = new Vector2((float)(Math.Cos(angle) * vertices[i].X - Math.Sin(angle) * vertices[i].Y),
                                            (float)(Math.Cos(angle) * vertices[i].Y + Math.Sin(angle) * vertices[i].X));
            }
        }
        public void RotateRadians(float angle, Vector2 center)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] -= center;
                vertices[i] = center + new Vector2((float)(Math.Cos(angle) * vertices[i].X  - Math.Sin(angle) * vertices[i].Y),
                                            (float)(Math.Cos(angle) * vertices[i].Y + Math.Sin(angle) * vertices[i].X));
            }
        }
        public void Flip(bool horizontally, bool vertically)
        {
            Flip(horizontally, vertically, GetCenter() - pos);
        }
        public void Flip(bool horizontally, bool vertically, Vector2 center)
        {
            if (horizontally)
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] = new Vector2(2 * center.X - vertices[i].X, vertices[i].Y);
            if (vertically)
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] = new Vector2(vertices[i].X, 2 * center.Y - vertices[i].Y);
        }
        public void Scale(Vector2 scale)
        {
            Scale(scale, Vector2.Zero);
        }
        public void Scale(Vector2 scale, Vector2 center)
        {
            if (Math.Sign(scale.X * scale.Y) == -1)
            {
                //invert vertice order
                List<Vector2> oldVertices = vertices.ToList();
                int j = vertices.Count - 1;
                for (int i = 0; i < vertices.Count; i++, j--)
                    vertices[i] = center + (oldVertices[j] - center) * scale;
            }
            else
            {
                //normal order
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] = center + (vertices[i] - center) * scale;
            }

        }


        public override Mask Clone()
        {
            M_Polygon clone = (M_Polygon)this.MemberwiseClone();
            clone.vertices = vertices.ToList();
            return clone;
        }


        public static M_Polygon GetRandomConvex(Vector2 pos, Random rand, float radius, float angleMin, float angleRange)
        {
            M_Polygon poly = new M_Polygon(pos, new List<Vector2>());

            float startAngle = (float)rand.NextDouble() * MathHelper.TwoPi;

            for (float a = 0f; a < MathHelper.TwoPi; )         // full circle
            {
                poly.vertices.Add(new Vector2((float)Math.Cos(startAngle + a) * radius, (float)Math.Sin(startAngle + a) * radius));
                a += (float)((angleMin + (angleRange * rand.NextDouble())) * Math.PI / 180f);
            }

            return poly;
        }
        public static M_Polygon GetRandomConvex(Vector2 pos, Random rand, float radius)
        {
            int maxAngle = rand.Next(45, 360 / 3);
            int minAngle = rand.Next(maxAngle);
            return GetRandomConvex(pos, rand, radius, minAngle, maxAngle - minAngle);
        }

        public static M_Polygon GetCircleOpen(Vector2 pos, float radius, int vertices)
        {
            M_Polygon poly = new M_Polygon(pos, new List<Vector2>());
            for (int i = 0; i < vertices; i++)
            {
                float a = i * MathHelper.TwoPi / vertices;
                poly.vertices.Add(new Vector2((float)Math.Cos(a) * radius, (float)Math.Sin(a) * radius));
            }
            return poly;
        }
        public static M_Polygon GetCircleClosed(Vector2 pos, float radius, int vertices)
        {
            M_Polygon poly = GetCircleOpen(pos, radius, vertices - 1);
            poly.vertices.Add(poly.vertices[0]);
            return poly;
		}
		public static M_Polygon GetCirclePart(Vector2 pos, float radius, float angle, float fov, int vertices)
		{
			M_Polygon poly = new M_Polygon(pos, new List<Vector2>() { Vector2.Zero });
			float a = angle - fov / 2f;
			float plus = fov / (vertices - 1);
			for (int i = 0; i < vertices; i++)
			{
				poly.vertices.Add(new Vector2((float)Math.Cos(a) * radius, (float)Math.Sin(a) * radius));
				a += plus;
			}
			return poly;
		}
		/*public static M_Polygon GetCircle(Vector2 pos, float radius, int vertices)
        {
            M_Polygon poly = new M_Polygon(pos, new List<Vector2>());
            
            float anglePlus = (float)(MathHelper.TwoPi / (float)vertices);

            for (float a = 0f; a < MathHelper.TwoPi; a += anglePlus)         // full circle
            {
                poly.vertices.Add(new Vector2((float)Math.Cos(a) * radius, (float)Math.Sin(a) * radius));
            }

            return poly;
        }*/

		#region Collision

		#region mask

		public override bool ColMask(Mask mask)
        {
            return mask.ColPolygon(this);
        }
        public override CollisionResult DistToMask(Mask mask, Vector2 dir)
        {
            return mask.DistToPolygon(this, -dir).GetAxisInvert();
        }
        public override CollisionResult DistToMask(Mask mask)
        {
            return mask.DistToPolygon(this).GetAxisInvert();
        }

        #endregion

        #region vector

        public override bool ColVector(Vector2 vec)
        {
            return Collision.ColVectorPolygon(vec, this);
        }
        public override CollisionResult DistToVector(Vector2 vec, Vector2 dir)
        {
            return Collision.DistVectorPolygon(vec, this, -dir).GetAxisInvert();
        }
        public override CollisionResult DistToVector(Vector2 vec)
        {
            return Collision.DistVectorPolygon(vec, this).GetAxisInvert();
        }

        #endregion

        #region rectangle

        public override bool ColRectangle(M_Rectangle rectangle)
        {
            return Collision.ColRectanglePolygon(rectangle, this);
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle, Vector2 dir)
        {
            return Collision.DistRectanglePolygon(rectangle, this, -dir).GetAxisInvert();
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle)
        {
            return Collision.DistRectanglePolygon(rectangle, this).GetAxisInvert();
        }

        #endregion

        #region polygon

        public override bool ColPolygon(M_Polygon polygon)
        {
            return Collision.ColPolygonPolygon(this, polygon);
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon, Vector2 dir)
        {
            //if (Input.rightShift.down)
            //    return Collision.DistPolygonPolygon2(this, polygon, dir);
            //else
                return Collision.DistPolygonPolygon(this, polygon, dir);
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon)
        {
            return Collision.DistPolygonPolygon(this, polygon);
        }

        #endregion

        #region circle

        public override bool ColCircle(M_Circle circle)
        {
            return Collision.ColPolygonCircle(this, circle);
        }
        public override CollisionResult DistToCircle(M_Circle circle, Vector2 dir)
        {
            return Collision.DistPolygonCircle(this, circle, dir);
        }
        public override CollisionResult DistToCircle(M_Circle circle)
        {
            return Collision.DistPolygonCircle(this, circle);
        }

        #endregion

        #region sprite

        public override bool ColSprite(Sprite sprite)
        {
            return Collision.ColPolygonSprite(this, sprite);
        }

        #endregion

        #endregion

    }
}
