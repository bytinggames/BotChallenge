using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.ThreeD
{
    public static class VertexHelper
    {
        public static void RecalculateNormals(Vertex[] vertices, int sign = 1)
        {
            for (var i = 0; i < vertices.Length; i += 3)
            {
                var ia = i;
                var ib = i + 1;
                var ic = i + 2;

                var aa = vertices[ia].pos;
                var bb = vertices[ib].pos;
                var cc = vertices[ic].pos;

                vertices[ia].normal = vertices[ib].normal = vertices[ic].normal = sign * Vector3.Normalize(Vector3.Cross(bb - cc, bb - aa));
            }
        }
        public static void RecalculateNormals(Vertex[] vertices, short[] indices, int sign)
        {
            for (var i = 0; i < indices.Length; i += 3)
            {
                var ia = indices[i];
                var ib = indices[i + 1];
                var ic = indices[i + 2];

                var aa = vertices[ia].pos;
                var bb = vertices[ib].pos;
                var cc = vertices[ic].pos;

                vertices[ia].normal = vertices[ib].normal = vertices[ic].normal = sign * Vector3.Normalize(Vector3.Cross(bb - cc, bb - aa));
            }
        }
        /*public static void RecalculateNormalsSoft(Vertex[] vertices, float softness, int sign, GridType gridType)
        {

            RecalculateNormals(vertices, sign);

            if (softness > 0)
            {
                bool[] used = new bool[vertices.Length];

                for (int i = 0; i < vertices.Length; i++)
                {
                    if (!used[i])
                    {
                        List<int> indices = new List<int>();

                        indices.Add(i);

                        Vector3 normal = vertices[i].normal;

                        for (int j = i + 1; j < vertices.Length; j++)
                        {
                            Vector3 dist = (vertices[j].pos - vertices[i].pos).AbsVector();

                            if ((dist.Y < 0.01f || (gridType == GridType.CeilingFloor && dist.Y > 1.99f)) && (dist.Z < 0.01f || dist.Z > 1.99f) && (dist.X < 0.01f))
                            //if (Vector3.Distance(vertices[i].pos, vertices[j].pos) < 0.001f || (wall && Math.Abs(vertices[i].pos.Z) == 1 && Math.Abs(vertices[j].pos.Z) == 1 && Math.Abs(vertices[i].pos.Y - vertices[j].pos.Y) < 0.001f))
                            {
                                indices.Add(j);

                                normal += vertices[j].normal;

                                used[j] = true;
                            }
                        }

                        if (gridType != GridType.CeilingFloor)
                        {
                            if (vertices[i].pos.Y == 0)
                                normal = new Vector3(0, 1, 0);
                            else if (vertices[i].pos.Y == 3)
                                normal = new Vector3(0, -1, 0);
                        }
                        if (gridType == GridType.InnerCorner)
                        {
                            if (vertices[i].pos.Z == 2)
                            {
                                normal.Z = 0;
                                //normal += new Vector3(0.5f, 0, 0);
                            }
                            else if (vertices[i].pos.X == 2)
                            {
                                normal.X = 0;
                                //normal += new Vector3(0, 0, 0.5f);
                            }
                        }


                        normal.Normalize();
                        for (int j = 0; j < indices.Count; j++)
                        {
                            Vector3 newNormal = softness * normal + (1 - softness) * vertices[indices[j]].normal;
                            newNormal.Normalize();
                            vertices[indices[j]].normal = newNormal;
                        }
                    }
                }
            }
        }*/
        public static void RecalculateNormalsSoft(Vertex[] vertices, float softness, int sign)
        {

            RecalculateNormals(vertices, sign);

            if (softness > 0)
            {
                bool[] used = new bool[vertices.Length];

                for (int i = 0; i < vertices.Length; i++)
                {
                    if (!used[i])
                    {
                        List<int> indices = new List<int>();

                        indices.Add(i);

                        Vector3 normal = vertices[i].normal;

                        for (int j = i + 1; j < vertices.Length; j++)
                        {
                            Vector3 dist = (vertices[j].pos - vertices[i].pos).AbsVector();

                            if (dist.X < 0.01f && dist.Y < 0.01f && dist.Z < 0.01f)
                            {
                                indices.Add(j);

                                normal += vertices[j].normal;

                                used[j] = true;
                            }
                        }


                        normal.Normalize();
                        for (int j = 0; j < indices.Count; j++)
                        {
                            Vector3 newNormal = softness * normal + (1 - softness) * vertices[indices[j]].normal;
                            newNormal.Normalize();
                            vertices[indices[j]].normal = newNormal;
                        }
                    }
                }
            }
        }

        public static void RecalculateTangentsAndBinormals(Vertex[] vertices)
        {
            for (int i1 = 0; i1 < vertices.Length; i1 += 3)
            {
                int i2 = i1 + 1;
                int i3 = i1 + 2;

                //int start = (i1 / 3) * 3;
                //int i2 = start + (i1 + 1) % 3;
                //int i3 = start + (i1 + 2) % 3;

                Vector3 A = vertices[i1].pos;
                Vector3 B = vertices[i2].pos;
                Vector3 C = vertices[i3].pos;
                Vector2 w1 = vertices[i1].texCoord;
                Vector2 w2 = vertices[i2].texCoord;
                Vector2 w3 = vertices[i3].texCoord;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float denom = s1 * t2 - s2 * t1;



                var r = 1.0f / denom;

                var v1 = vertices[i1].pos;
                var v2 = vertices[i2].pos;
                var v3 = vertices[i3].pos;

                var x1 = v2.X - v1.X;
                var x2 = v3.X - v1.X;
                var y1 = v2.Y - v1.Y;
                var y2 = v3.Y - v1.Y;
                var z1 = v2.Z - v1.Z;
                var z2 = v3.Z - v1.Z;

                Vector3 sdir = new Vector3()
                {
                    X = (t2 * x1 - t1 * x2) * r,
                    Y = (t2 * y1 - t1 * y2) * r,
                    Z = (t2 * z1 - t1 * z2) * r,
                };

                Vector3 tdir = new Vector3()
                {
                    X = (s1 * x2 - s2 * x1) * r,
                    Y = (s1 * y2 - s2 * y1) * r,
                    Z = (s1 * z2 - s2 * z1) * r,
                };

                vertices[i1].tangent = sdir;
                vertices[i2].tangent = sdir;
                vertices[i3].tangent = sdir;
                vertices[i1].binormal = tdir;
                vertices[i2].binormal = tdir;
                vertices[i3].binormal = tdir;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                var n = vertices[i].normal;

                var t = vertices[i].tangent;
                if (t.LengthSquared() < float.Epsilon)
                {
                    // TODO: Ideally we could spit out a warning to the
                    // content logging here!

                    // We couldn't find a good tanget for this vertex.
                    //
                    // Rather than set them to zero which could produce
                    // errors in other parts of the pipeline, we just take        
                    // a guess at something that may look ok.

                    t = Vector3.Cross(n, Vector3.UnitX);
                    if (t.LengthSquared() < float.Epsilon)
                        t = Vector3.Cross(n, Vector3.UnitY);

                    vertices[i].tangent = Vector3.Normalize(t);
                    vertices[i].binormal = Vector3.Cross(n, vertices[i].tangent);
                    continue;
                }

                // Gram-Schmidt orthogonalize
                // TODO: This can be zero can cause NaNs on 
                // normalize... how do we fix this?
                var tangent = t - n * Vector3.Dot(n, t);
                tangent = Vector3.Normalize(tangent);
                vertices[i].tangent = tangent;

                // Calculate handedness
                var w = (Vector3.Dot(Vector3.Cross(n, t), vertices[i].binormal) < 0.0F) ? -1.0F : 1.0F;


                // Calculate the bitangent
                var bitangent = Vector3.Cross(n, tangent) * w;
                vertices[i].binormal = bitangent;
            }
        }

        public static void RecalculateTangentsAndBinormals(Vertex[] vertices, short[] indices)
        {
            for (int i = 0; i < indices.Length; i += 3)
            {
                int i1 = indices[i];
                int i2 = indices[i + 1];
                int i3 = indices[i + 2];

                //int start = (i1 / 3) * 3;
                //int i2 = start + (i1 + 1) % 3;
                //int i3 = start + (i1 + 2) % 3;

                Vector3 A = vertices[i1].pos;
                Vector3 B = vertices[i2].pos;
                Vector3 C = vertices[i3].pos;
                Vector2 w1 = vertices[i1].texCoord;
                Vector2 w2 = vertices[i2].texCoord;
                Vector2 w3 = vertices[i3].texCoord;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float denom = s1 * t2 - s2 * t1;



                var r = 1.0f / denom;

                var v1 = vertices[i1].pos;
                var v2 = vertices[i2].pos;
                var v3 = vertices[i3].pos;

                var x1 = v2.X - v1.X;
                var x2 = v3.X - v1.X;
                var y1 = v2.Y - v1.Y;
                var y2 = v3.Y - v1.Y;
                var z1 = v2.Z - v1.Z;
                var z2 = v3.Z - v1.Z;

                Vector3 sdir = new Vector3()
                {
                    X = (t2 * x1 - t1 * x2) * r,
                    Y = (t2 * y1 - t1 * y2) * r,
                    Z = (t2 * z1 - t1 * z2) * r,
                };

                Vector3 tdir = new Vector3()
                {
                    X = (s1 * x2 - s2 * x1) * r,
                    Y = (s1 * y2 - s2 * y1) * r,
                    Z = (s1 * z2 - s2 * z1) * r,
                };

                vertices[i1].tangent = sdir;
                vertices[i2].tangent = sdir;
                vertices[i3].tangent = sdir;
                vertices[i1].binormal = tdir;
                vertices[i2].binormal = tdir;
                vertices[i3].binormal = tdir;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                var n = vertices[i].normal;

                var t = vertices[i].tangent;
                if (t.LengthSquared() < float.Epsilon)
                {
                    // TODO: Ideally we could spit out a warning to the
                    // content logging here!

                    // We couldn't find a good tanget for this vertex.
                    //
                    // Rather than set them to zero which could produce
                    // errors in other parts of the pipeline, we just take        
                    // a guess at something that may look ok.

                    t = Vector3.Cross(n, Vector3.UnitX);
                    if (t.LengthSquared() < float.Epsilon)
                        t = Vector3.Cross(n, Vector3.UnitY);

                    vertices[i].tangent = Vector3.Normalize(t);
                    vertices[i].binormal = Vector3.Cross(n, vertices[i].tangent);
                    continue;
                }

                // Gram-Schmidt orthogonalize
                // TODO: This can be zero can cause NaNs on 
                // normalize... how do we fix this?
                var tangent = t - n * Vector3.Dot(n, t);
                tangent = Vector3.Normalize(tangent);
                vertices[i].tangent = tangent;

                // Calculate handedness
                var w = (Vector3.Dot(Vector3.Cross(n, t), vertices[i].binormal) < 0.0F) ? -1.0F : 1.0F;


                // Calculate the bitangent
                var bitangent = Vector3.Cross(n, tangent) * w;
                vertices[i].binormal = bitangent;
            }
        }

        public static Model CloneModel(Model model)
        {

            List<ModelMesh> meshes = new List<ModelMesh>();

            foreach (var m in model.Meshes)
            {
                List<ModelMeshPart> parts = new List<ModelMeshPart>();
                foreach (var mp in m.MeshParts)
                {
                    //VertexElement[] vs = mp.VertexBuffer.VertexDeclaration.GetVertexElements();
                    //string lastFormat = vs[vs.Length - 1].VertexElementFormat.ToString();
                    //int vsize = vs[vs.Length - 1].Offset / 4 + Convert.ToInt32(lastFormat[lastFormat.Length - 1].ToString());

                    ////int h = (int)(mp.NumVertices * (4f + 2f / 3f));
                    //int length = mp.PrimitiveCount * 3; //* 3 because triangles


                    //Vertex[] vertices = new Vertex[length];
                    //mp.VertexBuffer.GetData<Vertex>(vertices);

                    //mp.VertexBuffer.SetData<Vertex>(vertices);

                    parts.Add(mp);
                }

                meshes.Add(new ModelMesh(DrawM.gDevice, parts));
                //meshes[meshes.Count - 1].Effects = m.Effects; //TODO: MG3.5 check if it is needed in MG3.6????
                meshes[meshes.Count - 1].ParentBone = m.ParentBone;
            }

            return new Model(DrawM.gDevice, new List<ModelBone>(), meshes);
        }
    }
}
