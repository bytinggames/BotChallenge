using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class MeshBatch
    {
        public List<MeshBatchItem> meshes;

        public MeshSortMode sortMode = MeshSortMode.Immediate;

        GraphicsDevice gDevice;

        public MeshBatch(GraphicsDevice gDevice)
        {
            this.gDevice = gDevice;
        }

        public void Begin()
        {
            meshes = new List<MeshBatchItem>();
        }

        /// <summary>
        /// Renders all batched meshparts
        /// </summary>
        public void End()
        {
            if (sortMode != MeshSortMode.Immediate)
                for (int i = 0; i < meshes.Count; i++)
                    meshes[i].Render(gDevice);

            meshes.Clear();
        }
        public void Draw(MeshBatchItem mesh)
        {

            int index;
            switch (sortMode)
            {
                case MeshSortMode.FrontToBack:
                    mesh.CalculateDepthFromFirstVertex();
                    index = meshes.FindIndex(f => f.depth < mesh.depth);
                    if (index == -1)
                        index = 0;
                    meshes.Insert(index , mesh);
                    break;
                case MeshSortMode.BackToFront:
                    mesh.CalculateDepthFromFirstVertex();
                    index = meshes.FindIndex(f => f.depth > mesh.depth);
                    if (index == -1)
                        index = meshes.Count;
                    meshes.Insert(index, mesh);
                    break;
                case MeshSortMode.Immediate:
                    mesh.Render(gDevice);
                    break;
            }
        }

        public void Draw(Model model, Matrix world, Matrix[] skeleton, bool uniformScaling, Dictionary<string, int> uvBones)
        {
            for (int i = 0; i < model.Meshes.Count; i++)
                for (int j = 0; j < model.Meshes[i].MeshParts.Count; j++)
                {
                    MeshBatchItem.name = model.Meshes[i].Name;

                    Texture2D tex = model.Meshes[i].Effects[j].Parameters["Texture"].GetValueTexture2D();
                    int uvBoneIndex = -1;
                    if (tex != null)
                    {
                        string key = Path.GetFileName(tex.Name);
                        key = key.Substring(0, key.Length - 2);//cut away "_0"
                        if (uvBones.ContainsKey(key))
                            uvBoneIndex = uvBones[key];
                    }
                    Matrix cWorld;
                    //TODO: better "non" bone detection (see marian)
                    if (!model.Meshes[i].Effects[j].CurrentTechnique.Name.Contains("Bone")) //TODO: in animation matrix remove parentBone.Transform, so that it can be multiplied here always
                        cWorld = model.Meshes[i].ParentBone.Transform * world;
                    else
                        cWorld = world;

                    MeshBatchItem.parent = cWorld;// model.Meshes[i].ParentBone.Transform;

                    Draw(new MeshBatchBoneItem(model.Meshes[i].MeshParts[j], model.Meshes[i].Effects[j], cWorld, skeleton, uniformScaling, uvBoneIndex, 0));
                }
        }

        public void Draw(Model model, Matrix world)
        {
            for (int i = 0; i < model.Meshes.Count; i++)
                for (int j = 0; j < model.Meshes[i].MeshParts.Count; j++)
                    Draw(new MeshBatchItem(model.Meshes[i].MeshParts[j], model.Meshes[i].Effects[j], model.Meshes[i].ParentBone.Transform * world, 0));
        }

        public void Draw2(Model model, Matrix world)
        {

        }

    }

    public enum MeshSortMode
    {
        FrontToBack,
        BackToFront,
        Immediate
    }
}
