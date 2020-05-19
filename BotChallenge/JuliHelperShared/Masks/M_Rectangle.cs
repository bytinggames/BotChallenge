using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;

namespace JuliHelper
{
    public class M_Rectangle : Mask
    {
        public Vector2 size;

        public float Width
        {
            get => size.X;
            set => size.X = value;
        }
        public float Height
        {
            get => size.Y;
            set => size.Y = value;
        }
        public M_Rectangle()
        {
        }
        public M_Rectangle(Vector2 pos, Vector2 size)
        {
            Initialize(pos, size);
        }
        public M_Rectangle(float x, float y, float width, float height)
        {
            Initialize(new Vector2(x, y), new Vector2(width, height));
        }
        public M_Rectangle(Rectangle rect)
        {
            Initialize(new Vector2(rect.X, rect.Y), new Vector2(rect.Width, rect.Height));
        }
        public M_Rectangle(Vector2 pos, Vector2 size, Vector2 originNormalized, bool round = false)
        {
            pos -= size * originNormalized;
            if (round)
                pos = pos.RoundVector();
            Initialize(pos, size);
        }
        public M_Rectangle(float x, float y, float width, float height, float originNormalizedX, float originNormalizedY, bool round = false)
        {
            Vector2 size = new Vector2(width, height);
            Vector2 pos = new Vector2(x, y) - size * new Vector2(originNormalizedX, originNormalizedY);
            if (round)
                pos = pos.RoundVector();
            Initialize(pos, size);
        }

        public void Initialize(Vector2 pos, Vector2 size)
        {
            if (size.X < 0)
            {
                pos.X += size.X;
                size.X = -size.X;
            }
            if (size.Y < 0)
            {
                pos.Y += size.Y;
                size.Y = -size.Y;
            }

            this.pos = pos;
            this.size = size;
        }

        public override Vector2 GetSizeTransformed()
        {
            return size;
        }
        public override Vector2 GetSize()
        {
            return size;
        }
        public override M_Rectangle GetRectangle()
        {
            return this;
        }
        public override M_Rectangle GetNoMatrixRectangle()
        {
            return this;
        }

        public override Vector2 GetRealPos()
        {
            return pos;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
        }

        public float Top { get => pos.Y; set => Y = value; }
        public float Left { get => pos.X; set => X = value; }
        public float Right { get => pos.X + size.X; set => X = value - size.X; }
        public float Bottom { get => pos.Y + size.Y; set => Y = value - size.Y; }
        public Vector2 TopLeft { get => pos; set => pos = value; }
        public Vector2 TopRight { get => pos + new Vector2(size.X, 0); set => pos = value - new Vector2(size.X, 0); }
        public Vector2 BottomLeft { get => pos + new Vector2(0, size.Y); set => pos = value - new Vector2(0, size.Y); }
        public Vector2 BottomRight { get => pos + size; set => pos = value - size; }
        public Vector2 TopV { get => new Vector2(pos.X + size.X / 2f, pos.Y); set => pos = value - new Vector2(size.X / 2f, 0); }
        public Vector2 LeftV { get => new Vector2(pos.X, pos.Y + size.Y / 2f); set => pos = value - new Vector2(0, size.Y / 2f); }
        public Vector2 RightV { get => new Vector2(pos.X + size.X, pos.Y + size.Y / 2f); set => pos = value - new Vector2(size.X, size.Y / 2f); }
        public Vector2 BottomV { get => new Vector2(pos.X + size.X / 2f, pos.Y + size.Y); set => pos = value - new Vector2(size.X / 2f, size.Y); }


        public M_Polygon ToPolygon()
        {
            return new M_Polygon(pos, new List<Vector2>() { Vector2.Zero, new Vector2(size.X, 0), size, new Vector2(0, size.Y) });
        }

        public void Transform(Matrix matrix)
        {
            size = Vector2.Transform(pos + size, matrix);
            pos = Vector2.Transform(pos, matrix);
            Initialize(pos, size - pos);
        }

        public static M_Rectangle operator *(M_Rectangle rect, float scale)
        {
            M_Rectangle clone = (M_Rectangle)rect.Clone();
            clone.pos *= scale;
            clone.size *= scale;
            return clone;
        }
        public static M_Rectangle operator *(M_Rectangle rect, Vector2 scale)
        {
            M_Rectangle clone = (M_Rectangle)rect.Clone();
            clone.pos *= scale;
            clone.size *= scale;
            return clone;
        }

        public override string ToString()
        {
            return pos.X + "; " + pos.Y + "; " + size.X + "; " + size.Y;
        }

