using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuliHelper.ThreeD;
using System.ComponentModel;
using Microsoft.Xna.Framework;


namespace JuliHelper.ThreeD
{
    /// <summary>
    /// Animation clip player. It maps an animation clip onto a model
    /// </summary>
    public class AnimationPlayer
    {
        const float frameLength = 1f / 24f;

        #region Fields

        /// <summary>
        /// Current position in time in the clip
        /// </summary>
        private float position = 0;

        /// <summary>
        /// The clip we are playing
        /// </summary>
        private AnimationClip clip = null;

        /// <summary>
        /// We maintain a BoneInfo class for each bone. This class does
        /// most of the work in playing the animation.
        /// </summary>
        private BoneInfo[] boneInfos;

        /// <summary>
        /// The number of bones
        /// </summary>
        private int boneCnt;

        /// <summary>
        /// An assigned model
        /// </summary>
        private AnimatedModel model = null;

        /// <summary>
        /// The looping option
        /// </summary>
        private bool looping = false;

        public float speed = 1f;

        #endregion

        #region Properties

        /// <summary>
        /// The position in the animation
        /// </summary>
        //[Browsable(false)]
        //public float Position
        //{
        //    get { return position; }
        //    set
        //    {
        //        if (value >= Duration)
        //        {
        //            if (looping)
        //            {
        //                value -= Duration;
        //                value += 1 / 24f; //0
        //            }
        //            else
        //                value = Duration;
        //        }

        //        position = value;
        //        foreach (BoneInfo bone in boneInfos)
        //        {
        //            bone.SetPosition(position);
        //        }
        //    }
        //}

        public float Position
        {
            get
            {
                return position;
            }
        }

        public void SetPosition(float pos)
        {
            if (pos >= Duration)
            {
                if (looping)
                {
                    pos -= Duration;
                    pos += frameLength; //0
                }
                else
                    pos = Duration;
            }

            this.position = pos;
            foreach (BoneInfo bone in boneInfos)
            {
                bone.SetPosition(pos);
            }
        }

        /// <summary>
        /// The associated animation clip
        /// </summary>
        [Browsable(false)]
        public AnimationClip Clip { get { return clip; } }

        /// <summary>
        /// The clip duration
        /// </summary>
        [Browsable(false)]
        public float Duration { get { return (float)clip.Duration; } }

        /// <summary>
        /// A model this animation is assigned to. It will play on that model.
        /// </summary>
        [Browsable(false)]
        public AnimatedModel Model { get { return model; } }

        /// <summary>
        /// The looping option. Set to true if you want the animation to loop
        /// back at the end
        /// </summary>
        public bool Looping { get { return looping; } set { looping = value; } }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor for the animation player. It makes the 
        /// association between a clip and a model and sets up for playing
        /// </summary>
        /// <param name="clip"></param>
        public AnimationPlayer(AnimationClip clip, AnimatedModel model, float start)
        {
            this.clip = clip;
            this.model = model;

            // Create the bone information classes
            boneCnt = clip.Bones.Count;
            boneInfos = new BoneInfo[boneCnt];

            for(int b=0;  b<boneInfos.Length;  b++)
            {
                // Create it
                boneInfos[b] = new BoneInfo(clip.Bones[b]);

                // Assign it to a model bone
                boneInfos[b].SetModel(model);
            }

            Rewind(start);
        }

        #endregion

        #region Update and Transport Controls


        /// <summary>
        /// Reset back to time zero.
        /// </summary>
        public void Rewind(float start)
        {
            SetPosition(start);
        }

        public void InitTransform()
        {
            foreach (BoneInfo bone in boneInfos)
            {
                bone.InitTransform();
            }
        }

        /// <summary>
        /// Update the clip position
        /// </summary>
        public void Update(float elapsedSec)
        {
            SetPosition(Position + elapsedSec * speed);
        }

        public void Blend(AnimationPlayer player2, float blend)
        {
            for (int i = 0; i < boneInfos.Length; i++)
            {
                if (!boneInfos[i].uvTransform)
                {
                    //lerp mesh transform
                    boneInfos[i].rotation = Quaternion.Slerp(boneInfos[i].rotation, player2.boneInfos[i].rotation, blend);
                    boneInfos[i].translation = Vector3.Lerp(boneInfos[i].translation, player2.boneInfos[i].translation, blend);
                    boneInfos[i].scale = Vector3.Lerp(boneInfos[i].scale, player2.boneInfos[i].scale, blend);
                }
                else
                {
                    //round uv transform
                    boneInfos[i].rotation = blend < 0.5f ? boneInfos[i].rotation : player2.boneInfos[i].rotation;
                    boneInfos[i].translation = blend < 0.5f ? boneInfos[i].translation : player2.boneInfos[i].translation;
                    boneInfos[i].scale = blend < 0.5f ? boneInfos[i].scale : player2.boneInfos[i].scale;
                }
            }
        }

        public void ApplyCompleteTransform()
        {
            foreach (BoneInfo bone in boneInfos)
            {
                bone.ApplyCompleteTransform();
            }
        }

        #endregion

        #region BoneInfo class


        /// <summary>
        /// Information about a bone we are animating. This class connects a bone
        /// in the clip to a bone in the model.
        /// </summary>
        private class BoneInfo
        {
            #region Fields

            /// <summary>
            /// The current keyframe. Our position is a time such that the 
            /// we are greater than or equal to this keyframe's time and less
            /// than the next keyframes time.
            /// </summary>
            private int currentKeyframe = 0;

            /// <summary>
            /// Bone in a model that this keyframe bone is assigned to
            /// </summary>
            private Bone assignedBone = null;

            /// <summary>
            /// We are not valid until the rotation and translation are set.
            /// If there are no keyframes, we will never be valid
            /// </summary>
            public bool valid = false;

