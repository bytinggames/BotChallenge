using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class M_Collection : Mask
    {
        public List<Mask> masks;

        public M_Collection(Vector2 pos, List<Mask> masks)
        {
            this.masks = masks;
            //for (int i = 0; i < masks.Count; i++)
            //    masks[i].pos += pos;
            this.pos = pos;
        }

        public override Vector2 GetSize()
        {
            Vector2 size = Vector2.Zero;
            for (int i = 0; i < masks.Count; i++)
            {
                Vector2 cSize = masks[i].GetSize();
                if (i == 0 || cSize.X > size.X)
                    size.X = cSize.X;
                if (i == 0 || cSize.Y > size.Y)
                    size.Y = cSize.Y;
            }

            return size;
        }

        public override Vector2 GetSizeTransformed()
        {
            Vector2 size = Vector2.Zero;
            for (int i = 0; i < masks.Count; i++)
            {
                Vector2 cSize = masks[i].GetSizeTransformed();
                if (i == 0 || cSize.X > size.X)
                    size.X = cSize.X;
                if (i == 0 || cSize.Y > size.Y)
                    size.Y = cSize.Y;
            }
            return size;
        }

        public override Vector2 GetRealPos()
        {
            Vector2 pos = Vector2.Zero;
            for (int i = 0; i < masks.Count; i++)
            {
                Vector2 cPos = masks[i].GetRealPos();
                if (i == 0 || cPos.X < pos.X)
                    pos.X = cPos.X;
                if (i == 0 || cPos.Y < pos.Y)
                    pos.Y = cPos.Y;
            }

            return pos; //+this.pos;
        }

        public override M_Rectangle GetNoMatrixRectangle()
        {
            M_Rectangle rect = new M_Rectangle(0, 0, 0, 0);
            for (int i = 0; i < masks.Count; i++)
            {
                M_Rectangle cRect = masks[i].GetNoMatrixRectangle();
                if (i == 0)
                    rect = cRect;
                else
                {
                    if (cRect.pos.X < rect.pos.X)
                        rect.X = cRect.pos.X;
                    if (cRect.pos.Y < rect.pos.Y)
                        rect.Y = cRect.pos.Y;
                    if (cRect.size.X > rect.size.X)
                        rect.size.X = cRect.size.X;
                    if (cRect.size.Y > rect.size.Y)
                        rect.size.Y = cRect.size.Y;
                }
            }
            //rect.pos += this.pos;
            return rect;
        }

        public override M_Rectangle GetRectangle()
        {
            Vector2 min = Vector2.Zero, max = Vector2.Zero;

            for (int i = 0; i < masks.Count; i++)
            {
                M_Rectangle cRect = masks[i].GetRectangle();
                if (i == 0)
                {
                    min = cRect.pos;
                    max = cRect.BottomRight;
                }
                else
                {
                    if (cRect.X < min.X)
                        min.X = cRect.X;
                    if (cRect.Right > max.X)
                        max.X = cRect.Right;
                    if (cRect.Y < min.Y)
                        min.Y = cRect.Y;
                    if (cRect.Bottom > max.Y)
                        max.Y = cRect.Bottom;
                }
            }
            M_Rectangle rect = new M_Rectangle(min, max - min);
            //rect.pos += this.pos;
            return rect;
        }

        public override Mask Clone()
        {
            M_Collection clone = (M_Collection)this.MemberwiseClone();
            clone.masks = new List<Mask>();
            for (int i = 0; i < masks.Count; i++)
                clone.masks.Add(masks[i].Clone());
            return clone;
        }

        public override Vector2 pos
        {
            get
            {
                return _pos;
            }
            set
            {
                Vector2 move = value - _pos;
                for (int i = 0; i < masks.Count; i++)
                    masks[i].pos += move;

                _pos = value;
            }
        }

        public override float X
        {
            get
            {
                return _pos.X;
            }
            set
            {
                float move = value - _pos.X;
                for (int i = 0; i < masks.Count; i++)
                    masks[i].X += move;

                _pos.X = value;
            }
        }

        public override float Y
        {
            get
            {
                return _pos.Y;
            }
            set
            {
                float move = value - _pos.Y;
                for (int i = 0; i < masks.Count; i++)
                    masks[i].Y += move;

                _pos.Y = value;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Color color, float depth = 0f)
        {
            for (int i = 0; i < masks.Count; i++)
                masks[i].Draw(spriteBatch, color, depth);
        }

        public void Add(Mask mask)
        {
            masks.Add(mask);
            mask.pos += pos;
        }

        #region Collision

        public override bool ColMask(Mask mask)
        {
            return masks.Any(f => f.ColMask(mask));
        }
        public override CollisionResult DistToMask(Mask mask, Vector2 dir)
        {
            return MinCol(f => f.DistToMask(mask, dir));
        }
        public override CollisionResult DistToMask(Mask mask)
        {
            return new CollisionResult() { collision = masks.Any(f => f.ColMask(mask)) };
            //return MaxCol(f => f.DistToMask(mask));
        }

        public override bool ColVector(Vector2 vec)
        {
            return masks.Any(f => f.ColVector(vec));
        }
        public override CollisionResult DistToVector(Vector2 vec, Vector2 dir)
        {
            return MinCol(f => f.DistToVector(vec, dir));
        }
        public override CollisionResult DistToVector(Vector2 vec)
        {
            return new CollisionResult() { collision = masks.Any(f => f.ColVector(vec)) };
            //return MaxCol(f => f.DistToVector(vec));
        }

        public override bool ColRectangle(M_Rectangle rectangle)
        {
            return masks.Any(f => f.ColRectangle(rectangle));
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle, Vector2 dir)
        {
            return MinCol(f => f.DistToRectangle(rectangle, dir));
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle)
        {
            return new CollisionResult() { collision = masks.Any(f => f.ColRectangle(rectangle)) };
            //return MaxCol(f => f.DistToRectangle(rectangle));
        }

        public override bool ColPolygon(M_Polygon polygon)
        {
            return masks.Any(f => f.ColPolygon(polygon));
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon, Vector2 dir)
        {
            return MinCol(f => f.DistToPolygon(polygon, dir));
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon)
        {
            return new CollisionResult() { collision = masks.Any(f => f.ColPolygon(polygon)) };
            //return MaxCol(f => f.DistToPolygon(polygon));
        }

        public override bool ColCircle(M_Circle circle)
        {
            return masks.Any(f => f.ColCircle(circle));
        }
        public override CollisionResult DistToCircle(M_Circle circle, Vector2 dir)
        {
            return MinCol(f => f.DistToCircle(circle, dir));
        }
        public override CollisionResult DistToCircle(M_Circle circle)
        {
            return new CollisionResult() { collision = masks.Any(f => f.ColCircle(circle)) };
            //return MaxCol(f => f.DistToCircle(circle));
        }
        
        public override bool ColSprite(Sprite sprite)
        {
            return masks.Any(f => f.ColSprite(sprite));
        }

        private CollisionResult MinCol(Func<Mask, CollisionResult> func)
        {
            CollisionResult colRes = new CollisionResult();
            for (int i = 0; i < masks.Count; i++)
                colRes.MinResult(func.Invoke(masks[i]));
            return colRes;
        }
        private CollisionResult MaxCol(Func<Mask, CollisionResult> func)
        {
            CollisionResult colRes = new CollisionResult();
            for (int i = 0; i < masks.Count; i++)
                colRes.MaxResult(func.Invoke(masks[i]));
            return colRes;
        }

        private delegate CollisionResult ColHandler();

        #endregion
    }

}
