using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public abstract class Mask
    {
        public static float minDist = 0.1f;

        protected Vector2 _pos;


        public virtual Vector2 pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public virtual float X
        {
            get { return _pos.X; }
            set { _pos.X = value; }
        }
        public virtual float Y
        {
            get { return _pos.Y; }
            set { _pos.Y = value; }
        }

        public abstract Vector2 GetSize();
        public abstract Vector2 GetSizeTransformed();
        public abstract Vector2 GetRealPos();
        public abstract M_Rectangle GetNoMatrixRectangle();
        public abstract M_Rectangle GetRectangle();

        public Vector2 GetCenter()
        {
            return GetRealPos() + GetSizeTransformed() / 2;
        }

        public Vector2 GetPos(float xPart, float yPart)
        {
            return GetRealPos() + GetSizeTransformed()  * new Vector2(xPart, yPart);
        }
        public Vector2 GetPos(Vector2 part)
        {
            return GetRealPos() + GetSizeTransformed() * part;
        }
        public float GetX(float xPart)
        {
            return GetRealPos().X + GetSizeTransformed().X * xPart;
        }
        public float GetY(float yPart)
        {
            return GetRealPos().Y + GetSizeTransformed().Y * yPart;
        }

        public virtual Mask Clone()
        {
            return (Mask)this.MemberwiseClone();
        }
        public virtual M_Rectangle CloneRectangle()
        {
            return (M_Rectangle)this.MemberwiseClone();
        }

        public virtual void Draw(SpriteBatch spriteBatch, Color color, float depth = 0f)
        {
            M_Rectangle rect = GetRectangle();
            spriteBatch.Draw(DrawM.pixel, rect.pos, new Rectangle(0, 0, 1, 1), color, 0f, Vector2.Zero, rect.size, SpriteEffects.None, depth);
        }

        //public void Draw(SpriteBatch spriteBatch, Vector2 drawPos, Color color, float depth = 0f)
        //{
        //    Vector2 save = pos;
        //    pos = drawPos;

        //    Draw(spriteBatch, color, depth);

        //    pos = save;
        //}


        #region Collision

        public abstract bool ColMask(Mask mask);
        public abstract CollisionResult DistToMask(Mask mask, Vector2 dir);
        public abstract CollisionResult DistToMask(Mask mask);

        public abstract bool ColVector(Vector2 vec);
        public abstract CollisionResult DistToVector(Vector2 vec, Vector2 dir);
        public abstract CollisionResult DistToVector(Vector2 vec);

        public abstract bool ColRectangle(M_Rectangle rectangle);
        public abstract CollisionResult DistToRectangle(M_Rectangle rectangle, Vector2 dir);
        public abstract CollisionResult DistToRectangle(M_Rectangle rectangle);

        public abstract bool ColPolygon(M_Polygon polygon);
        public abstract CollisionResult DistToPolygon(M_Polygon polygon, Vector2 dir);
        public abstract CollisionResult DistToPolygon(M_Polygon polygon);

        public abstract bool ColCircle(M_Circle circle);
        public abstract CollisionResult DistToCircle(M_Circle circle, Vector2 dir);
        public abstract CollisionResult DistToCircle(M_Circle circle);
        
        public abstract bool ColSprite(Sprite sprite);

        public bool ColPoint(M_Point point)
        {
            return ColVector(point.pos);
        }
        public CollisionResult DistToPoint(M_Point point, Vector2 dir)
        {
            return DistToVector(point.pos, dir);
        }
        public CollisionResult DistToPoint(M_Point point)
        {
            return DistToVector(point.pos);
        }

        #endregion
    }
}

