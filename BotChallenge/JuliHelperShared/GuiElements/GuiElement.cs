using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.Gui
{
    public abstract class GuiElement
    {
        public static SpriteFont fontStd = ContentLoader.fonts["lato-thin-mod_10"];

        private static int _resX, _resY;

        public static int resX
        {
            get { return _resX; }
        }
        public static int resY
        {
            get { return _resY; }
        }
        public static Action<Action> SetCatchUpdate;
        public static Func<Action> GetCatchUpdate;

        private static Vector2 _mbPos, _mbPosPast;

        public static Vector2 mbPos
        {
            get { return _mbPos; }
        }
        public static Vector2 mbPosPast
        {
            get { return _mbPosPast; }
        }


        public static void Initialize(ref Action<int, int> ResChanged, Action<Action> _SetCatchUpdate, Func<Action> _GetCatchUpdate, int resX, int resY)
        {
            ResChanged += SetRes;

            SetCatchUpdate = _SetCatchUpdate;
            GetCatchUpdate = _GetCatchUpdate;

            _resX = resX;
            _resY = resY;
        }

        private static void SetRes(int x, int y)
        {
            _resX = x;
            _resY = y;
        }
        
        public static void Dispose(ref Action<int, int> resChanged)
        {
            resChanged -= SetRes;
        }

        public static void Update(Vector2 MbPos, Vector2 MbPosPast)
        {
            _mbPos = MbPos;
            _mbPosPast = MbPosPast;
        }

        protected Vector2 _size;

        public Vector2 posL;

        public virtual Vector2 size
        {
            get { return _size; }
            set
            {
                _size = value;
            }
        }

        public float sizeX
        {
            get { return size.X; }
            set { size = new Vector2(value, _size.Y); }
        }
        public float sizeY
        {
            get { return size.Y; }
            set { size = new Vector2(_size.X, value); }
        }

        public float depth;

        public bool enabled = true;
        public bool visible = true;

        public GuiElement parent;
        public Vector2 posG
        {
            get
            {
                if (parent == null)
                    return posL;
                else
                    return posL + parent.posG;
            }
            set
            {
                if (parent != null)
                    posL = value - parent.posG;
                else
                    posL = value;
            }
        }

        protected bool hover;
        public bool Hover { get { return hover; } }

        public string tooltip = "";
        private int hoverCounter = 0;
        protected G_ToolTip toolTip;


        public Action<object> ValueChanged;
        public static int hoverTooltipTriggerTime = 20;

        public virtual void SetValue(object value, Type type) { }
        public virtual void Hotkey(bool invert) { }

        public abstract void Update();
        public abstract void Draw(SpriteBatch spriteBatch);

        protected void UpdateBegin()
        {
            if (visible)
            {
                hover = ColVector(mbPos);

                if (toolTip != null)
                    toolTip.Update();

                if (hover)
                {
                    if (tooltip != "")
                    {
                        if (mbPos == mbPosPast)
                        {
                            hoverCounter++;
                            if (hoverCounter == GuiElement.hoverTooltipTriggerTime)// || GUI.toolTipCounter > 0) //TODO: add
                                CreateToolTip();
                        }
                        //else if (toolTip != null)
                        //    RemoveToolTip();
                    }
                }
                else
                    RemoveToolTip();
            }
            else
                RemoveToolTip();
        }
        protected void DrawEnd(SpriteBatch spriteBatch)
        {
            if (toolTip != null)
                toolTip.Draw(spriteBatch);
        }

        public bool ColVector(Vector2 vec)
        {
            return (vec.X >= posG.X && vec.X < posG.X + size.X
                 && vec.Y >= posG.Y && vec.Y < posG.Y + size.Y);

        }

        public M_Rectangle GetRectangle()
        {
            return new M_Rectangle(posG, size);
        }

        public virtual GuiElement Clone(GuiElement parent)
        {
            GuiElement clone = (GuiElement)this.MemberwiseClone();
            clone.parent = parent;
            return clone;
        }

        private void CreateToolTip()
        {
            if (toolTip == null)
                toolTip = new G_ToolTip(mbPos, tooltip, ContentLoader.fonts["lato-thin-mod_10"], depth + 0.1f);
            //GUI.toolTipCounter = 30; //TODO: add
        }

        private void RemoveToolTip()
        {
            toolTip = null;
            hoverCounter = 0;
        }
    }
}
