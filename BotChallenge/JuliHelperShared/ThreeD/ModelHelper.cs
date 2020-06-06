using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace JuliHelper.ThreeD
{
    public static class ModelHelper
    {

        //public static Dictionary<VertexElementUsage, int> vertexChannelSize = new Dictionary<VertexElementUsage, int>()
        //{
        //    { VertexElementUsage.Position, 3 }
        //    { VertexElementUsage., 3 }
        //};

        /// <summary>
        /// transform = null: apply ParentBone.Transform and set it to identity
        /// </summary>
        public static void ApplyTransform(Model model, Matrix? transform = null)
        {
            foreach (ModelMesh mesh in model.Meshes)
                ApplyTransform(mesh, transform);
        }

        /// <summary>
        /// transform = null: apply ParentBone.Transform and set it to identity
        /// </summary>
        public static void ApplyTransform(ModelMesh mesh, Matrix? transform = null)
        {
            foreach (ModelMeshPart meshPart in mesh.MeshParts)
                ApplyTransform(meshPart, transform ?? mesh.ParentBone.Transform);

            if (transform == null)
                mesh.ParentBone.Transform = Matrix.Identity;
        }
        public static void ApplyTransformx(ModelMeshPart meshPart, Matrix transform)
        {

            Matrix cTransform = transform;
            Matrix cTransformInverseTranspose = Matrix.Invert(Matrix.Transpose(cTransform));


            VertexBuffer vertexBuffer = meshPart.VertexBuffer;

            VertexElement[] vs = vertexBuffer.VertexDeclaration.GetVertexElements();
            

            if (EqualUsages(vs, VertexPositionNormalColor.GetUsages()))
            {
                VertexPositionNormalColor[] vertices = new VertexPositionNormalColor[vertexBuffer.VertexCount];
                vertexBuffer.GetData<VertexPositionNormalColor>(vertices);
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].position = Vector3.Transform(vertices[i].position, cTransform);
                    vertices[i].normal = Vector3.Normalize(Vector3.Transform(vertices[i].normal, cTransformInverseTranspose));
                }

                vertexBuffer.SetData<VertexPositionNormalColor>(vertices);
            }
            else if (EqualUsages(vs, GetVertexPositionNormalUsage()))
            {
                int vertexSize = vertexBuffer.VertexDeclaration.VertexStride / 4;
                float[] verticeParts = new float[vertexBuffer.VertexCount * vertexSize];

                vertexBuffer.GetData<float>(verticeParts);

                for (int i = 0; i < verticeParts.Length; i += vertexSize)
                {
                    //transform position and normal
                    Vector3 vec = Vector3.Transform(new Vector3(verticeParts[i], verticeParts[i + 1], verticeParts[i + 2]), cTransform);
                    verticeParts[i] = vec.X;
                    verticeParts[i + 1] = vec.Y;
                    verticeParts[i + 2] = vec.Z;
                    vec = Vector3.Transform(new Vector3(verticeParts[i + 3], verticeParts[i + 4], verticeParts[i + 5]), cTransformInverseTranspose);
                    verticeParts[i + 3] = vec.X;
                    verticeParts[i + 4] = vec.Y;
                    verticeParts[i + 5] = vec.Z;
                    vec.Normalize();

                }

                vertexBuffer.SetData<float>(verticeParts);
            }
            else if (EqualUsages(vs, GetVertexBonesUsage()))
            {
                try
                {
                    int vertexSize = vertexBuffer.VertexDeclaration.VertexStride / 4;
                    float[] verticeParts = new float[vertexBuffer.VertexCount * vertexSize];

                    vertexBuffer.GetData<float>(verticeParts);

                    for (int i = 0; i < verticeParts.Length; i += vertexSize)
                    {
                        //transform position and normal
                        Vector3 vec = Vector3.Transform(new Vector3(verticeParts[i], verticeParts[i + 1], verticeParts[i + 2]), cTransform);
                        verticeParts[i] = vec.X;
                        verticeParts[i + 1] = vec.Y;
                        verticeParts[i + 2] = vec.Z;
                        vec = Vector3.Transform(new Vector3(verticeParts[i + 8], verticeParts[i + 9], verticeParts[i + 10]), cTransformInverseTranspose);
                        vec.Normalize();
                        verticeParts[i + 8] = vec.X;
                        verticeParts[i + 9] = vec.Y;
                        verticeParts[i + 10] = vec.Z;
                    }

                    vertexBuffer.SetData<float>(verticeParts);
                } catch
                {

                }
            }
            else
            {
                //throw new NotImplementedException();
            }
        }
        public static void ApplyTransform(ModelMeshPart meshPart, Matrix transform)
        {
            Matrix cTransform = transform;
            Matrix cTransformInverseTranspose = Matrix.Invert(Matrix.Transpose(cTransform));
            
            //TODO: add Binormal and tangents
            float[][] v = GetVertexData(meshPart, VertexElementUsage.Position, VertexElementUsage.Normal);
            
            for (int i = 0; i < meshPart.NumVertices; i++)
            {
                Vector3 vec = Vector3.Transform(new Vector3(v[0][i], v[1][i], v[2][i]), cTransform);
                v[0][i] = vec.X;
                v[1][i] = vec.Y;
                v[2][i] = vec.Z;
                vec = Vector3.Transform(new Vector3(v[3][i], v[4][i], v[5][i]), cTransformInverseTranspose);
                vec.Normalize();
                v[3][i] = vec.X;
                v[4][i] = vec.Y;
                v[5][i] = vec.Z;
            }

            SetVertexData(meshPart, v, VertexElementUsage.Position, VertexElementUsage.Normal);
        }

        public static float[][] GetVertexData(ModelMeshPart meshPart, params VertexElementUsage[] usages)
        {
            VertexElement[] vs = meshPart.VertexBuffer.VertexDeclaration.GetVertexElements();
            int stride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;

            float[][] v = new float[usages.Length * 3][];
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = new float[meshPart.NumVertices];
                int offset = vs.First(f => f.VertexElementUsage == usages[i / 3]).Offset + (i % 3) * 4;
                meshPart.VertexBuffer.GetData(meshPart.VertexOffset * stride + offset, v[i], 0, v[i].Length, stride);
            }
            return v;
        }
        public static object[][] GetVertexDataObject(ModelMeshPart meshPart, params VertexElementUsage[] usages)
        {
            VertexElement[] vs = meshPart.VertexBuffer.VertexDeclaration.GetVertexElements();
            int stride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
            
            List<object[]> v = new List<object[]>();
            //object[][] v = new object[usages.Length * 3][];
            for (int i = 0; i < usages.Length; i++)
            {
                VertexElement vElement = vs.First(f => f.VertexElementUsage == usages[i]);
                string format = vElement.VertexElementFormat.ToString();
                int elementLength = Convert.ToInt32(format[format.Length - 1].ToString());
                int offset = vElement.Offset;
                for (int j = 0; j < elementLength; j++)
                {
                    if (vElement.VertexElementFormat == VertexElementFormat.Byte4)
                    {
                        byte[] vNew = new byte[meshPart.NumVertices];
                        meshPart.VertexBuffer.GetData<byte>(meshPart.VertexOffset * stride + offset + j, vNew, 0, vNew.Length, stride);
                        v.Add(vNew.Cast<object>().ToArray());
                    }
                    else
                    {
                        float[] vNew = new float[meshPart.NumVertices];
                        meshPart.VertexBuffer.GetData<float>(meshPart.VertexOffset * stride + offset + j * 4, vNew, 0, vNew.Length, stride);
                        v.Add(vNew.Cast<object>().ToArray());
                    }
                }
            }
            return v.ToArray();
        }
        public static void SetVertexData(ModelMeshPart meshPart, float[][] v, params VertexElementUsage[] usages)
        {
            VertexElement[] vs = meshPart.VertexBuffer.VertexDeclaration.GetVertexElements();
            int stride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;

            for (int i = 0; i < v.Length; i++)
            {
                int offset = vs.First(f => f.VertexElementUsage == usages[i / 3]).Offset + (i % 3) * 4;
                meshPart.VertexBuffer.SetData(meshPart.VertexOffset * stride + offset, v[i], 0, v[i].Length, stride);
            }
        }
        
        public static void SetVertexDataObject(ModelMeshPart meshPart, object[][] v, params VertexElementUsage[] usages)
        {
            VertexElement[] vs = meshPart.VertexBuffer.VertexDeclaration.GetVertexElements();
            int stride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;

            int index = 0;

            for (int i = 0; i < usages.Length; i++)
            {
                VertexElement vElement = vs.First(f => f.VertexElementUsage == usages[i]);
                string format = vElement.VertexElementFormat.ToString();
                int elementLength = Convert.ToInt32(format[format.Length - 1].ToString());
                int offset = vElement.Offset;
                for (int j = 0; j < elementLength; j++)
                {
                    if (vElement.VertexElementFormat == VertexElementFormat.Byte4)
                        meshPart.VertexBuffer.SetData<byte>(meshPart.VertexOffset * stride + offset + j, v[index].Cast<byte>().ToArray(), 0, v[index].Length, stride);
                    else
                    {
                        float[] jau = v[index].Cast<float>().ToArray();
                        meshPart.VertexBuffer.SetData<float>(meshPart.VertexOffset * stride + offset + j * 4, v[index].Cast<float>().ToArray(), 0, v[index].Length, stride);
                    }

                    index++;
                }
            }
        }


        //public static int GetVertexSize(VertexElement[] vertexElements)
        //{
        //    string lastFormat = vertexElements[vertexElements.Length - 1].VertexElementFormat.ToString();

        //    int vsize = vertexElements[vertexElements.Length - 1].Offset / 4;
        //    if (lastFormat == "Color")
        //        vsize += 4;
        //    else
        //        vsize += Convert.ToInt32(lastFormat[lastFormat.Length - 1].ToString());

        //    return vsize;
        //}

        //struct Position
        //{
        //    Vector3 position;
        //}


        //struct VertexPositionNormal
        //{
        //    Vector3 position;
        //    Vector3 normal;
        //}

        struct VertexPositionNormalColor
        {
            public Vector3 position;
            public Vector3 normal;
            public Color color;

            public static VertexElementUsage[] GetUsages()
            {
                return new VertexElementUsage[]
                {
                    VertexElementUsage.Position,
                    VertexElementUsage.Normal,
                    VertexElementUsage.Color
                };
            }
        }

        public static VertexElementUsage[] GetVertexPositionNormalUsage()
        {
            return new VertexElementUsage[]
                {
                    VertexElementUsage.Position,
                    VertexElementUsage.Normal
                };
        }
        public static VertexElementUsage[] GetVertexBonesUsage()
        {
            return new VertexElementUsage[]
                {
                    VertexElementUsage.Position,
                    VertexElementUsage.BlendIndices,
                    VertexElementUsage.BlendWeight,
                    VertexElementUsage.Normal,
                    VertexElementUsage.Tangent
                };
        }

        private static bool EqualUsages(VertexElement[] vertexElements, VertexElementUsage[] usages)
        {
            if (usages.Length != vertexElements.Length)
                return false;

            for (int i = 0; i < usages.Length; i++)
            {
                if (vertexElements[i].VertexElementUsage != usages[i])
                    return false;
            }

            return true;
        }

        public static void SetModelTechniques(Model model, string techniqueAppend, RenderMethod renderMethod)
        {
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                //model.Meshes[i].ParentBone.Transform = Matrix.Identity;
                for (int j = 0; j < model.Meshes[i].MeshParts.Count; j++)
                {
                    VertexElement[] d = model.Meshes[i].MeshParts[j].VertexBuffer.VertexDeclaration.GetVertexElements();
                    Effect effect = model.Meshes[i].Effects[j];

                    string renderType = d.Any(f => f.VertexElementUsage == VertexElementUsage.TextureCoordinate) ? "Texture" : "Color";
                    string bone = d.Any(f => f.VertexElementUsage == VertexElementUsage.BlendIndices) ? "Bone" : "";

                    //object[][] v = GetVertexDataObject(model.Meshes[i].MeshParts[j], VertexElementUsage.Position, VertexElementUsage.BlendIndices, VertexElementUsage.BlendWeight);
                    //for (int k = 3; k < 4; k++)
                    //{
                    //    for (int l = 0; l < v[k].Length; l++)
                    //    {
                    //        v[k][l] = (byte)5;
                    //    }
                    //}
                    //SetVertexDataObject(model.Meshes[i].MeshParts[j], v, VertexElementUsage.Position, VertexElementUsage.BlendIndices, VertexElementUsage.BlendWeight);
                    ////v = GetVertexDataObject(model.Meshes[i].MeshParts[j], VertexElementUsage.Position, VertexElementUsage.BlendIndices, VertexElementUsage.BlendWeight);

                    if (effect.CurrentTechnique != effect.Techniques[renderMethod.ToString() + renderType + bone])
                        effect.CurrentTechnique = effect.Techniques[renderMethod.ToString() + renderType + bone];

                    //if (techniqueAppend == "Bone" && bone == "")
                    {
                        //if (i >= 5 && i <= 8)
                        //    ModelHelper.ApplyTransform(model.Meshes[i].MeshParts[j], Matrix.CreateScale(10, -10, -10));
                    }
                }

            }
        }
    }
}
