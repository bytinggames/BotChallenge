using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class MeshBatchItem
    {
        public ModelMeshPart meshPart;
        public Effect effect;
        public Matrix world;
        public float depth;

        public MeshBatchItem(ModelMeshPart meshPart, Effect effect, Matrix world, float depth)
        {
            this.meshPart = meshPart;
            this.effect = effect;
            this.world = world;
            this.depth = depth;
        }

        public virtual void Render(GraphicsDevice gDevice)
        {
            if (meshPart.PrimitiveCount > 0)
            {
                effect.SetWorldAndInvTransp(world);
                                                    
                gDevice.SetVertexBuffer(meshPart.VertexBuffer);
                gDevice.Indices = meshPart.IndexBuffer;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                           meshPart.VertexOffset,
                                                           meshPart.StartIndex,
                                                           meshPart.PrimitiveCount);

                }
            }
        }
        public static string name = "";
        public static Matrix parent;
        public void CalculateDepthFromFirstVertex()
        {
            float[] v = new float[1];
            int stride = meshPart.VertexBuffer.VertexDeclaration.VertexStride; //size in bytes for each vertex
            meshPart.VertexBuffer.GetData<float>(8 + stride * meshPart.VertexOffset, v, 0, 1, 0); //1 vertices: 14 floats 56 bytes (stride)
            //(8 = 4*2 = (floatsize)*(z-offset)) + ...
            depth = v[0];

            //meshPart.VertexBuffer.GetData<float>(w, 0, meshPart.VertexBuffer.VertexCount * meshPart.VertexBuffer.VertexDeclaration.VertexStride / 4); //get all data
            //System.IO.File.WriteAllLines("D:\\Desktop\\vertices.txt", w.Select(f => f.ToString()));

            //Vector3 d = Vector3.Transform(new Vector3(0, 0, depth), parent * Matrix.CreateScale(0.1f, 0.1f, -1));

            //depth = d.Z;
        }

        public void CalculateDepthFromAverage()
        {
            float[] v = new float[meshPart.NumVertices];
            int stride = meshPart.VertexBuffer.VertexDeclaration.VertexStride; //size in bytes for each vertex
            meshPart.VertexBuffer.GetData<float>(8 + stride * meshPart.VertexOffset, v, 0, meshPart.NumVertices, stride); //1 vertices: 14 floats 56 bytes (stride)
            
            depth = v.Average();
        }
    }
}
