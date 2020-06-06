using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.Gui
{
    public class G_Enum : GuiElement
    {
        Type enumType;
        Enum enumValue;

        G_B_Text btn_Text;
        G_B_Sprite btn_Arrow;
        List<G_Button> btn_Enums;

        public G_Enum(M_Rectangle area, Action<object> ValueChanged = null, float depth = 0f)
        {
            this.posL = area.pos;
            this.size = area.size;
            this.depth = depth;

            Initialize();

            this.ValueChanged += ValueChanged;
        }

        private void Initialize()
        {
            btn_Arrow = new G_B_Sprite(new M_Rectangle(new Vector2(size.X - size.Y, 0), new Vector2(size.Y)),
                new Sprite(Vector2.Zero, ContentLoader.textures["btn_arrowdown"])
                , BtnArrowPressLeft, true, depth) { parent = this };
            //btn_Arrow.PressRight += BtnArrowPressRight;

            btn_Text = new G_B_Text(new M_Rectangle(Vector2.Zero, new Vector2(size.X - size.Y, size.Y)), "", BtnTextPressLeft, false, null, null, depth);
            btn_Text.PressRight += BtnTextPressRight;

            btn_Enums = new List<G_Button>();

            ValueChanged = MyValueChanged;

            //SetEnumType(typeof(MouseCatched));
            //enumValue = (Enum)Enum.GetValues(enumType).GetValue(1);
            //ValueChanged(enumValue);
        }

        public void SetEnumType(Type enumType)
        {
            this.enumType = enumType;

            btn_Enums = new List<G_Button>();

            Array enums = Enum.GetValues(enumType);

            for (int i = 0; i < enums.Length; i++)
                btn_Enums.Add(new G_B_Text(new M_Rectangle(0, 16 + i * 16, size.X, size.Y), enums.GetValue(i).ToString(), BtnListPress, true, null, null, depth + 0.1f) { parent = this });
        }

        public override void Update()
        {
            if (visible)
            {
                UpdateBegin();

                btn_Text.Update();
                btn_Arrow.Update();

                if (btn_Arrow.pressed)
                    for (int i = 0; i < btn_Enums.Count; i++)
                        btn_Enums[i].Update();

                if (Input.mbLeft.pressed && btn_Arrow.pressed)
                    CloseList();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                btn_Text.Draw(spriteBatch);
                btn_Arrow.Draw(spriteBatch);

                if (btn_Arrow.pressed)
                    for (int i = 0; i < btn_Enums.Count; i++)
                        btn_Enums[i].Draw(spriteBatch);

                DrawEnd(spriteBatch);
            }
        }

        private void BtnArrowPressLeft(G_Button btn)
        {
            if (btn.pressed)
                SetCatchUpdate(Update);
            else
                CloseList();

            //Game1.mouseCatched = MouseCatched.ObjEditor;
            //Game1.mbCatched = MBCatched.Static;
        }
        private void BtnTextPressLeft(G_Button btn)
        {
            if (btn_Arrow.pressed)
                CloseList();
            else
            {
                int index;
                if (enumValue == null)
                    index = 0;
                else
                {
                    index = (int)Convert.ChangeType(enumValue, typeof(int));
                    index++;
                    if (index >= Enum.GetValues(enumType).Length)
                        index = 0;
                }
                enumValue = (Enum)Enum.GetValues(enumType).GetValue(index);

                if (ValueChanged != null)
                    ValueChanged(enumValue);
            }
        }
        private void BtnTextPressRight(G_Button btn)
        {
            if (btn_Arrow.pressed)
                CloseList();
            else
            {
                int index = (int)Convert.ChangeType(enumValue, typeof(int));
                index--;
                if (index < 0)
                    index = Enum.GetValues(enumType).Length - 1;
                enumValue = (Enum)Enum.GetValues(enumType).GetValue(index);

                if (ValueChanged != null)
                    ValueChanged(enumValue);
            }
        }
        private void BtnListPress(G_Button btn)
        {
            int index = btn_Enums.IndexOf(btn);
            enumValue = (Enum)Enum.GetValues(enumType).GetValue(index);

            if (ValueChanged != null)
                ValueChanged(enumValue);

            CloseList();
        }

        private void MyValueChanged(object value)
        {
            if (value != null)
            {
                btn_Text.text = value.ToString();

                for (int i = 0; i < btn_Enums.Count; i++)
                    btn_Enums[i].pressed = false;


                //if ((int)value < Enum.GetValues(enumType).Length)
                    btn_Enums[Array.IndexOf(Enum.GetValues(enumType), value)].pressed = true;
                //else
                //    btn_Enums[0].pressed = true;
            }
            else
                btn_Text.text = "";
        }

        private void CloseList()
        {
            btn_Arrow.pressed = false;
            SetCatchUpdate(null);
        }

        public override GuiElement Clone(GuiElement parent)
        {
            return CloneChild(parent);
        }

        public G_Enum CloneChild(GuiElement parent)
        {
            G_Enum clone = (G_Enum)base.Clone(parent);
            //clone.Initialize();

            clone.btn_Text = btn_Text.CloneChild(clone);
            clone.btn_Text.PressLeft = clone.BtnTextPressLeft;
            clone.btn_Text.PressRight = clone.BtnTextPressRight;

            clone.btn_Arrow = btn_Arrow.CloneChild(clone);
            clone.btn_Arrow.PressLeft = clone.BtnArrowPressLeft;
            //clone.btn_Arrow.PressRight = clone.BtnArrowPressRight;

            if (enumType != null)
                clone.SetEnumType(enumType);
            //clone.btn_Enums = new List<G_Button>();
            //for (int i = 0; i < btn_Enums.Count; i++)
            //{
            //    clone.btn_Enums.Add(btn_Enums[i].CloneChild(clone));
            //    clone.btn_Enums[i].PressLeft 
            //}

            clone.ValueChanged = clone.MyValueChanged;

            if (clone.ValueChanged != null)
                clone.ValueChanged(clone.enumValue);

            //Initialize();
            return clone;
        }

        public override void SetValue(object value, Type type)
        {
            Type newEnumType = type;
            if (newEnumType != enumType)
                SetEnumType(newEnumType);

            if (value == null)
            {
                btn_Text.text = "";
                enumValue = null;
            }
            else
            {
                if (Enum.IsDefined(type, value))
                    enumValue = (Enum)value;
                else
                    enumValue = (Enum)Enum.GetValues(type).GetValue(0);
                MyValueChanged(enumValue);
            }

        }

        public override void Hotkey(bool invert)
        {
            if (invert)
                btn_Text.Press(MouseButton.Right);
            else
                btn_Text.Press(MouseButton.Left);
        }
    }
}