#region old
/*


        //public static bool PerPixelCollisionTransformed(Sprite sprite1, M_Rectangle rect2)//ok
        //{
        //    M_Rectangle rect1 = sprite1.GetRectangle();

        //    if (ColRectangleRectangle(rect1, rect2))
        //    {
        //        Matrix transform1 = Matrix.Invert(sprite1.GetMatrix());

        //        float xStart = Math.Max(0, rect1.pos.X - rect2.pos.X);
        //        float xEnd = Math.Min(rect2.size.X, rect1.pos.X + rect1.size.X - rect2.pos.X);
        //        float yStart = Math.Max(0, rect1.pos.Y - rect2.pos.Y);
        //        float yEnd = Math.Min(rect2.size.Y, rect1.pos.Y + rect1.size.Y - rect2.pos.Y);

        //        Vector2 pos1InY = Vector2.Transform(rect2.pos + new Vector2(xStart, yStart), transform1);
        //        Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transform1);
        //        Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transform1);
        //        for (float y2 = yStart;;)
        //        {
        //            Vector2 pos1 = pos1InY;
        //            for (float x2 = xStart;;)
        //            {
        //                if (pos1.X >= 0 && pos1.X < sprite1.sizex && pos1.Y >= 0 && pos1.Y < sprite1.sizey)
        //                {
        //                    if (sprite1.colorData[(int)pos1.Y * sprite1.sizex + (int)pos1.X].A != 0)
        //                        return true;
        //                }
        //                pos1 += stepX;

        //                x2++;
        //                if (x2 == xEnd && pos1.X % 1 == 0)
        //                    break;
        //                else if (x2 > xEnd)
        //                    break;
        //            }
        //            pos1InY += stepY;

        //            y2++;
        //            if (y2 == yEnd && pos1InY.Y % 1 == 0)
        //                break;
        //            else if (y2 > yEnd)
        //                break;
        //        }
        //    }

        //    return false;
        //}


        public static bool PerPixelCollision(Sprite sprite1, Sprite sprite2)
        {
            M_Rectangle rect1 = sprite1.GetRectangle();
            M_Rectangle rect2 = sprite2.GetRectangle();

            if (ColRectangleRectangle(rect1, rect2))
            {
                int top = (int)Math.Max(rect1.Top, rect2.Top);
                int bottom = (int)Math.Min(rect1.Bottom, rect2.Bottom);
                int left = (int)Math.Max(rect1.Left, rect2.Left);
                int right = (int)Math.Min(rect1.Right, rect2.Right);

                for (int y = top; y < bottom; y++)
                {
                    for (int x = left; x < right; x++)
                    {
                        Color color1 = sprite1.colorData[(int)((y - (int)rect1.Top) * rect1.size.X + (x - (int)(rect1.Left)))];
                        Color color2 = sprite2.colorData[(int)((y - (int)rect2.Top) * rect2.size.X + (x - (int)rect2.Left))];

                        if (color1.A != 0 && color2.A != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

 * 
 * 
 * 
 *          ////PosSpriteDist
                    //if (Math.Abs(d) <= Math.Abs(d2))
                    //{
                    //    int x = Math.Max(0, (int)-d);
                    //    bool startColored = sprite.colorData[y * sprite.sizex + x].A != 0;
                    //    x += (startColored ? -1 : 1);
                    //    for (; x < sprite.sizex && x >= 0; x += (startColored ? -1 : 1))
                    //    {
                    //        Color color = sprite.colorData[y * sprite.sizex + x];
                    //        if (color.A != 0 != startColored)
                    //        {
                    //            d += x + (startColored ? 1 : 0);
                    //            inDistance = true;
                    //            return d;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    d = d2;

                    //    int x = sprite.sizex - Math.Max(1, (int)d);
                    //    bool startColored = sprite.colorData[y * sprite.sizex + x].A != 0;
                    //    x += (startColored ? 1 : -1);
                    //    for (; x < sprite.sizex && x >= 0; x += (startColored ? 1 : -1))
                    //    {
                    //        Color color = sprite.colorData[y * sprite.sizex + x];
                    //        if (color.A != 0 != startColored)
                    //        {
                    //            d -= sprite.sizex - x - (startColored ? 0 : 1);
                    //            inDistance = true;
                    //            return d;
                    //        }
                    //    }
                    //}
*/
#endregion