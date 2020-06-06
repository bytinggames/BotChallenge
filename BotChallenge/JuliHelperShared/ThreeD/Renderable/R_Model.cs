using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.ThreeD
{
    public class R_Model : Renderable
    {
        public Model model;

        public R_Model(Model model, RenderMethod renderMethod = RenderMethod.Deferred)
        {
            this.model = model;

            ModelHelper.SetModelTechniques(model, "", renderMethod);
        }
        public R_Model(string assetName, RenderMethod renderMethod = RenderMethod.Deferred)
        {
            this.model = ContentLoader.models[assetName];

            ModelHelper.SetModelTechniques(model, "", renderMethod);
        }

        public override void Render(MeshBatch meshBatch, Vector3 pos)
        {
            Render(meshBatch, Matrix.CreateTranslation(pos));
        }

        public override void Render(MeshBatch meshBatch, Matrix transform)
        {
            meshBatch.Draw(model, transform);
            return;

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                for (int j = 0; j < model.Meshes[i].MeshParts.Count; j++)
                {
                    ModelMeshPart mp = model.Meshes[i].MeshParts[j];

                    if (mp.PrimitiveCount > 0)
                    {
                        Effect effect = model.Meshes[i].Effects[j];
                        effect.SetWorldAndInvTransp(model.Meshes[i].ParentBone.Transform * transform);

                        DrawM.gDevice.SetVertexBuffer(mp.VertexBuffer);
                        DrawM.gDevice.Indices = mp.IndexBuffer;


                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            //if the error appears here export with "Std" settings
                            DrawM.gDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                                   mp.VertexOffset,
                                                                   mp.StartIndex,
                                                                   mp.PrimitiveCount);

                        }
                    }
                }
            }
        }

        public override void ForEachEffect(Action<Effect> action)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    action(part.Effect);
                }
            }
        }

        public override void SetWorld(Matrix world)
        {
            foreach (var mesh in model.Meshes)
            {
                mesh.ParentBone.Transform = world;
            }
        }

        public override void Transform(Matrix matrix)
        {
            foreach (var mesh in model.Meshes)
            {
                mesh.ParentBone.Transform *= matrix;
            }
        }
    }
}
