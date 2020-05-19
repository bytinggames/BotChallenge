using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace JuliHelper.ThreeD
{
    /// <summary>
    /// An animation clip is a set of keyframes with associated bones.
    /// </summary>
    public class AnimationClip
    {
        #region Keyframe and Bone nested class

        /// <summary>
        /// An Keyframe is a rotation and translation for a moment in time.
        /// It would be easy to extend this to include scaling as well.
        /// </summary>
        public class Keyframe
        {
            public double Time;             // The keyframe time
            public Quaternion Rotation;     // The rotation for the bone
            public Vector3 Translation;     // The translation for the bone
            public Vector3 Scale;           // The scale for the bone

            public Matrix Transform
            {
                get
                {
                    return Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Translation);
                }
                set
                {
                    Matrix transform = value;


                    //ModelViewer
                    //Scale = new Vector3(transform.Right.Length(), transform.Up.Length(), transform.Backward.Length());

                    //transform.Right = transform.Right / Scale.X;
                    //transform.Up = transform.Up / Scale.Y;
                    //transform.Backward = transform.Backward / Scale.Z;
                    //Rotation = Quaternion.CreateFromRotationMatrix(transform);

                    //Translation = transform.Translation;


                    //BetterSkinned
                    //transform.Right = Vector3.Normalize(transform.Right);
                    //transform.Up = Vector3.Normalize(transform.Up);
                    //transform.Backward = Vector3.Normalize(transform.Backward);
                    //Scale = transform.Scale;// Calculate.GetCoordinateScale(transform);
                    //Rotation = Quaternion.CreateFromRotationMatrix(transform);
                    //Translation = transform.Translation;



                    transform.Decompose(out Scale, out Rotation, out Translation);
                }
            }

            public Keyframe Clone()
            {
                return (Keyframe)this.MemberwiseClone();
            }
        }

        /// <summary>
        /// Keyframes are grouped per bone for an animation clip
        /// </summary>
        public class Bone
        {
            /// <summary>
            /// Each bone has a name so we can associate it with a runtime model
            /// </summary>
            private string name = "";

            /// <summary>
            /// The keyframes for this bone
            /// </summary>
            private List<Keyframe> keyframes = new List<Keyframe>();

            /// <summary>
            /// The bone name for these keyframes
            /// </summary>
            public string Name { get { return name; } set { name = value; } }

            /// <summary>
            /// The keyframes for this bone
            /// </summary>
            public List<Keyframe> Keyframes { get { return keyframes; } }
        }

        #endregion

        /// <summary>
        /// The bones for this animation
        /// </summary>
        private List<Bone> bones = new List<Bone>();


        /// <summary>
        /// Name of the animation clip
        /// </summary>
        public string Name;

        /// <summary>
        /// Duration of the animation clip
        /// </summary>
        public double Duration;

        /// <summary>
        /// The bones for this animation clip with their keyframes
        /// </summary>
        public List<Bone> Bones { get { return bones; } }


        public void MakeLoopBlend(float blendTime)
        {
            Duration += blendTime;

            foreach(Bone bone in bones)
            {
                //add loop blending (add first frame copy at the end)
                if (bone.Keyframes.Count > 2)
                {
                    AnimationClip.Keyframe keyframe = bone.Keyframes[1].Clone();
                    keyframe.Time = Duration;
                    bone.Keyframes.Add(keyframe);
                }
            }
        }
    }
}
