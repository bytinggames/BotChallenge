using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using JuliHelper;

namespace JuliHelper.Gui
{
    public class G_ObjSelect : GuiElement
    {
        G_B_Text btn;

        public G_ObjSelect(Vector2 pos, float depth = 0f)
        {
            this.posL = pos;
            this.depth = 0f;
            this.size = new Vector2(16, 16);
            this.depth = depth;

            btn = new G_B_Text(new M_Rectangle(0, 0, 16, 16), "P", PressLeft, true, ContentLoader.fonts["lato-thin-mod_10"], null, depth);
            btn.parent = this;
        }

        public override void Update()
        {
            if (visible)
            {
                btn.Update();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                btn.Draw(spriteBatch);
            }
        }

        public void PressLeft(G_Button btn)
        {
            //if (btn.pressed)
            //    Brush.SelectObjs(true);
            //else
            //{
            //    this.btn.text = Brush.selectedObjs.Count.ToString();

            //    if (ValueChanged != null)
            //        ValueChanged(Brush.selectedObjs);
            //    Brush.SelectObjs(false);
            //}
        }

        public override GuiElement Clone(GuiElement parent)
        {
            return CloneChild(parent);
        }

        public G_ObjSelect CloneChild(GuiElement parent)
        {
            G_ObjSelect clone = (G_ObjSelect)base.Clone(parent);
            clone.btn = btn.CloneChild(clone);
            clone.btn.PressLeft = clone.PressLeft;
            return clone;
        }
    }
}
