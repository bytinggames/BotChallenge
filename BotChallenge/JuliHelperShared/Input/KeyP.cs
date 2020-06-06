using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JuliHelper
{
    public abstract class KeyP
    {
        public bool _down, _pressed, _released;

        public bool down { get { return (catchedOrRecord ? false : _down); } set { _down = value; } }
        public bool pressed { get { return (catchedOrRecord ? false : _pressed); } set { _pressed = value; } }
        public bool released { get { return (catchedOrRecord ? false : _released); } set { _released = value; } }

        public bool none { get { return (catchedOrRecord ? true : !down && !released); } }

        public bool catched, blocked;

        public virtual bool catchedOrRecord
        {
            get { return catched || KeyString.Record; }
        }

        public short id;

        public KeyP()
        {
            _down = _pressed = _released = false;
        }

        public abstract void Update();
    }
}
