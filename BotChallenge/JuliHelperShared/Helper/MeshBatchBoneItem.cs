using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class MeshBatchBoneItem : MeshBatchItem
    {
        Matrix[] skeleton;
        bool uniformScaling;
        int uvBoneIndex;

        public MeshBatchBoneItem(ModelMeshPart meshPart, Effect effect, Matrix world, Matrix[] skeleton, bool uniformScalingAnimation, int uvBoneIndex, float depth)
            :base(meshPart, effect, world, depth)
        {
            this.skeleton = skeleton;
            this.uniformScaling = uniformScalingAnimation;
            this.uvBoneIndex = uvBoneIndex;


            //if (effect.CurrentTechnique.Name == "DiffuseTexture") //TODO: check
            //    this.world = Matrix.CreateScale(10, -10, -10) * world;
        }

        public override void Render(GraphicsDevice gDevice)
        {
            if (meshPart.PrimitiveCount > 0)
            {
                //bone animation extra
                effect.Parameters["Bones"].SetValue(skeleton);
                effect.Parameters["uniformScalingAnimation"].SetValue(uniformScaling);
                effect.Parameters["uvBoneIndex"].SetValue(uvBoneIndex);

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
    }
}
