using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JuliHelper
{
    public class KeyPad : KeyP
    {
        private Buttons key;

        private int playerIndex;

        public KeyPad(Buttons key, int playerIndex)
        {
            this.key = key;
            this.playerIndex = playerIndex;
        }

        public override void Update()
        {
            if (Input.deCatchOnLoop)
                catched = false;

            if (!blocked)
            {
                if (Input.gpStates[playerIndex].IsButtonDown(key))
                {
                    if (!_down)
                    {
                        _pressed = true;
                        _down = true;
                    }
                    else
                        _pressed = false;

                    _released = false;
                }

                if (Input.gpStates[playerIndex].IsButtonUp(key))
                {
                    if (_down)
                    {
                        _released = true;
                        _down = false;
                    }
                    else
                    {
                        _released = false;

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
    }
}
