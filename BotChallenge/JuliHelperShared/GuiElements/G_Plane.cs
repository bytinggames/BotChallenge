using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.Gui
{
    public class G_Plane : GuiElement
    {
        public List<GuiElement> guis;

        public static Color color = Color.Black * 0.5f;

        public Color areaColor;

        public G_Plane(M_Rectangle area, float depth = 0f)
        {
            this.posL = area.pos;
            this.size = area.size;
            this.depth = depth;

            guis = new List<GuiElement>();

            areaColor = color;
        }

        public void Add(GuiElement gui)
        {
            gui.depth += depth + 0.01f;
            guis.Add(gui);
            gui.parent = this;
        }

        public override void Update()
        {
            if (visible)
            {
                UpdateBegin();

                for (int i = 0; i < guis.Count; i++)
                    guis[i].Update();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                DrawM.Sprite.DrawRectangle(spriteBatch, GetRectangle(), areaColor, depth);

                for (int i = 0; i < guis.Count; i++)
                    guis[i].Draw(spriteBatch);

                DrawEnd(spriteBatch);
            }
        }

        public override GuiElement Clone(GuiElement parent)
        {
            return CloneChild(parent);
        }

        public G_Plane CloneChild(GuiElement parent)
        {
            G_Plane clone = (G_Plane)base.Clone(parent);
            clone.guis = new List<GuiElement>();
            for (int i = 0; i < guis.Count; i++)
                clone.guis.Add(guis[i].Clone(this));
            return clone;
        }
    }
}