        public void PushIntoRectangle(M_Rectangle bounds) //no center, if bounds is smaller then this rectangle!
        {
            if (X < bounds.X)
                X = bounds.X;
            else if (Right > bounds.Right)
                X = bounds.Right - size.X;

            if (Y < bounds.Y)
                Y = bounds.Y;
            else if (Bottom > bounds.Bottom)
                Y = bounds.Bottom - size.Y;
        }

        public Vector2 VectorPushInto(Vector2 pos) //no center, if bounds is smaller then this rectangle!
        {
            if (pos.X < X)
                pos.X = X;
            else if (pos.X > Right)
                pos.X = Right;

            if (pos.Y < Y)
                pos.Y = Y;
            else if (pos.Y > Bottom)
                pos.Y = Bottom;

            return pos;
        }


        public M_Rectangle Enlarge(float grow)
        {
            pos -= new Vector2(grow);
            size += new Vector2(grow * 2);
            return this;
        }
        public M_Rectangle Enlarge(float growX, float growY) => Enlarge(new Vector2(growX, growY));
        public M_Rectangle Enlarge(Vector2 grow)
        {
            pos -= grow;
            size += grow * 2;
            return this;
        }

        #region Collision

        #region mask

        public override bool ColMask(Mask mask)
        {
            return mask.ColRectangle(this);
        }
        public override CollisionResult DistToMask(Mask mask, Vector2 dir)
        {
            return mask.DistToRectangle(this, -dir).GetAxisInvert();
        }
        public override CollisionResult DistToMask(Mask mask)
        {
            return mask.DistToRectangle(this).GetAxisInvert();
        }

        #endregion

        #region vector

        public override bool ColVector(Vector2 vec)
        {
            return Collision.ColVectorRectangle(vec, this);
        }
        public override CollisionResult DistToVector(Vector2 vec, Vector2 dir)
        {
            return Collision.DistVectorRectangle(vec, this, -dir).GetAxisInvert();
        }
        public override CollisionResult DistToVector(Vector2 vec)
        {
            return Collision.DistVectorRectangle(vec, this).GetAxisInvert();
        }

        #endregion

        #region rectangle

        public override bool ColRectangle(M_Rectangle rectangle)
        {
            return Collision.ColRectangleRectangle(this, rectangle);
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle, Vector2 dir)
        {
            return Collision.DistRectangleRectangle(this, rectangle, dir);
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle)
        {
            return Collision.DistRectangleRectangle(this, rectangle);
        }

        #endregion

        #region polygon

        public override bool ColPolygon(M_Polygon polygon)
        {
            return Collision.ColRectanglePolygon(this, polygon);
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon, Vector2 dir)
        {
            return Collision.DistRectanglePolygon(this, polygon, dir);
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon)
        {
            return Collision.DistRectanglePolygon(this, polygon);
        }

        #endregion

        #region circle

        public override bool ColCircle(M_Circle circle)
        {
            return Collision.ColRectangleCircle(this, circle);
        }
        public override CollisionResult DistToCircle(M_Circle circle, Vector2 dir)
        {
            return Collision.DistRectangleCircle(this, circle, dir);
        }
        public override CollisionResult DistToCircle(M_Circle circle)
        {
            return Collision.DistRectangleCircle(this, circle);
        }

        #endregion

        #region sprite

        public override bool ColSprite(Sprite sprite)
        {
            return Collision.ColRectangleSprite(this, sprite);
        }

        #endregion

        #endregion

        public M_Rectangle Expand(Vector2 expand)
        {
            return Expand(expand.X, expand.Y);
        }
        public M_Rectangle Expand(float x, float y)
        {
            if (x > 0)
                size.X += x;
            else if (x < 0)
            {
                _pos.X += x;
                size.X -= x;
            }

            if (y > 0)
                size.Y += y;
            else if (y < 0)
            {
                _pos.Y += y;
                size.Y -= y;
            }
            return this;
        }

        public void SetCenter(Vector2 center)
        {
            pos = center - size / 2f;
        }

        public void SetPosByOriginNormalized(Vector2 newPos, Vector2 origin)
        {
            pos = newPos - origin * size;
        }

        public void SetToAnchor(Anchor anchor, bool round = true)
        {
            pos = anchor.pos - size * anchor.origin;
            if (round)
                pos = pos.RoundVector();
            Initialize(pos, size);
        }
        public void SetToAnchor(Anchor anchor, Vector2 size, bool round = true)
        {
            pos = anchor.pos - size * anchor.origin;
            if (round)
                pos = pos.RoundVector();
            Initialize(pos, size);
        }
        public Anchor GetCenterAnchor()
        {
            return new Anchor(GetCenter());
        }
    }
}
