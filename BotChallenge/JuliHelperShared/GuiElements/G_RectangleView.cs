using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper.Gui
{
    public class G_RectangleViewGrid : GuiElement
    {
        G_TextBox txt;
        G_B_Sprite btn;

        public M_Rectangle rect;
        public bool positiveSize;

        M_Rectangle rectOld;


        Func<Matrix> viewTransform;
        Func<Vector2> customGrid;
        bool isPosSet;
        Vector2 p1;

        public G_RectangleViewGrid(M_Rectangle area, Func<Matrix> getViewTransform, Func<Vector2> getCustomGrid, Action<object> ValueChanged = null, M_Rectangle rect = null, float depth = 0f, bool positiveSize = true)
        {
            if (rect == null)
                rect = new M_Rectangle(0, 0, 0, 0);

            this.posL = area.pos;
            this.size = area.size;
            this.rect = rect;
            this.depth = depth;
            this.viewTransform = getViewTransform;
            this.customGrid = getCustomGrid;
            this.positiveSize = positiveSize;

            Initialize();

            this.ValueChanged = ValueChanged;
        }

        public void SetViewTransform(Func<Matrix> viewTransform)
        {
            this.viewTransform = viewTransform;
        }

        public void SetCustomGrid(Func<Vector2> customGrid)
        {
            this.customGrid = customGrid;
        }

        private void Initialize()
        {
            btn = new G_B_Sprite(new M_Rectangle(new Vector2(size.X - size.Y, 0), new Vector2(size.Y)),
                new Sprite(Vector2.Zero, ContentLoader.textures["btn_edit"]),
                BtnPressLeft, true, depth) { parent = this };
            btn.PressRight = g => PressRight();

            txt = new G_TextBox(new M_Rectangle(new Vector2(0, 0), new Vector2(size.X - size.Y, size.Y)), TextChanged, new KeyCollection().AddNumbers().Add(Input.dot, Input.comma), -1, depth) { parent = this };
            txt.PressRight = PressRight;

            txt.text = MyConverter.ToString(rect);

            isPosSet = false;

        }

        private void TextChanged(object text)
        {
            M_Rectangle newRect;
            if (MyConverter.TryParse<M_Rectangle>((string)text, out newRect))
            {
                rect = newRect;
                if (ValueChanged != null)
                    ValueChanged(rect);
            }

            txt.text = MyConverter.ToString(rect);
        }

        private void BtnPressLeft(G_Button obj)
        {
            if (btn.pressed)
            {
                //action

                SetCatchUpdate(Update);
                rectOld = (M_Rectangle)rect.Clone();
            }
        }

        private void PressRight()
        {
            //if (btn.pressed)
            {
                rect = new M_Rectangle(0, 0, 0, 0);
                txt.text = MyConverter.ToString(rect);
                btn.pressed = false;

                if (ValueChanged != null)
                    ValueChanged(rect);
            }
        }

        public override void SetValue(object value, Type type)
        {
            if (value == null)
                rect = new M_Rectangle(0,0,0,0);
            else
                rect = (M_Rectangle)value;

            txt.text = MyConverter.ToString(rect);
        }

        public override void Update()
        {
            UpdateBegin();

            if (visible)
            {
                btn.Update();

                if (btn.pressed)
                {
                    if (Input.mbLeft.pressed)
                    {
                        Vector2 g = customGrid();
                        p1 = (Vector2.Transform(Input.mbPos, Matrix.Invert(viewTransform())) / g).RoundVector() * g;
                        rect.pos = p1;
                        rect.size = Vector2.Zero;
                        isPosSet = true;
                    }
                    else if (isPosSet)
                    {
                        if (Input.mbLeft.down)
                        {
                            Vector2 g = customGrid();
                            Vector2 p2 = (Vector2.Transform(Input.mbPos, Matrix.Invert(viewTransform())) / g).RoundVector() * g;

                            if (positiveSize)
                            {
                                rect.size = (p2 - p1).AbsVector();
                                rect.pos = Calculate.MinVector(p1, p2);
                            }
                            else
                                rect.size = p2 - p1;

                            txt.text = MyConverter.ToString(rect);
                        }
                        else if (Input.mbLeft.released)
                        {
                            btn.pressed = false;
                            isPosSet = false;
                            SetCatchUpdate(null);

                            if (ValueChanged != null)
                                ValueChanged(rect);
                        }
                    }
                }
                else
                {
                    txt.Update();
                    isPosSet = false;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                txt.Draw(spriteBatch);
                btn.Draw(spriteBatch);

                if (btn.pressed)
                {
                    DrawRect(spriteBatch, rectOld, Color.Red * 0.5f);

                    if (!isPosSet)
                    {
                        //DrawM.Sprite.DrawLine(spriteBatch, p, 4, Color.Red, depth + 0.001f);
                        Vector2 g = customGrid();
                        Vector2 p = (Vector2.Transform(Input.mbPos, Matrix.Invert(viewTransform())) / g).RoundVector() * g;
                        DrawCross(spriteBatch, p, Color.Red * 0.5f);
                    }
                    else
                        DrawRect(spriteBatch, rect, Color.Red);
                }

                DrawEnd(spriteBatch);
            }
        }

        private void DrawRect(SpriteBatch spriteBatch, M_Rectangle rect, Color color)
        {
            Vector2 p1 = Vector2.Transform(rect.pos, viewTransform());
            Vector2 p2 = Vector2.Transform(rect.BottomRight, viewTransform());
            M_Rectangle newRect = new M_Rectangle(p1, p2 - p1);
            DrawM.Sprite.DrawRectCross(spriteBatch, newRect, color, 2, 16, 2, 1);
        }

        private void DrawCross(SpriteBatch spriteBatch, Vector2 pos, Color color)
        {
            DrawM.Sprite.DrawCross(spriteBatch, Vector2.Transform(pos, viewTransform()), color, 16, 2, 1);
        }

        public override GuiElement Clone(GuiElement parent)
        {
            return CloneChild(parent);
        }
        public G_RectangleViewGrid CloneChild(GuiElement parent)
        {
            G_RectangleViewGrid clone = (G_RectangleViewGrid)base.Clone(parent);
            //clone.Initialize();

            clone.rect = (M_Rectangle)rect.Clone();

            clone.btn = btn.CloneChild(clone);
            clone.btn.PressLeft = clone.BtnPressLeft;
            clone.btn.PressRight = g => clone.PressRight();

            clone.txt = txt.CloneChild(clone);
            clone.txt.ValueChanged = clone.TextChanged;
            clone.txt.PressRight = clone.PressRight;

            clone.ValueChanged = null;
            return clone;
        }

    }
}
