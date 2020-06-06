using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class Sprite : Mask
    {
        public Texture2D texture;
        public int sizex, sizey;
        private Vector2 _index;
        public float imageSpeed;
        public Color[] colorData;

        public bool visible;
        public bool sizeUV;

        //Aditional
        public Color color;
        public Vector2 objectOrigin;
        public Vector2 objectOriginTransformed
        {
            get { return Calculate.AbsVector(Vector2.TransformNormal(objectOrigin, GetMatrix())); }
        }
        public Vector2 imageOrigin;
        public Vector2 scale;
        public float rotation;
        public SpriteEffects mirror;
        public float depth;

        public bool showHalfImages = false;
        public bool updateColorData = true;

        public List<Sprite> childs;

        public Vector2 index
        {
            get { return _index; }
            set
            {
                Vector2 oldIndex = _index;
                _index = value;
                //if (updateColorData && Calculate.FloorVector(oldIndex) != Calculate.FloorVector(value))
                //    SetColorData();
            }
        }
        public float indexX
        {
            get { return _index.X; }
            set
            {
                //float oldIndex = _index.X;
                _index.X = value;

                if (resetIndex)
                {
                    if (Math.Floor(index.X) * sizex + ((showHalfImages) ? 0 : sizex) > texture.Width)
                        _index.X = 0;
                    else if (Math.Floor(index.X) < ((showHalfImages) ? -sizex : 0))
                        _index.X = texture.Width / sizex - 1;
                }
                //if (updateColorData && Math.Floor(oldIndex) != Math.Floor(value))
                //    SetColorData();
            }
        }
        public float indexY
        {
            get { return _index.Y; }
            set
            {
                float oldIndex = _index.Y;
                _index.Y = value;
                //if (updateColorData && Math.Floor(oldIndex) != Math.Floor(value))
                //    SetColorData();

                if (resetIndex)
                {
                    if (Math.Floor(index.Y) * sizey + ((showHalfImages) ? 0 : sizey) > texture.Height)
                        _index.Y = 0;
                    else if (Math.Floor(index.Y) < ((showHalfImages) ? -sizey : 0))
                        _index.Y = texture.Height / sizey - 1;
                }
            }
        }

        public bool resetIndex;

        public Vector2 size
        {
            get { return new Vector2(sizex, sizey); }
            set { sizex = (int)value.X; sizey = (int)value.Y; }
        }

        public Sprite(Vector2 pos, Texture2D texture)
        {
            Initialize();

            this.pos = pos;
            this.texture = texture;
            sizex = texture.Width;
            sizey = texture.Height;

            //SetColorData();
        }
        public Sprite(Vector2 pos, Texture2D texture, int sizex, int sizey, float imageSpeed, Color color, Vector2 objectOrigin, Vector2 imageOrigin, float depth)
        {
            Initialize();

            this.pos = pos;
            this.texture = texture;
            this.sizex = sizex;
            this.sizey = sizey;
            this.imageSpeed = imageSpeed;
            this.color = color;
            this.objectOrigin = objectOrigin;
            this.imageOrigin = imageOrigin;
            this.depth = depth;

            //SetColorData();
        }
        public Sprite(Vector2 pos, Texture2D texture, int sizex, int sizey, float imageSpeed, Color color, Vector2 objectOrigin, Vector2 imageOrigin, float depth, Vector2 scale, float rotation, SpriteEffects mirror)
        {
            Initialize();

            this.pos = pos;
            this.texture = texture;
            this.sizex = sizex;
            this.sizey = sizey;
            this.imageSpeed = imageSpeed;
            this.color = color;
            this.objectOrigin = objectOrigin;
            this.imageOrigin = imageOrigin;
            this.depth = depth;
            this.scale = scale;
            this.rotation = rotation;
            this.mirror = mirror;

            //SetColorData();
        }

        private void Initialize()
        {
            index = Vector2.Zero;
            imageSpeed = 0f;
            color = Color.White;
            objectOrigin = Vector2.Zero;
            imageOrigin = Vector2.Zero;
            scale = Vector2.One;
            rotation = 0f;
            mirror = SpriteEffects.None;
            depth = 0f;
            visible = true;
            resetIndex = true;
            sizeUV = true;
            childs = new List<Sprite>();
        }

        public void Update(float speed = 1f)
        {
            indexX += imageSpeed * speed;
            if (resetIndex)
            {
                if (imageSpeed > 0 && Math.Floor(index.X) * sizex + ((showHalfImages) ? 0 : sizex) > texture.Width)
                {
                    indexX = 0;
                }
                else if (imageSpeed < 0 && Math.Floor(index.X) < ((showHalfImages) ? -sizex : 0))
                {
                    indexX = texture.Width / sizex - 1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, pos, color);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            Draw(spriteBatch, pos, color);
        }
        //public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
        //{
        //    if (visible)
        //        spriteBatch.Draw(texture, (pos + imageOrigin - objectOrigin), new Rectangle((int)Math.Floor(index.X) * sizex, (int)Math.Floor(index.Y) * sizey, sizex, sizey), color, rotation, imageOrigin, scale, mirror, depth);
        //}
        public override void Draw(SpriteBatch spriteBatch, Color color, float depth = -1f)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, (pos + imageOrigin - objectOrigin), new Rectangle(GetSourceX(), GetSourceY(), sizex, sizey), color, rotation, imageOrigin, scale, mirror, depth == -1 ? this.depth : depth);
                childs.ForEach(f => f.DrawRelative(spriteBatch, pos, rotation, scale));
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float depth = -1f)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, (pos + imageOrigin - objectOrigin), new Rectangle(GetSourceX(), GetSourceY(), sizex, sizey), color, rotation, imageOrigin, scale, mirror, depth == -1 ? this.depth : depth);
                childs.ForEach(f => f.DrawRelative(spriteBatch, pos, rotation, scale));
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch, M_Rectangle destinationRectangle, Color color, float depth = -1f)
        {
            if (visible)
            {
                if (depth == -1)
                    depth = this.depth;
                Vector2 scale = new Vector2(destinationRectangle.size.X / sizex, destinationRectangle.size.Y / sizey);
                Vector2 imageOrigin = new Vector2(0, 0);

                spriteBatch.Draw(texture, (destinationRectangle.pos), new Rectangle(GetSourceX(), GetSourceY(), sizex, sizey), color, rotation, imageOrigin, scale, mirror, depth);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, Vector2 scale, float depth = -1f)
        {
            if (visible)
            {
                if (depth == -1)
                    depth = this.depth;
                spriteBatch.Draw(texture, (pos + (imageOrigin - objectOrigin) * Calculate.AbsVector(scale)), new Rectangle(GetSourceX(), GetSourceY(), sizex, sizey), color, rotation, imageOrigin, scale, mirror, depth);
            }
        }

        public void DrawRelative(SpriteBatch spriteBatch, Vector2 absPos, float absRotation, Vector2 absScale)
        {
            spriteBatch.Draw(texture, (absPos + pos + imageOrigin - objectOrigin), new Rectangle(GetSourceX(), GetSourceY(), sizex, sizey), color, absRotation + rotation, imageOrigin, absScale * scale, mirror, depth == -1 ? this.depth : depth);
        }

        //Warning: this method is not save on some graphics cards (so I disabled it for now)
        //public void SetColorData()
        //{
        //    if (texture != null)
        //    {
        //        colorData = new Color[sizex * sizey];
        //        if (sizex == texture.Width && sizey == texture.Height)
        //            texture.GetData<Color>(colorData);
        //        else
        //            texture.GetData<Color>(0, new Rectangle((int)index.X * sizex, (int)index.Y * sizey, sizex, sizey), colorData, 0, sizex * sizey);

        //        /*string text = "{ ";
        //        for (int i = 0; i < colorData.Length; i++)
        //        {
        //            text += "new Color(" + colorData[i].PackedValue + "), ";
        //        }*/
        //    }
        //}
        public void SetColorData(Color[] colorData)
        {
            this.colorData = colorData;
        }

        public Matrix GetMatrix()
        {
            if (mirror == SpriteEffects.None)
                return Matrix.CreateTranslation(new Vector3(-imageOrigin, 0))
                     * Matrix.CreateScale(scale.X, scale.Y, 1f)
                     * Matrix.CreateRotationZ(rotation)
                     * Matrix.CreateTranslation(new Vector3(pos - objectOrigin + imageOrigin, 0));
            else
                return Matrix.CreateTranslation(mirror == SpriteEffects.FlipHorizontally ? -sizex : 0, mirror == SpriteEffects.FlipVertically ? -sizey : 0, 0)
                     * Matrix.CreateScale(mirror == SpriteEffects.FlipHorizontally ? -1 : 1, mirror == SpriteEffects.FlipVertically ? -1 : 1, 1f)
                     * Matrix.CreateTranslation(new Vector3(-imageOrigin, 0))
                     * Matrix.CreateScale(scale.X, scale.Y, 1f)
                     * Matrix.CreateRotationZ(rotation)
                     * Matrix.CreateTranslation(new Vector3(pos - objectOrigin + imageOrigin, 0));
        }

        public bool IsTransformed()
        {
            return (!(scale == Vector2.One && rotation == 0f && mirror == SpriteEffects.None));
        }

        public override Vector2 GetSizeTransformed()
        {
            return GetRectangle().size;
        }
        public override Vector2 GetSize()
        {
            return new Vector2(sizex, sizey);
        }
        public override M_Rectangle GetRectangle()
        {
            if (rotation == 0 && scale == Vector2.One)
            {
                return new M_Rectangle(pos.X - objectOrigin.X, pos.Y - objectOrigin.Y, sizex, sizey);
            }
            else
            {
                Matrix transform = GetMatrix();
                Vector2 topLeft = Vector2.Zero;
                Vector2 topRight = new Vector2(sizex, 0);
                Vector2 bottomLeft = new Vector2(0, sizey);
                Vector2 bottomRight = new Vector2(sizex, sizey);

                topLeft = Vector2.Transform(topLeft, transform);
                topRight = Vector2.Transform(topRight, transform);
                bottomLeft = Vector2.Transform(bottomLeft, transform);
                bottomRight = Vector2.Transform(bottomRight, transform);


                Vector2 min = Vector2.Min(Vector2.Min(topLeft, topRight),
                                            Vector2.Min(bottomLeft, bottomRight));

                Vector2 max = Vector2.Max(Vector2.Max(topLeft, topRight),
                                            Vector2.Max(bottomLeft, bottomRight));

                min = Calculate.RoundVector(min * 10) / 10;
                max = Calculate.RoundVector(max * 10) / 10;

                return new M_Rectangle(min, max - min);
            }
        }
        public override M_Rectangle GetNoMatrixRectangle()
        {
            return new M_Rectangle(pos.X - objectOrigin.X, pos.Y - objectOrigin.Y, sizex, sizey);
        }
        public override Vector2 GetRealPos()
        {
            return pos - objectOrigin;
        }

        protected int GetSourceX()
        {
            if (showHalfImages)
                return (int)Math.Floor(index.X * (sizeUV ? Math.Min(sizex, texture.Width) : 1f));
            else
                return (int)Math.Floor(index.X) * (sizeUV ? Math.Min(sizex, texture.Width) : 1);
        }
        protected int GetSourceY()
        {
            if (showHalfImages)
                return (int)Math.Floor(index.Y * (sizeUV ? Math.Min(sizey, texture.Height) : 1f));
            else
                return (int)Math.Floor(index.Y) * (sizeUV ? Math.Min(sizey, texture.Height) : 1);
        }
        protected int GetSourceX(Texture2D tex)
        {
            if (showHalfImages)
                return (int)Math.Floor(index.X * (sizeUV ? Math.Min(sizex, tex.Width) : 1f));
            else
                return (int)Math.Floor(index.X) * (sizeUV ? Math.Min(sizex, tex.Width) : 1);
        }
        protected int GetSourceY(Texture2D tex)
        {
            if (showHalfImages)
                return (int)Math.Floor(index.Y * (sizeUV ? Math.Min(sizey, tex.Height) : 1f));
            else
                return (int)Math.Floor(index.Y) * (sizeUV ? Math.Min(sizey, tex.Height) : 1);
        }

        public Sprite CloneChild()
        {
            return (Sprite)this.MemberwiseClone();
        }

        #region Collision

        #region mask

        public override bool ColMask(Mask mask)
        {
            return mask.ColSprite(this);
        }
        public override CollisionResult DistToMask(Mask mask, Vector2 dir)
        {
            throw new NotImplementedException();
        }
        public override CollisionResult DistToMask(Mask mask)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region vector

        public override bool ColVector(Vector2 vec)
        {
            return Collision.ColVectorSprite(vec, this);
        }

        public override CollisionResult DistToVector(Vector2 vec, Vector2 dir)
        {
            throw new NotImplementedException();
        }

        public override CollisionResult DistToVector(Vector2 vec)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region rectangle

        public override bool ColRectangle(M_Rectangle rectangle)
        {
            return Collision.ColRectangleSprite(rectangle, this);
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle, Vector2 dir)
        {
            throw new NotImplementedException();
        }
        public override CollisionResult DistToRectangle(M_Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region polygon

        public override bool ColPolygon(M_Polygon polygon)
        {
            return Collision.ColPolygonSprite(polygon, this);
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon, Vector2 dir)
        {
            throw new NotImplementedException();
        }
        public override CollisionResult DistToPolygon(M_Polygon polygon)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region circle

        public override bool ColCircle(M_Circle circle)
        {
            return Collision.ColCircleSprite(circle, this);
        }
        public override CollisionResult DistToCircle(M_Circle circle, Vector2 dir)
        {
            throw new NotImplementedException();
        }
        public override CollisionResult DistToCircle(M_Circle circle)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region sprite

        public override bool ColSprite(Sprite sprite)
        {
            return Collision.ColSpriteSprite(this, sprite);
        }

        #endregion

        #endregion

    }
}
