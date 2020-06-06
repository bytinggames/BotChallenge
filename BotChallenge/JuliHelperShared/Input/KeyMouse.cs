using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace JuliHelper
{
    public class KeyMouse : KeyP
    {
        public ButtonState state;

        public uint? lastPressed = null;
        public int comboPressed = 0;

        public bool pressedDouble
        {
            get
            {
                return pressed && (comboPressed % 2 == 0);
            }
        }
        public bool pressedTriple
        {
            get
            {
                return pressed && (comboPressed % 3 == 0);
            }
        }

        //public override bool catchedOrRecord
        //{
        //    get { return catched; }
        //}

        public override void Update()
        {
            if (Input.deCatchOnLoop)
                catched = false;

            if (!blocked)
            {
                //increment lastPressed frame counter
                if (lastPressed.HasValue)
                {
                    lastPressed++;

                    if (lastPressed >= Input.doublePressTimeMax)// || Input.mouseStillFrames < lastPressed.Value)
                        comboPressed = 0;
                }

                if (state == ButtonState.Pressed)
                {
                    if (!_down)
                    {
                        _pressed = true;
                        _down = true;
                        comboPressed++;
                        lastPressed = 0;
                    }
                    else
                        _pressed = false;
                }
                else
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
    }
}
