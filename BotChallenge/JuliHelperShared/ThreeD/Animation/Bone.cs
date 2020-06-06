using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace JuliHelper.ThreeD
{
    /// <summary>
    /// Bones in this model are represented by this class, which
    /// allows a bone to have more detail associatd with it.
    /// 
    /// This class allows you to manipulate the local coordinate system
    /// for objects by changing the scaling, translation, and rotation.
    /// These are indepenent of the bind transformation originally supplied
    /// for the model. So, the actual transformation for a bone is
    /// the product of the:
    /// 
    /// Scaling
    /// Bind scaling (scaling removed from the bind transform)
    /// Rotation
    /// Translation
    /// Bind Transformation
    /// Parent Absolute Transformation
    /// 
    /// </summary>
	public class Bone
    {
        #region Fields

        /// <summary>
        /// Any parent for this bone
        /// </summary>
        private Bone parent = null;

        /// <summary>
        /// The children of this bone
        /// </summary>
        private List<Bone> children = new List<Bone>();

        /// <summary>
        /// The bind transform is the transform for this bone
        /// as loaded from the original model. It's the base pose.
        /// I do remove any scaling, though.
        /// </summary>
        private Matrix bindTransform = Matrix.Identity;

        ///// <summary>
        ///// The bind scaling component extracted from the bind transform
        ///// </summary>
        //private Vector3 bindScale = Vector3.One;

        /// <summary>
        /// Any translation applied to the bone
        /// </summary>
        private Vector3 translation = Vector3.Zero;

        /// <summary>
        /// Any rotation applied to the bone
        /// </summary>
        private Quaternion rotation = Quaternion.Identity;

        /// <summary>
        /// Any scaling applied to the bone
        /// </summary>
        private Vector3 scale = Vector3.One;

        #endregion 

        #region Properties

        /// <summary>
        /// The bone name
        /// </summary>
		public string Name = "";

        /// <summary>
        /// The bone bind transform
        /// </summary>
        public Matrix BindTransform { get {return bindTransform;} }

        /// <summary>
        /// Inverse of absolute bind transform for skinnning
        /// </summary>
        public Matrix SkinTransform { get; set; }

        /// <summary>
        /// Bone rotation
        /// </summary>
		public Quaternion Rotation {get {return rotation;} set {rotation = value;}}

        /// <summary>
        /// Any translations
        /// </summary>
		public Vector3 Translation {get {return translation;} set {translation = value;}}

        /// <summary>
        /// Any scaling
        /// </summary>
        public Vector3 Scale { get { return scale; } set { scale = value; } }

        /// <summary>
        /// The parent bone or null for the root bone
        /// </summary>
        public Bone Parent { get { return parent; } }

        /// <summary>
        /// The children of this bone
        /// </summary>
        public List<Bone> Children { get { return children; } }

        /// <summary>
        /// The bone absolute transform
        /// </summary>
        public Matrix AbsoluteTransform = Matrix.Identity;

        #endregion

        #region Operations

        /// <summary>
        /// Constructor for a bone object
        /// </summary>
        /// <param name="name">The name of the bone</param>
        /// <param name="bindTransform">The initial bind transform for the bone</param>
        /// <param name="parent">A parent for this bone</param>
        public Bone(string name, Matrix bindTransform, Bone parent)
        {
            this.Name = name;
            this.parent = parent;
            if (parent != null)
                parent.children.Add(this);
            //else
            //    bindTransform = Matrix.CreateScale(0.10f, -0.10f, -0.10f) * bindTransform; //TODO: check
            
            //TODO: check
            //if (Name.StartsWith("b_oberarm"))
            //{
            //    bindTransform.Translation = new Vector3(bindTransform.Translation.X, bindTransform.Translation.Y, 0);
            //}

            //Quaternion rotation;
            //Vector3 x, y;
            //bindTransform.Decompose(out x, out rotation, out y);

            //if (Name == "Armature")
            //    bindTransform = Matrix.CreateScale(100);

            //bindTransform *= Matrix.CreateRotationY(MathHelper.PiOver4);//Matrix.CreateRotationX(-rotation.X) * Matrix.CreateRotationY(-rotation.Y);
            this.bindTransform = bindTransform;
        }

        /// <summary>
        /// Compute the absolute transformation for this bone.
        /// </summary>

        public bool ComputeAbsoluteTransformRecursive(Vector3 absolutePos) //returns if animations have uniform scaling
        {
            bool uniformScaling = true;

            Matrix transform = Matrix.CreateScale(Scale) *
                Matrix.CreateFromQuaternion(Rotation) *
                Matrix.CreateTranslation(Translation) *
                BindTransform;
            

            if (Parent != null)
            {
                // This bone has a parent bone
                AbsoluteTransform = transform * Parent.AbsoluteTransform;
            }
            else
            {   // The root bone
                AbsoluteTransform = transform;
            }
            
            uniformScaling = uniformScaling && scale.X == scale.Y && scale.X == scale.Z;

            foreach (Bone child in children)
            {
                bool childUniform = child.ComputeAbsoluteTransformRecursive(absolutePos);
                if (uniformScaling && !childUniform)
                    uniformScaling = false;
            }

            return uniformScaling;
        }

        /// <summary>
        /// This sets the rotation and translation such that the
        /// rotation times the translation times the bind after set
        /// equals this matrix. This is used to set animation values.
        /// </summary>
        /// <param name="m">A matrix include translation and rotation</param>
        public void SetCompleteTransform(Vector3 s, Quaternion r, Vector3 t)
        {
            scale = s;
            rotation = r;
            translation = t;

            Matrix m = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(translation);

            Matrix setTo = m * Matrix.Invert(BindTransform);
            setTo.Decompose(out scale, out rotation, out translation);
            
            /*if (Global2.randomAnimation > 0)
            {
                scale = Vector3.One;
                translation = Vector3.Zero;
                rotation = Quaternion.Identity;
                if (Parent != null && Parent.Parent != null && Parent.Parent.Parent != null && Parent.Parent.Parent.Parent != null)
                {
                    Random rand = Global2.frameRandom;
                    if (Global2.randomAnimation == 1 || Global2.randomAnimation == 3)
                    {
                        rotation = Quaternion.CreateFromAxisAngle(Vector3.Normalize(new Vector3((float)rand.NextDouble() - 0.5f, (float)rand.NextDouble() - 0.5f, (float)rand.NextDouble() - 0.5f)),
                            MathHelper.PiOver2 * (float)Math.Pow(rand.NextDouble(), 1f) * (float)(Math.Sin(Game1.frame / 25f * (float)rand.NextDouble() + (float)rand.NextDouble() * MathHelper.TwoPi)));
                    }
                    if (Global2.randomAnimation == 2 || Global2.randomAnimation == 3)
                        scale = new Vector3(
                            1 + 0.5f * (float)Math.Pow(rand.NextDouble(), 3) * (float)(Math.Sin(Game1.frame / 25f * (float)rand.NextDouble() + (float)rand.NextDouble() * MathHelper.TwoPi)),
                            1 + 0.5f * (float)Math.Pow(rand.NextDouble(), 3) * (float)(Math.Sin(Game1.frame / 25f * (float)rand.NextDouble() + (float)rand.NextDouble() * MathHelper.TwoPi)),
                            1 + 0.5f * (float)Math.Pow(rand.NextDouble(), 3) * (float)(Math.Sin(Game1.frame / 25f * (float)rand.NextDouble() + (float)rand.NextDouble() * MathHelper.TwoPi)));
                    //if (Global2.randomAnimation == 3)
                    //    translation = new Vector3(
                    //        (float)Math.Pow(rand.NextDouble(), 4) * (float)(Math.Sin(Game1.frame / 25f * (float)rand.NextDouble() + (float)rand.NextDouble() * MathHelper.TwoPi)),
                    //        (float)Math.Pow(rand.NextDouble(), 4) * (float)(Math.Sin(Game1.frame / 25f * (float)rand.NextDouble() + (float)rand.NextDouble() * MathHelper.TwoPi)),
                    //        (float)Math.Pow(rand.NextDouble(), 4) * (float)(Math.Sin(Game1.frame / 25f * (float)rand.NextDouble() + (float)rand.NextDouble() * MathHelper.TwoPi)));

                }
            }

            if (Global2.lookAtMe)
            {
                //look at me...
                if ((Name == "CenterNeck" || Name =="head") && Ingame.player != null)
                {
                    Vector3 cPos = Global2.currentModelPos + Vector3.Transform(Vector3.Zero, Parent.AbsoluteTransform);

                    Vector3 dist = Ingame.player.GetHeadPos() - cPos;

                    float yaw = +MathHelper.PiOver2 - (float)Math.Atan2(dist.Z, dist.X);
                    //float pitch = -(float)Math.Atan2(dist.Y, Math.Sqrt(dist.X * dist.X + dist.Z * dist.Z));
                    //rotation = Quaternion.CreateFromYawPitchRoll(yaw, 0, 0);//Game1.frame / 25f, 0, 0);

                    Quaternion parentRotation;

                    Parent.AbsoluteTransform.Decompose(out scale, out parentRotation, out translation);

                    //rotation = Quaternion.Identity;
                    rotation = Quaternion.Inverse(parentRotation);// * rotation;

                    scale = Vector3.One;
                    translation = Vector3.Zero;
                }
                else if (AnyParentName("CenterNeck"))
                    rotation = Quaternion.Identity;
            }*/

            //if (Name == "pelvis")
            //    translation = Vector3.Zero;

            //rotation = Quaternion.Identity;
            //translation = Vector3.Zero;
            //scale = Vector3.One;
            //if (Name.Contains("arm") || Name.Contains("thigh"))// == "lowerarm_r")
            //    rotation = Quaternion.CreateFromYawPitchRoll(0, 0, Game1.frame * 0.01f);

        }

        public void AddTransform(Vector3 scale, Quaternion rotation, Vector3 translation, float amount)
        {
            this.scale += scale * amount;
            this.rotation += rotation * amount;
            this.translation += translation * amount;
        }

        #endregion

        public override string ToString()
        {
            return Name + " abs: " + AbsoluteTransform.ToMyString();
            //return Name + " t:" + translation + " r:" + rotation + " s:" + scale + " bind:" + bindTransform.ToMyString();
        }

        private bool AnyParentName(string name)
        {
            if (Parent == null)
                return false;
            else if (Parent.Name == name)
                return true;
            else
                return Parent.AnyParentName(name);
        }

        internal void InitTransform()
        {
            scale = Vector3.Zero;
            rotation = new Quaternion(0,0,0,0);
            translation = Vector3.Zero;
        }
    }
}
