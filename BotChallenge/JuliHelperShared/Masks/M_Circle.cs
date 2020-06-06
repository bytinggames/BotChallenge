using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class M_Circle : Mask
    {
        public float radius;

        public M_Circle(Vector2 pos, float radius)
        {
            this.pos = pos;
            this.radius = radius;
        }

        public override Vector2 GetSizeTransformed()
        {
            return GetSize();
        }
        public override Vector2 GetSize()
        {
            return new Vector2(radius * 2f, radius * 2f);
        }

        public override M_Rectangle GetRectangle()
        {
            return new M_Rectangle(pos - new Vector2(radius), new Vector2(radius * 2));
        }

        public override M_Rectangle GetNoMatrixRectangle()
        {
            return GetRectangle();
        }

        public override Vector2 GetRealPos()
        {
            return pos - new Vector2(radius);
        }

        public override void Draw(SpriteBatch spriteBatch, Color color, float depth = 0f)
        {
            DrawM.Sprite.DrawCircle(spriteBatch, pos, radius, color, depth);
        }

        public void Draw(SpriteBatch spriteBatch, Color color, float depth = 0f, float textureScale = 1f)
        {
            DrawM.Sprite.DrawCircle(spriteBatch, pos, radius, color, depth, textureScale);
        }

        //public void Draw(SpriteBatch spriteBatch, Vector2 drawPos, Color color, float depth = 0f, float textureScale = 1f)
        //{
        //    DrawM.Sprite.DrawCircle(spriteBatch, drawPos, radius, color, depth, textureScale);
        //}


        #region Collision

        #region mask

        public override bool ColMask(Mask mask)
        {
            return mask.ColCircle(this);
        }
        public override CollisionResult DistToMask(Mask mask, Vector2 dir)
        {
            return mask.DistToCircle(this, -dir).GetAxisInvert();
        }
        public override CollisionResult DistToMask(Mask mask)
        {
            return mask.DistToCircle(this).GetAxisInvert();
        }

        #endregion

        #region vector

        public override bool ColVector(Vector2 vec)
        {
            return Collision.ColVectorCircle(vec, this);
        }
        public override CollisionResult DistToVector(Vector2 vec, Vector2 dir)
        {
            return Collision.DistVectorCircle(vec, this, -dir).GetAxisInvert();
        }
        public override CollisionResult DistToVector(Vector2 vec)
        {
            return Collision.DistVectorCircle(vec, this).GetAxisInvert();
        }

        #endregion

        #region rectangle

        public override bool ColRectangle(M_Rectangle rectangle)
        {
            return Collision.ColRectangleCircle(rectangle, this);
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle, Vector2 dir)
        {
            return Collision.DistRectangleCircle(rectangle, this, -dir).GetAxisInvert();
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle)
        {
            return Collision.DistRectangleCircle(rectangle, this).GetAxisInvert();
        }

        #endregion

        #region polygon

        public override bool ColPolygon(M_Polygon polygon)
        {
            return Collision.ColPolygonCircle(polygon, this);
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon, Vector2 dir)
        {
            return Collision.DistPolygonCircle(polygon, this, -dir).GetAxisInvert();
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon)
        {
            return Collision.DistPolygonCircle(polygon, this).GetAxisInvert();
        }

        #endregion

        #region circle

        public override bool ColCircle(M_Circle circle)
        {
            return Collision.ColCircleCircle(this, circle);
        }
        public override CollisionResult DistToCircle(M_Circle circle, Vector2 dir)
        {
            return Collision.DistCircleCircle(this, circle, dir);
        }
        public override CollisionResult DistToCircle(M_Circle circle)
        {
            return Collision.DistCircleCircle(this, circle);
        }

        #endregion

        #region sprite
        
        public override bool ColSprite(Sprite sprite)
        {
            return Collision.ColCircleSprite(this, sprite);
        }

        #endregion

        #endregion
    }
}
