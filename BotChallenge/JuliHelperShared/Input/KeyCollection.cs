using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JuliHelper
{
    /// <summary>
    /// don't forget to Update()
    /// </summary>
    public class KeyCollection : KeyP
    {
        /// <summary>
        /// don't forget to Update()
        /// </summary>
        public List<KeyP> keys;
        int activeIndex = -1;

        public KeyCollection()
        {
            this.keys = new List<KeyP>();
        }
        public KeyCollection(List<KeyP> keys)
        {
            this.keys = keys;
            Update();
        }
        public KeyCollection(params KeyP[] keys)
        {
            this.keys = keys.ToList();
            Update();
        }
        
        public KeyCollection Add(params KeyP[] keys)
        {
            this.keys.AddRange(keys);
            Update();
            return this;
        }

        public KeyCollection AddNumbers()
        {
            for (int i = Input.d0.id; i <= Input.d9.id; i++)
                Add(Input.keys[i]);
            for (int i = Input.num0.id; i < Input.num9.id; i++)
                Add(Input.keys[i]);

            return this;
        }

        public KeyCollection AddHex()
        {
            AddNumbers();

            for (int i = Input.a.id; i <= Input.f.id; i++)
                Add(Input.keys[i]);

            return this;
        }

        public KeyCollection AddPlusMinus()
        {
            Add(
                Input.plus,
                Input.plusNum,
                Input.minus,
                Input.minusNum
                );
            return this;
        }

        public KeyCollection AddAlphabet26()
        {
            for (int i = Input.a.id; i <= Input.z.id; i++)
                Add(Input.keys[i]);
            return this;
        }

        public KeyCollection AddFileNameChars()
        {
            AddAlphabet26();
            AddNumbers();
            AddPlusMinus();
            Add(Input.dot, Input.comma, Input.sharp);
            return this;
        }

        public void Decatch()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                keys[i].catched = false;
            }
        }
        
        public bool Any(Func<KeyP, bool> func)
        {
            return keys.Any(func);
        }
        
        public override void Update()
        {
            if (activeIndex == -1)
            {
                activeIndex = keys.FindIndex(f => f.pressed || f.down || f.released);
            }

            if (activeIndex != -1)
            {
                if (keys[activeIndex].none)
                {
                    activeIndex = -1;
                    id = -1;
                    _pressed = _down = _released = catched = blocked = false;
                }
                else
                {
                    _pressed = keys[activeIndex]._pressed;
                    _down = keys[activeIndex]._down;
                    _released = keys[activeIndex]._released;
                    catched = keys[activeIndex].catched;
                    blocked = keys[activeIndex].blocked;
                    id = keys[activeIndex].id;
                }
            }
        }
    }
}
