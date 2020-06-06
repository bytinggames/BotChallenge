using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JuliHelper
{
    public class Key : KeyP
    {
        private Keys _key;
        public Keys key { get { return _key; } }
        public string keyStringLow;
        public string keyStringHigh;
        public string keyStringAltGr;
        protected int timeDown;
        public int TimeDown
        {
            get { return timeDown; }
        }

        public Action<KeyP> KeyStringDown, KeyStringControlDown;

        public Key(Keys key, string keyStringLow = "", string keyStringHigh = "", string keyStringAltGr = "") : base()
        {
            this._key = key;
            this.keyStringLow = keyStringLow;
            this.keyStringHigh = keyStringHigh;
            this.keyStringAltGr = keyStringAltGr;
        }

        public override void Update()
        {
            if (Input.deCatchOnLoop)
                catched = false;

            if (!blocked)
            {
                if (Input.ks.IsKeyDown(key))
                {
                    //if (releaseForNewInput)
                    //{

                    //}
                    //else 
                        if (!_down)
                    {
                        _pressed = true;
                        _down = true;
                        timeDown = 1;

                        AddKeyString();
                    }
                    else
                    {
                        _pressed = false;
                        timeDown++;

                        if (timeDown > 30)
                            AddKeyString();
                    }
                    _released = false;
                }
                else
                {
                    if (_down)
                    {
                        _released = true;
                        _down = false;
                        timeDown = 0;
                    }
                    else
                    {
                        _released = false;
                        timeDown--;

                        if (Input.deCatchOnRelease)
                            catched = false;
                    }

                    _pressed = false;
                }
            }
        }

        public override string ToString()
        {
            return key.ToString();
        }

        private void AddKeyString()
        {
            if (KeyString.Record)
            {
                KeyString.frame = 0;
                if (KeyString.enterOnEnter && key == Keys.Enter)
                {
                    Input.enter.catched = true;
                    if (KeyString.KeyStringEntered != null)
                        KeyString.KeyStringEntered(KeyString.keyString);
                }
                else if (Input.leftControl._down && KeyStringControlDown != null)
                        KeyStringControlDown(this);
                else if (KeyStringDown != null)
                    KeyStringDown(this);
                else if (keyStringLow != "" && !catched)
                {
                    if (Input.leftControl._down && Input.leftAlt._down)
                        KeyString.Write(keyStringAltGr);
                    else if (Input.leftShift._down)
                        KeyString.Write(keyStringHigh);
                    else
                        KeyString.Write(keyStringLow);
                }
            }
        }
    }
}
