using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class M_Point : Mask
    {
        public M_Point(Vector2 pos)
        {
            this.pos = pos;
        }


        public override Vector2 GetSize()
        {
            return Vector2.Zero;
        }

        public override Vector2 GetSizeTransformed()
        {
            return Vector2.Zero;
        }

        public override Vector2 GetRealPos()
        {
            return pos;
        }

        public override M_Rectangle GetNoMatrixRectangle()
        {
            return new M_Rectangle(pos, Vector2.Zero);
        }

        public override M_Rectangle GetRectangle()
        {
            return new M_Rectangle(pos, Vector2.Zero);
        }

        public override void Draw(SpriteBatch spriteBatch, Color color, float depth = 0f)
        {
            DrawM.Sprite.DrawCircle(spriteBatch, pos, 2, color, depth);
        }

        #region Collision

        #region mask

        public override bool ColMask(Mask mask)
        {
            return mask.ColPoint(this);
        }
        public override CollisionResult DistToMask(Mask mask, Vector2 dir)
        {
            return mask.DistToPoint(this, -dir).GetAxisInvert();
        }
        public override CollisionResult DistToMask(Mask mask)
        {
            return mask.DistToPoint(this).GetAxisInvert();
        }

        #endregion

        #region vector

        public override bool ColVector(Vector2 vec)
        {
            return vec == pos;
        }
        public override CollisionResult DistToVector(Vector2 vec, Vector2 dir) //??? is this function helpful?
        {
            Vector2 dist = vec - pos;

            if (dist == Vector2.Zero)
                return new CollisionResult() { collision = true };

            //if (Calculate.XPositive(Vector2.Normalize(dir)) == Calculate.XPositive(Vector2.Normalize(dist)))
            //{
            //    CollisionResult cr = new CollisionResult();
            //    cr.distance = (float)Math.Sqrt(dist.X * dist.X + dist.Y * dist.Y) / dir.Length();
            //    cr.axisCol = Vector2.Normalize(dist);
            //    cr.distanceReversed = cr.distance;
            //    cr.axisColReversed = -cr.axisCol;
            //    return cr;
            //}
            return new CollisionResult();
        }
        public override CollisionResult DistToVector(Vector2 vec) //??? is this function helpful?
        {
            if (vec == pos)
                return new CollisionResult() { collision = true };
            return new CollisionResult();
        }

        #endregion

        #region rectangle

        public override bool ColRectangle(M_Rectangle rectangle)
        {
            return Collision.ColVectorRectangle(pos, rectangle);
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle, Vector2 dir)
        {
            return Collision.DistVectorRectangle(pos, rectangle, dir);
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle)
        {
            return Collision.DistVectorRectangle(pos, rectangle);
        }

        #endregion

        #region polygon

        public override bool ColPolygon(M_Polygon polygon)
        {
            return Collision.ColVectorPolygon(pos, polygon);
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon, Vector2 dir)
        {
            return Collision.DistVectorPolygon(pos, polygon, dir);
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon)
        {
            return Collision.DistVectorPolygon(pos, polygon);
        }

        #endregion

        #region circle

        public override bool ColCircle(M_Circle circle)
        {
            return Collision.ColVectorCircle(pos, circle);
        }
        public override CollisionResult DistToCircle(M_Circle circle, Vector2 dir)
        {
            return Collision.DistVectorCircle(pos, circle, dir);
        }
        public override CollisionResult DistToCircle(M_Circle circle)
        {
            return Collision.DistVectorCircle(pos, circle);
        }

        #endregion
        
        #region sprite

        public override bool ColSprite(Sprite sprite)
        {
            return Collision.ColVectorSprite(pos, sprite);
        }

        #endregion

        #endregion
    }
}
