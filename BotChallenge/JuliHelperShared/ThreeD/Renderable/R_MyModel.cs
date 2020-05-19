using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper.ThreeD
{
    public class R_MyModel : Renderable
    {
        public List<R_MyMesh> meshes;

        public R_MyModel(List<R_MyMesh> meshes)
        {
            this.meshes = meshes;
        }

        public R_MyModel(Model model, Action<Vertex[]> action = null)
        {
            meshes = new List<R_MyMesh>();
            foreach (var mesh in model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    //TODO: add textures
                    meshes.Add(new R_MyMesh(part, mesh.ParentBone.Transform, action));
                }
            }
        }

        public override void Render(MeshBatch meshBatch, Vector3 pos)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Render(meshBatch, pos);
            }
        }

        public override void ForEachEffect(Action<Effect> action)
        {
            for (int i = 0; i < meshes.Count; i++)
                meshes[i].ForEachEffect(action);
        }

        public void RecalculateNormalsSoft(float softness)
        {
            foreach (R_MyMesh mesh in meshes)
            {
                mesh.RecalculateNormalsSoft(softness);
            }
        }


        public override void SetWorld(Matrix world)
        {
            foreach (R_MyMesh mesh in meshes)
            {
                mesh.SetWorld(world);
            }
        }

        public override void Transform(Matrix matrix)
        {
            foreach (var mesh in meshes)
            {
                mesh.Transform(matrix);
            }
        }

        public override void Render(MeshBatch meshBatch, Matrix transform)
        {
            for (int i = 0; i < meshes.Count; i++)
                meshes[i].Render(meshBatch, transform);
        }
    }
}
