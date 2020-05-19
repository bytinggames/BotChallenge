using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using JuliHelper.ThreeD;
using JuliHelper;

namespace JuliHelper.ThreeD
{
    /// <summary>
    /// An encloser for an XNA model that we will use that includes support for
    /// bones, animation, and some manipulations.
    /// </summary>
    public class AnimatedModel
    {
        #region Fields

        /// <summary>
        /// The actual underlying XNA model
        /// </summary>
        private Model model = null;

        /// <summary>
        /// Extra data associated with the XNA model
        /// </summary>
        private ModelExtra modelExtra = null;

        /// <summary>
        /// The model bones
        /// </summary>
        private List<Bone> bones = new List<Bone>();

        /// <summary>
        /// The model asset name
        /// </summary>
        private string assetName = "";

        /// <summary>
        /// An associated animation clip player
        /// </summary>
        private AnimationPlayer finalPlayer;
        private List<BlendPlayer> players; //blending out of these into finalPlayer

        class BlendPlayer
        {
            public AnimationPlayer player;
            public float blend; //animation += (1-blend) * this.player -> blend away from this player
            public float blendSpeed;

            public BlendPlayer(AnimationPlayer player, float blendSpeed)
            {
                this.player = player;
                this.blendSpeed = blendSpeed;
                blend = 0;
            }
            /// <summary>
            /// Update blending
            /// returns true if blend finished
            /// </summary>
            public bool Update(float elapsedSec)
            {
                blend += blendSpeed;
                if (blend >= 1)
                {
                    blend = 1;
                    return true;
                }
                else
                {
                    player.Update(elapsedSec);
                    return false;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The actual underlying XNA model
        /// </summary>
        public Model Model
        {
            get { return model; }
        }

        /// <summary>
        /// The underlying bones for the model
        /// </summary>
        public List<Bone> Bones { get { return bones; } }

        /// <summary>
        /// The model animation clips
        /// </summary>
        public List<AnimationClip> Clips { get { return modelExtra.Clips; } }

        public float ClipPosition
        {
            get
            {
                if (finalPlayer != null)
                    return finalPlayer.Position;
                else
                    return -1;
            }
        }

        public void IdentityPose()
        {
            finalPlayer = PlayClip(Clips[0], true, 0, 1, 0);
            players.Clear();
            finalPlayer.speed = 0;
            Update(0);
            //player1 = null;
        }

        public void MakeLoopBlend(float blendTime)
        {
            for (int i = 0; i < Clips.Count; i++)
            {
                Clips[i].MakeLoopBlend(blendTime);
            }
        }


        #endregion

        #region Construction and Loading

        /// <summary>
        /// Constructor. Creates the model from an XNA model
        /// </summary>
        /// <param name="assetName">The name of the asset for this model</param>
        public AnimatedModel(string assetName)
        {
            this.assetName = assetName;
        }

        /// <summary>
        /// Load the model asset from content
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(RenderMethod renderMethod)
        {
            this.model = ContentLoader.models[assetName];//content.Load<Model>(assetName);
            modelExtra = model.Tag as ModelExtra;
            System.Diagnostics.Debug.Assert(modelExtra != null);
            
            ObtainBones();

            ModelHelper.SetModelTechniques(model, "Bone", renderMethod);

            players = new List<BlendPlayer>();
        }


        #endregion

        #region Bones Management

        /// <summary>
        /// Get the bones from the model and create a bone class object for
        /// each bone. We use our bone class to do the real animated bone work.
        /// </summary>
        private void ObtainBones()
        {
            bones.Clear();
            foreach (ModelBone bone in model.Bones)
            {
                // Create the bone object and add to the heirarchy
                
                if (bone.Name.ToLower().EndsWith("_identity"))
                    bone.Transform = Matrix.Identity;

                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? bones[bone.Parent.Index] : null);

                // Add to the bones for this model
                bones.Add(newBone);
            }

            //compute absolute transformations //TODO: is this really necessary (only bindTransform is set)
            for (int i = 0; i < bones.Count; i++)
            {
                if (bones[i].Parent == null)
                {
                    bones[i].ComputeAbsoluteTransformRecursive(Vector3.Zero);
                }
            }

            // Set the skinning bind transform
            // That is the inverse of the absolute transform in the bind pose
            for (int i = 0; i < bones.Count; i++)
                bones[i].SkinTransform = Matrix.Invert(bones[i].AbsoluteTransform);
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            foreach(Bone bone in Bones)
            {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }

        #endregion

        #region Animation Management

        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip, bool loop, float speed = 1f, float blendSpeed = 1f, float start = 1f / 24f)
        {
            bool newPlayer = true;
            if (blendSpeed >= 1)
            {
                //no blending
                players.Clear();
            }
            else
            {
                //blending
                if (blendSpeed < 0)
                    blendSpeed = 0;

                if (finalPlayer.Clip == clip && (finalPlayer.speed == 0) == (speed == 0))
                {
                    if (players.Count > 0)
                    {
                        players[players.Count - 1].blendSpeed = blendSpeed;
                    }
                    newPlayer = false;
                }

                //if the previous animation has the same clip as this new one (and both are animating or stopping), just swap finalPlayer with the last blendPlayer
                //else if (players.Count > 0 && players[players.Count - 1].player.Clip == clip && (players[players.Count - 1].player.speed == 0) == (speed == 0))
                //{
                //    BlendPlayer p = players[players.Count - 1];
                //    AnimationPlayer save = finalPlayer;
                //    finalPlayer = p.player;
                //    p.player = save;
                //    p.blend = 1 - p.blend;
                //    p.blendSpeed = blendSpeed;
                //}
                else
                //add blend player
                    players.Add(new BlendPlayer(finalPlayer, blendSpeed));
            }

            if (newPlayer)
                finalPlayer = new AnimationPlayer(clip, this, start);
            finalPlayer.Looping = loop;
            finalPlayer.speed = speed;


            return finalPlayer;

            //// Create a clip player and assign it to this model

            //blendSpeed = Math.Max(0, Math.Min(1, blendSpeed));

            //this.blendSpeed = blendSpeed;

            //if (blendSpeed >= 1)
            //{
            //    player1 = new AnimationPlayer(clip, this, start);
            //    player1.Looping = loop;
            //    player1.speed = speed;
            //    clipBlend = 0;
            //    return player1;
            //}
            //else
            //{
            //    if (player2 != null)
            //    {
            //        if (player2.Clip == clip &&(player2.speed == 0) == (speed == 0)) //already blending to this clip
            //        { }
            //        else
            //        if (player1.Clip == clip) //reverse blending
            //        {
            //            //Swap players
            //            Calculate.Swap(ref player1, ref player2);
            //            clipBlend = 1 - clipBlend;
            //        }
            //        else
            //        {
            //            //jump to player2 if blending is >= 0.5
            //            if (clipBlend >= 0.5f)
            //                player1 = player2;
            //            player2 = null;
            //        }

            //    }

            //    if (player2 == null)
            //    {
            //        //(re)start blending
            //        player2 = new AnimationPlayer(clip, this, start);
            //        clipBlend = 0;
            //    }

            //    //set loop and speed
            //    player2.Looping = loop;
            //    player2.speed = speed;
            //    return player2;
            //}
        }

        #endregion

        #region Updating

        /// <summary>
        /// Update animation for the model.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float elapsedSec)
        {
            finalPlayer.InitTransform();

            for (int i = 0; i < players.Count;)
            {
                if (players[i].Update(elapsedSec))
                {
                    players.RemoveAt(i);
                }
                else
                {
                    if (i > 0)
                        players[i].player.Blend(players[i - 1].player, 1 - players[i - 1].blend);
                    i++;
                }
            }

            finalPlayer.Update(elapsedSec);

            if (players.Count > 0)
                finalPlayer.Blend(players[players.Count - 1].player, 1 - players[players.Count - 1].blend);

            finalPlayer.ApplyCompleteTransform();


            //if (player1 != null)
            //{
            //    player1.InitTransform();

            //    if (player2 != null)
            //    {
            //        clipBlend += blendSpeed;
            //        if (clipBlend >= 1)
            //        {
            //            clipBlend = 0;
            //            player1 = player2;
            //            player2 = null;
            //        }
            //        else
            //        {
            //            player2.Update(elapsedSec);
            //        }
            //    }

            //    player1.Update(elapsedSec);

            //    if (player2 != null)
            //        player1.Blend(player2, clipBlend); //blend player2 on player1

            //    player1.ApplyCompleteTransform();
            //}
        }

        //public void Init()
        //{

        //    if (player1 != null)
        //    {
        //        player1.InitTransform();
        //        player1.Update(0, 1);

        //        player1.ApplyCompleteTransform();
        //    }
        //}


        #endregion

        #region Drawing

        /// <summary>
        /// Draw the model
        /// </summary>
        /// <param name="graphics">The graphics device to draw on</param>
        /// <param name="world">A world matrix to place the model</param>
        public void Render(MeshBatch meshBatch, Matrix world)
        {
            if (model == null)
                return;
            
            //// Compute all of the bone absolute transforms
            //Matrix[] boneTransforms = new Matrix[bones.Count];
            
            bool uniformScalingAnimation = true;

            for (int i = 0; i < bones.Count; i++)
            {
                if (bones[i].Parent == null)
                {
                    uniformScalingAnimation = bones[i].ComputeAbsoluteTransformRecursive(Vector3.Zero);
                }
            }

            //for (int i = 0; i < bones.Count; i++)
            //{
            //    boneTransforms[i] = bones[i].AbsoluteTransform;
            //}

            // Determine the skin transforms from the skeleton
            
            Dictionary<string, int> uvBones = new Dictionary<string, int>();

            Matrix[] skeleton = new Matrix[modelExtra.Skeleton.Count];
            for (int s = 0; s < modelExtra.Skeleton.Count; s++)
            {
                Bone bone = bones[modelExtra.Skeleton[s]];
                skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform; //TODO: check (skintransform?)
                if (bone.Name.StartsWith("UV_"))// && !bone.Name.EndsWith("_end"))
                {
                    skeleton[s].Translation *= 0.1f;
                    uvBones.Add(bone.Name.Substring(3), s);
                    //skeleton[s] = Matrix.CreateTranslation(new Vector3(0.5f, 0, 0));
                }

                //if (skeleton[s].Scale == Vector3.One)
                //{
                    //skeleton[s] = Matrix.CreateScale(10, -10, -10) * skeleton[s]; //TODO: TEST
                //}

                //if (bone.Name == "Bone.004" || bone.Name == "Bone.005")
                //    skeleton[s].Translation = new Vector3(skeleton[s].Translation.X, skeleton[s].Translation.Y, skeleton[s].Translation.Z); //TODO: TEST
            }
            
            meshBatch.Draw(model, world, skeleton, uniformScalingAnimation, uvBones);
            //meshBatch.Draw(model, world);

            
            /*for (int i = 0; i < model.Meshes.Count; i++)
            {
                for (int j = 0; j < model.Meshes[i].MeshParts.Count; j++)
                {
                    ModelMeshPart mp = model.Meshes[i].MeshParts[j];

                    if (mp.PrimitiveCount > 0)
                    {
                        Effect effect = model.Meshes[i].Effects[j];

                        effect.SetWorldAndInvTransp(world); //TODO: check
                        //effect.SetWorldAndInvTransp(boneTransforms[model.Meshes[i].ParentBone.Index] * world); //TODO: check

                        effect.Parameters["Bones"].SetValue(skeleton);
                        effect.Parameters["uniformScalingAnimation"].SetValue(uniformScalingAnimation);

                        DrawM.gDevice.SetVertexBuffer(mp.VertexBuffer);
                        DrawM.gDevice.Indices = mp.IndexBuffer;

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            DrawM.gDevice.SamplerStates[0] = SamplerState.PointClamp; //RELEASE: REMOVE
                            DrawM.gDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                                   mp.VertexOffset,
                                                                   mp.StartIndex,
                                                                   mp.PrimitiveCount);

                        }
                    }
                }
            }*/
        }

        #endregion

        public void ForEachEffect(Action<Effect> action)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    action(part.Effect);
                }
            }
        }
        public void SetWorld(Matrix world)
        {
            foreach (var mesh in model.Meshes)
            {
                mesh.ParentBone.Transform = world;
            }
        }

        public void Transform(Matrix matrix)
        {
            //for (int i = 0; i < bones.Count; i++)
            //{
            //    bones[i].BindTransform *= matrix;
            //}

            foreach (var mesh in model.Meshes)
            {
                mesh.ParentBone.Transform *= matrix;
            }
        }

        public int GetClip()
        {
            return Clips.IndexOf(finalPlayer.Clip);
        }

        public void SetSpeed(float speed)
        {
            finalPlayer.speed = speed;
        }
    }
}