            /// <summary>
            /// Current animation rotation
            /// </summary>
            public Quaternion rotation;

            /// <summary>
            /// Current animation translation
            /// </summary>
            public Vector3 translation;

            /// <summary>
            /// Current animation scale
            /// </summary>
            public Vector3 scale;

            /// <summary>
            /// We are at a location between Keyframe1 and Keyframe2 such 
            /// that Keyframe1's time is less than or equal to the current position
            /// </summary>
            public AnimationClip.Keyframe Keyframe1;

            /// <summary>
            /// Second keyframe value
            /// </summary>
            public AnimationClip.Keyframe Keyframe2;

            public bool uvTransform;

            #endregion

            #region Properties

            /// <summary>
            /// The bone in the actual animation clip
            /// </summary>
            public AnimationClip.Bone ClipBone { get; set; }

            /// <summary>
            /// The bone this animation bone is assigned to in the model
            /// </summary>
            public Bone ModelBone { get { return assignedBone; } }
            
            #endregion

            #region Constructor

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bone"></param>
            public BoneInfo(AnimationClip.Bone bone)
            {
                this.ClipBone = bone;
                uvTransform = bone.Name.StartsWith("UV_");// && !bone.Name.EndsWith("_end");
                SetKeyframes();
                SetPosition(0);
            }


            #endregion

            #region Position and Keyframes

            /// <summary>
            /// Set the bone based on the supplied position value
            /// </summary>
            /// <param name="position"></param>
            public void SetPosition(float position)
            {
                List<AnimationClip.Keyframe> keyframes = ClipBone.Keyframes;
                if (keyframes.Count == 0)
                    return;

                // If our current position is less that the first keyframe
                // we move the position backward until we get to the right keyframe
                while (position < Keyframe1.Time && currentKeyframe > 0)
                {
                    // We need to move backwards in time
                    currentKeyframe--;
                    SetKeyframes();
                }
                
                // If our current position is greater than the second keyframe
                // we move the position forward until we get to the right keyframe
                while (position >= Keyframe2.Time && currentKeyframe < ClipBone.Keyframes.Count - 1)//-2
                {
                    // We need to move forwards in time
                    currentKeyframe++;
                    SetKeyframes();
                }

                if (Keyframe1 == Keyframe2)
                {
                    // Keyframes are equal
                    rotation = Keyframe1.Rotation;
                    translation = Keyframe1.Translation;
                    scale = Keyframe1.Scale;
                }
                else
                {
                    float t = (float)((position - Keyframe1.Time) / (Keyframe2.Time - Keyframe1.Time));
                    if (!uvTransform)
                    {
                        //mesh transform
                        // Interpolate between keyframes
                        rotation = Quaternion.Slerp(Keyframe1.Rotation, Keyframe2.Rotation, t);
                        translation = Vector3.Lerp(Keyframe1.Translation, Keyframe2.Translation, t);
                        scale = Vector3.Lerp(Keyframe1.Scale, Keyframe2.Scale, t);
                    }
                    else
                    {
                        //if (t < 0.5f)
                        //{
                            rotation = Keyframe1.Rotation;
                            translation = Keyframe1.Translation;
                            scale = Keyframe1.Scale;
                        //}
                        //else
                        //{
                        //    rotation = Keyframe2.Rotation;
                        //    translation = Keyframe2.Translation;
                        //    scale = Keyframe2.Scale;
                        //}
                    }
                }

                valid = true;
            }

            //public void SetPositionApply(float position, float amount, Quaternion r, Vector3 t, Vector3 s)
            //{
            //    valid = true;
            //    if (assignedBone != null)
            //    {
            //        rotation = Quaternion.Slerp(r, rotation, amount);
            //        translation = Vector3.Lerp(t, translation, amount);
            //        scale = Vector3.Lerp(s, scale, amount);


            //        // Send to the model
            //        // Make it a matrix first
            //        assignedBone.AddTransform(scale, rotation, translation, 1);
            //    }
            //}

            public void ApplyCompleteTransform()
            {
                if (assignedBone != null)
                {
                    assignedBone.SetCompleteTransform(scale, rotation, translation);

                }
            }



            /// <summary>
            /// Set the keyframes to a valid value relative to 
            /// the current keyframe
            /// </summary>
            private void SetKeyframes()
            {
                if (ClipBone.Keyframes.Count > 0)
                {
                    //if (currentKeyframe == 0 && ClipBone.Keyframes.Count > 1)
                    //    currentKeyframe = 1;

                    Keyframe1 = ClipBone.Keyframes[currentKeyframe];
                    if (currentKeyframe == ClipBone.Keyframes.Count - 1)
                        Keyframe2 = Keyframe1;
                    else
                    {
                        //if (currentKeyframe == ClipBone.Keyframes.Count - 1)
                        //{
                        //    if (ClipBone.Keyframes.Count > 1)
                        //        Keyframe2 = ClipBone.Keyframes[1];
                        //    else
                        //        Keyframe2 = ClipBone.Keyframes[0];
                        //}
                        //else
                        {
                            Keyframe2 = ClipBone.Keyframes[currentKeyframe + 1];
                        }
                    }
                }
                else
                {
                    // If there are no keyframes, set both to null
                    Keyframe1 = null;
                    Keyframe2 = null;
                }
            }

            /// <summary>
            /// Assign this bone to the correct bone in the model
            /// </summary>
            /// <param name="model"></param>
            public void SetModel(AnimatedModel model)
            {
                // Find this bone
                assignedBone = model.FindBone(ClipBone.Name);

            }

            internal void InitTransform()
            {
                assignedBone.InitTransform();
            }

            #endregion
        }

        #endregion

    }
}
