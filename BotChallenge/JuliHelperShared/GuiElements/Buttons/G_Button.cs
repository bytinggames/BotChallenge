using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.Gui
{
    public abstract class G_Button : GuiElement
    {
        public Color colorNormal, colorHover, colorPressed, colorHoverPressed;

        public bool pressed, canDepress;
            
        public bool stayPressed;

        public Action<G_Button> PressLeft, PressRight, PressMiddle;

        public MouseButton btnPressed;

        public static Color[] colors = new Color[]
        {
            Color.White, Color.LightGray, Color.Gray, Color.Gray
        };

        public static void InitColors()
        {
            colors = new Color[]
            {
                Color.White, Color.LightGray, Color.Gray, Color.Gray
            };
        }

        public G_Button()
        {
            canDepress = true;

            colorNormal = colors[0];
            colorHover = colors[1];
            colorPressed = colors[2];
            colorHoverPressed = colors[3];
        }

        public override void Update()
        {
            if (visible)
            {
                UpdateBegin();

                if (enabled)
                {
                    if (hover)
                    {
                        if (!pressed || (pressed && stayPressed && canDepress))
                        {
                            if (Input.mbLeft.pressed)
                            {
                                Input.mbLeft.pressed = false;
                                SetCatchUpdate(Update);
                                Press(MouseButton.Left);
                                //Input.mbLeft.catched = true;
                            }
                            else if (PressRight != null && Input.mbRight.pressed)
                            {
                                Input.mbRight.pressed = false;
                                SetCatchUpdate(Update);
                                Press(MouseButton.Right);
                                //Input.mbRight.catched = true;
                            }
                            else if (PressMiddle != null && Input.mbMiddle.pressed)
                            {
                                Input.mbMiddle.pressed = false;
                                SetCatchUpdate(Update);
                                Press(MouseButton.Middle);
                                //Input.mbMiddle.catched = true;
                            }
                        }
                    }

                    switch (btnPressed)
                    {
                        case MouseButton.Left:
                            if (!Input.mbLeft.down)
                                ReleaseMB();
                            break;
                        case MouseButton.Middle:
                            if (!Input.mbMiddle.down)
                                ReleaseMB();
                            break;
                        case MouseButton.Right:
                            if (!Input.mbRight.down)
                                ReleaseMB();
                            break;
                    }

                    //decatch
                    if (GetCatchUpdate() == Update && btnPressed == MouseButton.None)
                        SetCatchUpdate(null);
                }
                else
                {
                    if (GetCatchUpdate() == Update)
                        SetCatchUpdate(null);

                    if (!stayPressed)
                        pressed = false;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                Color color;

                if (hover)
                {
                    if (pressed)
                        color = colorHoverPressed;
                    else
                        color = colorHover;
                }
                else
                {
                    if (pressed)
                        color = colorPressed;
                    else
                        color = colorNormal;
                }

                DrawM.Sprite.DrawRectangle(spriteBatch, GetBounds(), color, depth);
            }
        }

        protected void DrawNotEnabled(SpriteBatch spriteBatch)
        {
            DrawM.Sprite.DrawRectangle(spriteBatch, GetBounds(), Color.Gray * 0.75f, depth + 0.02f);
        }

        public M_Rectangle GetBounds()
        {
            return new M_Rectangle(posG, size);
        }

        public void Deselect()
        {
            pressed = false;

            if (canDepress && PressLeft != null)
                PressLeft(this);
        }

        public void Select()
        {
            pressed = true;

            if (PressLeft != null)
                PressLeft(this);
        }

        public override GuiElement Clone(GuiElement parent)
        {
            return CloneChild(parent);
        }

        public G_Button CloneChild(GuiElement parent)
        {
            G_Button clone = (G_Button)base.Clone(parent);
            //clone.PressLeft = null;
            //clone.PressRight = null;
            //clone.PressMiddle = null;
            if (PressLeft != null)
                clone.PressLeft = (Action<G_Button>)PressLeft.Clone();
            if (PressRight != null)
                clone.PressRight = (Action<G_Button>)PressRight.Clone();
            if (PressMiddle != null)
                clone.PressMiddle = (Action<G_Button>)PressMiddle.Clone();
            return clone;
        }

        public void Press(MouseButton mb)
        {
            btnPressed = mb;
            pressed = !pressed;
            switch (mb)
            {
                case MouseButton.Left:
                    if (PressLeft != null)
                        PressLeft(this);
                    break;
                case MouseButton.Right:
                    if (PressRight != null)
                    PressRight(this);
                    break;
                case MouseButton.Middle:
                    if (PressMiddle != null)
                    PressMiddle(this);
                    break;
            }
        }

        private void ReleaseMB()
        {
            if (!stayPressed)
                pressed = false;
            btnPressed = MouseButton.None;
        }
    }
}