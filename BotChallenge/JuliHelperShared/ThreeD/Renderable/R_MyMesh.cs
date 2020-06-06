using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.ThreeD
{
    public struct Vertex
    {
        public Vector3 pos;
        public Vector3 normal;
        public Vector2 texCoord;
        public Vector3 tangent;
        public Vector3 binormal;
    }

    public class R_MyMesh : Renderable
    {
        public Matrix world;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        Effect effect;

        public float diffuseMapIntensity, normalMapIntensity, specularMapIntensity, glossMapIntensity, specularBaseIntensity, glossBaseIntensity;
        public bool mipMap;

        Texture2D diffuseMap, normalMap, specularMap, glossMap;

        public R_MyMesh(ModelMeshPart meshPart, Matrix world, Action<Vertex[]> action = null)
        {
            this.effect = meshPart.Effect;
            this.world = world;

            //Vertex[] vertices;
            //short[] indices;

            ////VertexElement[] vs = meshPart.VertexBuffer.VertexDeclaration.GetVertexElements();
            ////string lastFormat = vs[vs.Length - 1].VertexElementFormat.ToString();

            ////int vsize = vs[vs.Length - 1].Offset / 4;
            ////if (lastFormat == "Color")
            ////    vsize += 4;
            ////else
            ////    vsize += Convert.ToInt32(lastFormat[lastFormat.Length - 1].ToString());

            //int length = meshPart.PrimitiveCount * 3; //* 3 because triangles


            //vertices = new Vertex[length];
            //meshPart.VertexBuffer.GetData<Vertex>(vertices);

            //if (action != null)
            //    action(vertices);

            //indices = new short[meshPart.IndexBuffer.IndexCount];
            //meshPart.IndexBuffer.GetData<short>(indices);

            //vertexBuffer = new VertexBuffer(DrawM.gDevice, meshPart.VertexBuffer.VertexDeclaration, vertices.Length, BufferUsage.None);
            //vertexBuffer.SetData<Vertex>(vertices);

            //indexBuffer = new IndexBuffer(DrawM.gDevice, meshPart.IndexBuffer.IndexElementSize, indices.Length, BufferUsage.None);
            //indexBuffer.SetData<short>(indices);

            Initialize();
        }

        public R_MyMesh(Vertex[] vertices, Effect effect, Matrix world, short[] indices = null)
        {
            vertexBuffer = new VertexBuffer(DrawM.gDevice, new VertexDeclaration(new VertexElement[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                new VertexElement(44, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)
            }), vertices.Length, BufferUsage.None);

            this.vertexBuffer.SetData<Vertex>(vertices);
            this.effect = effect;

            this.indexBuffer = new IndexBuffer(DrawM.gDevice, IndexElementSize.SixteenBits, vertices.Length, BufferUsage.None);

            if (indices == null)
            {
                indices = new short[vertices.Length];
                for (short i = 0; i < indices.Length; i++)
                    indices[i] = i;
            }

            this.indexBuffer.SetData<short>(indices);
            this.world = world;

            Initialize();
        }

        private void Initialize()
        {
            diffuseMapIntensity = normalMapIntensity = specularMapIntensity = glossMapIntensity = 0;
            specularBaseIntensity = 0.5f;
            glossBaseIntensity = 0.1f;
        }

        public void LoadTexturesFromContent(string texName, bool mipMap, float diffuseMapIntensity = 0, float normalMapIntensity = 0, float specularMapIntensity = 0, float glossMapIntensity = 0)
        {
            this.diffuseMapIntensity = diffuseMapIntensity;
            this.normalMapIntensity = normalMapIntensity;
            this.specularMapIntensity = specularMapIntensity;
            this.glossMapIntensity = glossMapIntensity;

            this.mipMap = mipMap;
            string mipString = (mipMap ? "_mip" : "");

            if (diffuseMapIntensity != 0)
                diffuseMap = ContentLoader.textures[texName + mipString];
            if (normalMapIntensity != 0)
                normalMap = ContentLoader.textures[texName + "_n" + mipString];
            if (specularMapIntensity != 0)
                specularMap = ContentLoader.textures[texName + "_s" + mipString];
            if (glossMapIntensity != 0)
                glossMap = ContentLoader.textures[texName + "_g" + mipString];
        }


        public override void ForEachEffect(Action<Effect> action)
        {
            action(effect);
        }

        public R_MyMesh Clone()
        {
            R_MyMesh clone = (R_MyMesh)MemberwiseClone();

            return clone;
        }

        public static Vertex[] CreateGrid(Vector3 origin, Vector3 xDir, Vector3 yDir, int vx, int vy, Vector2 texCoordScale, Random rand)
        {

            Vertex[] vertices = new Vertex[vx * vy * 6];

            int th = vy; //texture height
            int tw = vx; //texture width

            for (int y = 0; y < vy; y++)
            {
                for (int x = 0; x < vx; x++)
                {
                    bool flip = rand.Next(2) == 0;

                    float x2 = (float)x / vx;
                    float y2 = (float)y / vy;

                    int i = y * vx * 6 + x * 6;

                    vertices[i] = new Vertex()
                    {
                        pos = origin + xDir * x2 + yDir * y2,
                        texCoord = new Vector2((float)x / tw, (float)y / th) * texCoordScale
                    };
                    vertices[i + 4] = new Vertex()
                    {
                        pos = origin + xDir * x2 + yDir * (y + 1) / vy,
                        texCoord = new Vector2((float)x / tw, (float)(y + 1) / th) * texCoordScale
                    };
                    vertices[i + 2] = new Vertex()
                    {
                        pos = origin + xDir * (x + 1) / vx + yDir * y2,
                        texCoord = new Vector2((float)(x + 1) / tw, (float)y / th) * texCoordScale
                    };
                    vertices[i + 5] = new Vertex()
                    {
                        pos = origin + xDir * (x + 1) / vx + yDir * (y + 1) / vy,
                        texCoord = new Vector2((float)(x + 1) / tw, (float)(y + 1) / th) * texCoordScale
                    };

                    if (!flip)
                    {
                        vertices[i + 1] = vertices[i + 4];
                        vertices[i + 3] = vertices[i + 2];
                    }
                    else
                    {
                        vertices[i + 1] = vertices[i + 5];
                        vertices[i + 3] = vertices[i];
                    }
                }
            }

            VertexHelper.RecalculateNormals(vertices, -1);
            VertexHelper.RecalculateTangentsAndBinormals(vertices);

            return vertices;
        }

        public static Vertex[] CreateGrid(Vector3 origin, Vector3 xDir, Vector3 yDir, int vx, int vy, out short[] indices, Vector2 texCoordScale)
        {
            bool flip = false;

            Vertex[] vertices = new Vertex[vx * vy * 4];

            indices = new short[vx * vy * 6];

            int th = vy - 1; //texture height
            int tw = vx - 1; //texture width

            int j = 0;

            for (int y = 0; y < vy; y++)
            {
                for (int x = 0; x < vx; x++)
                {
                    float x2 = (float)x / vx;
                    float y2 = (float)y / vy;

                    short i = (short)(y * vx * 4 + x * 4);

                    vertices[i] = new Vertex()
                    {
                        pos = origin + xDir * x2 + yDir * y2,
                        texCoord = new Vector2((float)x / tw, (float)y / th) * texCoordScale
                    };
                    vertices[i + 1] = new Vertex()
                    {
                        pos = origin + xDir * x2 + yDir * (y + 1) / vy,
                        texCoord = new Vector2((float)x / tw, (float)(y + 1) / th) * texCoordScale
                    };
                    vertices[i + 2] = new Vertex()
                    {
                        pos = origin + xDir * (x + 1) / vx + yDir * y2,
                        texCoord = new Vector2((float)(x + 1) / tw, (float)y / th) * texCoordScale
                    };
                    vertices[i + 3] = new Vertex()
                    {
                        pos = origin + xDir * (x + 1) / vx + yDir * (y + 1) / vy,
                        texCoord = new Vector2((float)(x + 1) / tw, (float)(y + 1) / th) * texCoordScale
                    };

                    indices[j++] = i;
                    indices[j++] = (short)(i + (flip ? 3 : 1));
                    indices[j++] = (short)(i + 2);
                    indices[j++] = (short)(i + (flip ? 0 : 2));
                    indices[j++] = (short)(i + 1);
                    indices[j++] = (short)(i + 3);


                    /*
                    vertices[i] = new Vertex()
                    {
                        pos = origin + xDir * x2 + yDir * y2,
                        texCoord = new Vector2((float)x / tw, (float)y / th)
                    };
                    vertices[i + 4] = new Vertex()
                    {
                        pos = origin + xDir * x2 + yDir * (y + 1) / vy,
                        texCoord = new Vector2((float)x / tw, (float)(y + 1) / th)
                    };
                    vertices[i + 2] = new Vertex()
                    {
                        pos = origin + xDir * (x + 1) / vx + yDir * y2,
                        texCoord = new Vector2((float)(x + 1) / tw, (float)y / th)
                    };
                    vertices[i + 5] = new Vertex()
                    {
                        pos = origin + xDir * (x + 1) / vx + yDir * (y + 1) / vy,
                        texCoord = new Vector2((float)(x + 1) / tw, (float)(y + 1) / th)
                    };*/
                    //if (!flip)
                    //{
                    //    vertices[i + 1] = vertices[i + 4];
                    //    vertices[i + 3] = vertices[i + 2];
                    //}
                    //else
                    //{
                    //    vertices[i + 1] = vertices[i + 5];
                    //    vertices[i + 3] = vertices[i];
                    //}
                }
            }

            VertexHelper.RecalculateNormals(vertices, indices, -1);
            VertexHelper.RecalculateTangentsAndBinormals(vertices, indices);

            return vertices;
        }

        public void RecalculateNormalsSoft(float softness)
        {
            //VertexElement[] vs = vertexBuffer.VertexDeclaration.GetVertexElements();
            //string lastFormat = vs[vs.Length - 1].VertexElementFormat.ToString();
            //int vsize = vs[vs.Length - 1].Offset / 4 + Convert.ToInt32(lastFormat[lastFormat.Length - 1].ToString());


            Vertex[] vertices = new Vertex[vertexBuffer.VertexCount];
            vertexBuffer.GetData<Vertex>(vertices);


            VertexHelper.RecalculateNormalsSoft(vertices, softness, 1);

            //vertexBuffer = new VertexBuffer(DrawM.gDevice, vertexBuffer.VertexDeclaration, vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<Vertex>(vertices);
        }


        public override void SetWorld(Matrix world)
        {
            this.world = world;
        }

        public override void Transform(Matrix matrix)
        {
            this.world *= matrix;
        }

        public override void Render(MeshBatch meshBatch, Vector3 pos)
        {
             Render(meshBatch, Matrix.CreateTranslation(pos));
        }

        public override void Render(MeshBatch meshBatch, Matrix transform)//TODO: implement meshBatch
        {
            Matrix newWorld = world;
            //newWorld  *= Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2);
            newWorld *= transform;

            effect.SetWorldAndInvTransp(newWorld);

            if (diffuseMapIntensity != 0)
                effect.Parameters["Texture"].SetValue(diffuseMap);
            if (normalMapIntensity != 0)
                effect.Parameters["NormalMap"].SetValue(normalMap);
            if (specularMapIntensity != 0)
                effect.Parameters["SpecularMap"].SetValue(specularMap);
            if (glossMapIntensity != 0)
                effect.Parameters["GlossMap"].SetValue(glossMap);

            effect.Parameters["useMipMap"].SetValue(mipMap);
            effect.Parameters["diffuseMapIntensity"].SetValue(diffuseMapIntensity);
            effect.Parameters["normalMapIntensity"].SetValue(normalMapIntensity);
            effect.Parameters["specularMapIntensity"].SetValue(specularMapIntensity);
            effect.Parameters["glossMapIntensity"].SetValue(glossMapIntensity);
            effect.Parameters["specularBaseIntensity"].SetValue(specularBaseIntensity);
            effect.Parameters["glossBaseIntensity"].SetValue(glossBaseIntensity);

            DrawM.gDevice.SetVertexBuffer(vertexBuffer);
            DrawM.gDevice.Indices = indexBuffer;

            //effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(mpEffect.World)));
            //effect.Parameters["WorldViewProjection"].SetValue(mpEffect.World * Game1.camera.view * Game1.camera.projection);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                DrawM.gDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                        0,
                                                        0,
                                                        vertexBuffer.VertexCount / 3);

            }
        }
    }
}
