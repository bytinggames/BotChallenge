using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class NewSprite
    {
        public List<NewSprite> childs = new List<NewSprite>();

        public NewSprite parent;

        //relative to parent
        public Vector2 pos;    //3. transformation //top left of drawn rectangle
        public float rotation; //2. transformation //rotate around 0
        public Vector2 scale = Vector2.One;  //1. transformation
        public SpriteSource source; //texture + source rectangle
        public float depth = 0;
        public SpriteEffects flip = SpriteEffects.None;

        public Color color = Color.White;
        public InheritageFactor colorInherit = InheritageFactor.Multiply;
        public Vector2 origin; //relative pivot point for rotation

        #region Accessors

        //public Vector2 oPos //absolute pivot point for rotation
        //{
        //    get { return pos + origin; }
        //    set { pos = value - origin; }
        //}

        #endregion

        public NewSprite()
        {

        }

        public NewSprite(string texName, int sourceX, int sourceY, int sourceW, int sourceH, float posX, float posY, float originX, float originY, float depth, List<NewSprite> childs = null)
        {
            if (childs == null)
                childs = new List<NewSprite>();
            else
                for (int i = 0; i < childs.Count; i++)
                    childs[i].parent = this;

            this.source = new SpriteSourceStatic(ContentLoader.textures[texName], sourceX, sourceY, sourceW, sourceH);
            this.pos = new Vector2(posX, posY);
            this.origin = new Vector2(originX, originY);
            this.depth = depth;
            this.childs = childs;
        }

        public void Update(float gameSpeed = 1f)
        {
            source.Update(gameSpeed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Matrix.Identity, 0);

            //if (parent == null)
            //    spriteBatch.Draw(source.texture, pos, source.GetSourceRectangle(), color, rotation, origin + source.GetOrigin(), scale, flip, depth);
            //else
            //{
            //    Vector2 transformedPos = parent.pos + Vector2.Transform(pos, Matrix.CreateRotationZ(parent.rotation));

            //    spriteBatch.Draw(source.texture, transformedPos, source.GetSourceRectangle(), color, parent.rotation + rotation, origin + source.GetOrigin(), parent.scale * scale, flip, depth);
            //}
            //for (int i = 0; i < childs.Count; i++)
            //{
            //    childs[i].Draw(spriteBatch);
            //}
        }

        public void Draw(SpriteBatch spriteBatch, Matrix transform, float parentDepth)
        {
            transform = Matrix.CreateScale(scale.X, scale.Y, 1) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(pos, 0)) * transform;
            parentDepth += depth;

            Vector3 posT, scaleT;
            Quaternion q;
            transform.Decompose(out scaleT, out q, out posT);
            float rot = (float)Math.Atan2(-2f * (q.X * q.Y - q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);
            spriteBatch.Draw(source.texture, new Vector2(posT.X, posT.Y), source.GetSourceRectangle(), color, rot, origin + source.GetOrigin(), new Vector2(scaleT.X, scaleT.Y), flip, parentDepth);


            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].Draw(spriteBatch, transform, parentDepth);
            }
        }

        public void CallRecursive(Action<NewSprite> action)
        {
            action(this);
            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].CallRecursive(action);
            }
        }
    }
    public enum InheritageFactor
    {
        None,       //x = this.x
        Override,   //x = parent.x
        Multiply    //x = parent.x * this.x
    }
}
