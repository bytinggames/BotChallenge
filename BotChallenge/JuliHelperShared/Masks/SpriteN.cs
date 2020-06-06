using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class SpriteN : Sprite
    {
        public Texture2D texNormal;

        public Effect effect;

        public SpriteN(Vector2 pos, Texture2D texture, Texture2D texNormal, Effect effect)
            : base(pos, texture)
        {
            this.texNormal = texNormal;
            this.effect = effect;
        }
        public SpriteN(Vector2 pos, Texture2D texture, int sizex, int sizey, float imageSpeed, Color color, Vector2 objectOrigin, Vector2 imageOrigin, float depth, Texture2D texNormal, Effect effect)
            : base(pos, texture, sizex, sizey, imageSpeed, color, objectOrigin, imageOrigin, depth)
        {
            this.texNormal = texNormal;
            this.effect = effect;
        }
        public SpriteN(Vector2 pos, Texture2D texture, int sizex, int sizey, float imageSpeed, Color color, Vector2 objectOrigin, Vector2 imageOrigin, float depth, Vector2 scale, float rotation, SpriteEffects mirror, Texture2D texNormal, Effect effect)
            :base(pos, texture, sizex, sizey, imageSpeed, color, objectOrigin, imageOrigin, depth, scale, rotation, mirror)
        {
            this.texNormal = texNormal;
            this.effect = effect;
        }

        public override void Draw(SpriteBatch spriteBatch, Color color, float depth = -1f)
        {
            DrawEffect();
            base.Draw(spriteBatch, color, depth);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float depth = -1f)
        {
            DrawEffect();
            base.Draw(spriteBatch, pos, color, depth);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, Vector2 scale, float depth = -1f)
        {
            DrawEffect();
            base.Draw(spriteBatch, pos, color, scale, depth);
        }

        public override void Draw(SpriteBatch spriteBatch, M_Rectangle destinationRectangle, Color color, float depth = -1f)
        {
            DrawEffect();
            base.Draw(spriteBatch, destinationRectangle, color, depth);
        }

        public void DrawNormal(SpriteBatch spriteBatch)
        {
            DrawEffect();
            if (visible)
            {
                if (depth == -1)
                    depth = this.depth;

                spriteBatch.Draw(texNormal, (pos + imageOrigin - objectOrigin), new Rectangle(GetSourceX(texNormal), GetSourceY(texNormal), sizex, sizey), color, rotation, imageOrigin, scale, mirror, depth);
            }
        }

        private void DrawEffect()
        {
            if (DrawM.currentPass != null)
            {
                Matrix matrix = effect.Parameters["normalTransform"].GetValueMatrix();
                
                if (rotation != 0 || mirror != SpriteEffects.None)
                {
                    Matrix matrixTemp = matrix * Matrix.CreateRotationZ(rotation);
                    if (mirror == SpriteEffects.FlipHorizontally)
                        matrixTemp *= Matrix.CreateScale(new Vector3(-1, 1, 1));
                    else if (mirror == SpriteEffects.FlipVertically)
                        matrixTemp *= Matrix.CreateScale(new Vector3(1, -1, 1));
                    effect.Parameters["normalTransform"].SetValue(matrixTemp);
                }

                effect.Parameters["diffuseOffset"].SetValue(new Vector2(indexX * sizex, indexY * sizey));
                effect.Parameters["diffuseTexSize"].SetValue(new Vector2(texture.Width, texture.Height));

                effect.Parameters["normalTexSize"].SetValue(new Vector2(texNormal.Width, texNormal.Height));
                effect.Parameters["normalPartOffset"].SetValue(new Vector2(indexX * sizex, indexY * sizey));
                effect.Parameters["normalPartSize"].SetValue(new Vector2(texNormal.Width, texNormal.Height));

                DrawM.currentPass.Apply();

                DrawM.gDevice.Textures[1] = texNormal;

                if (rotation != 0)
                    effect.Parameters["normalTransform"].SetValue(matrix);
            }
        }
    }
}
