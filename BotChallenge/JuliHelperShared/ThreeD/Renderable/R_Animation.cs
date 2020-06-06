using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuliHelper.ThreeD;
using JuliHelper;
using Microsoft.Xna.Framework;

namespace JuliHelper.ThreeD
{
    public class R_Animation : Renderable
    {
        public AnimatedModel model;//, animation;

        public R_Animation(string assetName, int animationClip, RenderMethod renderMethod = RenderMethod.Deferred)
        {
            model = new AnimatedModel(assetName);
            model.LoadContent(renderMethod);
            if (animationClip >= model.Clips.Count)
                animationClip = -1;

            if (animationClip == -1)
            {
                IdentityPose();
            }
            else if (animationClip >= 0 && animationClip < model.Clips.Count)
            {
                AnimationClip clip = model.Clips[animationClip];
                AnimationPlayer player = model.PlayClip(clip, true);
            }
        }

        /// <summary>
        /// call at the end of update
        /// </summary>
        public void Update(float elapsedSec)
        {
            model.Update(elapsedSec);//(float)gameTime.ElapsedGameTime.TotalSeconds); //don't use, because of random async (if game logic is depending on animations)
        }

        public override void Render(MeshBatch meshBatch, Vector3 pos)
        {
            Render(meshBatch, Matrix.CreateTranslation(pos));
        }

        public override void ForEachEffect(Action<Microsoft.Xna.Framework.Graphics.Effect> action)
        {
            model.ForEachEffect(action);
            //animation.ForEachEffect(action);
        }

        public override void SetWorld(Microsoft.Xna.Framework.Matrix world)
        {
            model.SetWorld(world);
            //animation.SetWorld(world);
        }

        public override void Render(MeshBatch meshBatch, Matrix transform)
        {
            model.Render(meshBatch, transform);
        }

        public void ChangeClip(int newClip, bool loop, float speed = 1f, float blendSpeed = 1f)
        {
            if (newClip != -1)
            {
                model.PlayClip(model.Clips[newClip], loop, speed, blendSpeed);
            }
            else
            {
                if (blendSpeed >= 1)
                    IdentityPose();
                else
                    model.PlayClip(model.Clips[0], loop, 0, blendSpeed, 0);
            }
        }

        public void SetSpeed(float speed)
        {
            model.SetSpeed(speed);
        }

        public override void Transform(Matrix matrix)
        {
            model.Transform(matrix);
        }

        public int GetClip()
        {
            return model.GetClip();
        }

        public void IdentityPose()
        {
            model.IdentityPose();
        }

        public int ClipCount()
        {
            return model.Clips.Count;
        }

        public void MakeLoopBlend(float blendTime)
        {
            model.MakeLoopBlend(blendTime);
        }
    }
}
