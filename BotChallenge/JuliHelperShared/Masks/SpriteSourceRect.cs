using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
    using SAF = JuliHelper.RectFrame;

namespace JuliHelper
{
    public class SpriteSourceRect : SpriteSource
    {


        public int animation;
        public float frame;
        public float relativeSpeed;

        public RectAnim[] animations;

        #region Accessors

        public int FrameInt
        {
            get { return (int)frame; }
        }

        public int AnimationCount
        {
            get { return animations.Length; }
        }

        #endregion

        /// <summary></summary>
        /// <param name="w">part width</param>
        /// <param name="h">part height</param>
        /// <param name="x">sheet offset x</param>
        /// <param name="y">sheet offset y</param>
        /// <param name="frameCount">frame count for indexed animation.<para/>null: calculate maximum frame count regarding x, y, w and h</param>
        /// <param name="speed">speed for each animation</param>
        public SpriteSourceRect(Texture2D texture, RectAnim[] animations, int startAnimation = 0, float startFrame = 0, float relativeSpeed = 1f)
            :base(texture)
        {
            this.animations = animations;
            this.animation = startAnimation;
            this.frame = startFrame;
            this.relativeSpeed = relativeSpeed;
        }

        public override void Update(float gameSpeed = 1f)
        {
            SetFrameRelative(animations[animation].speed * relativeSpeed * gameSpeed);
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
            if (frame >= animations[animation].frames.Length)
                frame = frame % animations[animation].frames.Length;
            else if (frame < 0)
                frame = animations[animation].frames.Length + (frame % animations[animation].frames.Length);
        }

        public override Rectangle GetSourceRectangle()
        {
            return animations[animation].frames[FrameInt].rectangle;
        }

        public override Vector2 GetOrigin()
        {
            return animations[animation].frames[FrameInt].origin;
        }
    }

    public struct RectAnim
    {
        public float speed;
        public RectFrame[] frames;
    }

    public class RectFrame
    {
        public Rectangle rectangle;
        public Vector2 origin;

        public RectFrame(int x, int y, int w, int h, float originX, float originY)
        {
            this.rectangle = new Rectangle(x, y, w, h);
            this.origin = new Vector2(originX, originY);
        }
    }
}
