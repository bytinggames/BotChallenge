using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class SpriteSourceSheet : SpriteSource
    {
        public int x, y, w, h;
        public int animation;
        public float frame;
        public int[] frameCount; //[animation index] = frame count
        public float[] speed;
        public float relativeSpeed;

        #region Accessors

        public int FrameInt
        {
            get { return (int)frame; }
        }

        public int AnimationCount
        {
            get { return frameCount.Length; }
        }

        #endregion

        /// <summary></summary>
        /// <param name="w">part width</param>
        /// <param name="h">part height</param>
        /// <param name="x">sheet offset x</param>
        /// <param name="y">sheet offset y</param>
        /// <param name="frameCount">frame count for indexed animation.<para/>null: calculate maximum frame count regarding x, y, w and h</param>
        /// <param name="speed">speed for each animation</param>
        public SpriteSourceSheet(Texture2D texture, int w, int h, int x = 0, int y = 0, int[] frameCount = null, float[] speed = null, int startAnimation = 0, float startFrame = 0, float relativeSpeed = 1f)
            :base(texture)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.speed = speed;
            this.animation = startAnimation;
            this.frame = startFrame;
            this.relativeSpeed = relativeSpeed;

            if (w <= 0 || h <= 0)
            {
                throw new Exception("size is <= 0");
                //w = h = 1;
            }

            int maxAnimations = (texture.Height - y) / h;

            if (frameCount == null) //calculate maximum frame count
            {
                this.frameCount = new int[maxAnimations];
                int frames = (texture.Width - x) / w;
                for (int i = 0; i < this.frameCount.Length; i++)
                {
                    this.frameCount[i] = frames;
                }
            }
            else
            {
                this.frameCount = frameCount;

                if (frameCount.Length > maxAnimations)
                    throw new Exception("too high animation count for the texture");
                else
                {
                    for (int i = 0; i < frameCount.Length; i++)
                        if (frameCount[i] > (texture.Width - x) / w)
                            throw new Exception("frameCount[" + i + "] is to large for the texture");
                }
            }

            if (this.speed == null)
            {
                this.speed = new float[] { 0f };
            }
        }

        public override void Update(float gameSpeed = 1f)
        {
            SetFrameRelative(speed[animation % speed.Length] * relativeSpeed * gameSpeed);
        }

        /// <summary></summary>
        /// <param name="animation"></param>
        /// <param name="startFrame">-1: old frame</param>
        public void PlayAnimation(int animation, float startFrame = 0)
        {
            this.animation = animation;

            if (startFrame >= 0)
                this.frame = startFrame;
        }
        /// <summary></summary>
        /// <param name="animation"></param>
        /// <param name="startFrame">-1: old frame</param>
        public void PlayAnimation(int animation, float relativeSpeed, float startFrame = 0)
        {
            PlayAnimation(animation, startFrame);
            this.relativeSpeed = relativeSpeed;
        }

        public void SetFrameRelative(float relativeFrames)
        {
            frame += relativeFrames;

            //if outside of animation, jump back in (start/end)
            if (frame >= frameCount[animation])
                frame = frame % frameCount[animation];
            else if (frame < 0)
                frame = frameCount[animation] + (frame % frameCount[animation]);
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(x + (int)frame * w, y + animation * h, w, h);
        }
    }
}
