using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;


namespace JuliHelper
{

    public class BytingIntro
    {
        private List<Tooth> teeth;
        private List<List<Tooth>> teethPast;

        private int frame;
        private int cPhase;
        private int cFrame;

        private float teethOpacity;

        //static int width, height;
        float zoom;
        //static Color background;


        private int[] phaseLength = new int[] { 30, 30, 30, 30, 3, 10, 30 };//30,60,30,45,4,10
                                                                        //wait, "Byting Games" fading, Teeth fading, Teeth opening, Teeth closing, wait then trigger Fading, fade out

        float blackBlend = 0f;

        Action onEnd;

        public BytingIntro(float _zoom, Action _onEnd)
        {
            zoom = _zoom;
            onEnd = _onEnd;

            frame = 0;
            cFrame = 0;
            teethOpacity = 0f;
            cPhase = 0;
            teeth = new List<Tooth>();

            int tex = 0;

            for (int i = 0; i < 23; i++)
            {
                teeth.Add(new Tooth(ContentLoader.textures["intro/tooth_" + tex], i > 11));
                tex++;

                if (tex == 12)
                    tex = 0;

                if (i == 18)
                    tex++;
            }

            teeth[0].pos = new Vector2(0, 24);
            teeth[1].pos = new Vector2(6, 15);
            teeth[2].pos = new Vector2(14, 7);
            teeth[3].pos = new Vector2(25, 3);
            teeth[4].pos = new Vector2(37, 1);
            teeth[5].pos = new Vector2(50, 0);
            teeth[6].pos = new Vector2(63, 0);
            teeth[7].pos = new Vector2(76, 1);
            teeth[8].pos = new Vector2(88, 3);
            teeth[9].pos = new Vector2(100, 7);
            teeth[10].pos = new Vector2(109, 15);
            teeth[11].pos = new Vector2(117, 24);

            teeth[12].pos = new Vector2(4, 59);
            teeth[13].pos = new Vector2(11, 68);
            teeth[14].pos = new Vector2(20, 74);
            teeth[15].pos = new Vector2(31, 78);
            teeth[16].pos = new Vector2(43, 80);
            teeth[17].pos = new Vector2(56, 81);
            teeth[18].pos = new Vector2(69, 80);
            teeth[19].pos = new Vector2(81, 78);
            teeth[20].pos = new Vector2(93, 74);
            teeth[21].pos = new Vector2(104, 68);
            teeth[22].pos = new Vector2(112, 59);

            //for (int i = 0; i < 6; i++)
            //{
            //    teeth[i].depth = teeth[i + 12].depth = (5 - i) / 6f;
            //}
            //for (int i = 0; i < 5; i++)
            //{
            //    teeth[i + 6].depth = teeth[i + 18].depth = (i) / 6f;
            //}

            //teeth[11].depth = 0f;

            for (int i = 0; i < 12; i++)
            {
                teeth[i].pos.Y = (float)Math.Pow(i - 5.5f, 2) * 1.5f - 60 - 8;//14;
            }
            for (int i = 0; i < 11; i++)
            {
                teeth[i + 12].pos.Y = -(float)Math.Pow(i - 5, 2) * 1.5f + 52.5f + 8;// 37.5;
            }

            for (int i = 0; i < teeth.Count; i++)
            {
                //teeth[i].pos.Y += 16;

                teeth[i].distance = -teeth[i].pos.Y;//(51f + 8 - teeth[i].pos.Y);
                teeth[i].pos.Y += teeth[i].distance * 0.95f;
            }

            teeth = teeth.OrderBy(f => f.distance).ToList();


            teethPast = new List<List<Tooth>>();
        }

        public void Update()
        {
            if (cPhase < 6 &&
                (Input.mbLeft.pressed || Input.enter.pressed || Input.space.pressed || Input.esc.pressed))
            {
                cPhase = 6;
                cFrame = 0;
            }

            //Update Past Teeths
            //if (cPhase >= 4)
            {
                teethPast.Add(new List<Tooth>());
                for (int i = 0; i < teeth.Count; i++)
                {
                    teethPast[teethPast.Count - 1].Add(teeth[i].Clone());
                }
            }

            if (teethPast.Count > 3)
                teethPast.RemoveAt(0);

            //Real Teeth Update
            for (int i = 0; i < teeth.Count; i++)
                teeth[i].Update();

            //Phases
            if (cPhase >= 2 && cPhase <= 4)
            {
                teethOpacity += 1f / 3f / (float)phaseLength[cPhase];
            }

            if (cPhase == 3)
            {
                for (int i = 0; i < teeth.Count; i++)
                {
                    teeth[i].pos.Y -= teeth[i].distance * 0.95f / (float)phaseLength[cPhase] * (1f - (float)cFrame / (float)phaseLength[cPhase]) * 2f;
                }
            }
            else if (cPhase == 4)
            {
                for (int i = 0; i < teeth.Count; i++)
                {
                    teeth[i].pos.Y += teeth[i].distance / (float)phaseLength[cPhase];

                    teeth[i].scaleY = Math.Abs(teeth[i].pos.Y) / teeth[i].tex.Height / 2 + 1;//speedScale;
                }
            }
            //else if (cPhase == 5 && cFrame == phaseLength[cPhase] - 1)
            //{
            //    // wait
            //}
            else if (cPhase == 6)
            {
                blackBlend += 1f / phaseLength[cPhase];
                
                if (cFrame >= phaseLength[cPhase] - 1)
                {
                    onEnd();
                }
            }

            frame++;

            cFrame++;
            if (cPhase < phaseLength.Length && cFrame >= phaseLength[cPhase])
            {
                cFrame = 0;
                cPhase++;
            }
        }

        public void SetToLogo()
        {
            while (cPhase != 3 || frame != 100 || cFrame != 10)
            {
                Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Game1.graphics.GraphicsDevice.Clear(Color.Black);


            if (cPhase < 4)
                DrawTeeth(spriteBatch);


            //Byting Games
            Vector2 size = new Vector2(ContentLoader.textures["intro/logo_name"].Width, ContentLoader.textures["intro/logo_name"].Height);
            //if (size.X > Game1.width / Camera.zoom)
            size *= zoom; //4

            Vector2 pos = -size / 2;
            spriteBatch.Draw(ContentLoader.textures["intro/logo_name"], new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), Color.White * ((frame - phaseLength[0]) / (float)phaseLength[1]));

            if (cPhase >= 4)
                DrawTeeth(spriteBatch);

            if (blackBlend > 0f)
            {
                //spriteBatch.GraphicsDevice.Clear(Color.Black * blackBlend);
                spriteBatch.Draw(DrawM.pixel, new Rectangle(-10000, -10000, 20000, 20000), Color.Black * blackBlend);
            }
        }

        private void DrawTeeth(SpriteBatch spriteBatch)
        {
            float scale = 0.5f * zoom + teethOpacity * 0.5f * zoom;

            Vector2 shift = new Vector2(-125 * scale, -24 * scale) / 2;
            Color color = Color.White * teethOpacity;
            color *= 1f / (teethPast.Count);

            for (int i = 0; i < teethPast.Count; i++)
            {
                for (int j = 0; j < teethPast[i].Count; j++)
                {
                    teethPast[i][j].Draw(spriteBatch, shift, color, scale);
                }
            }
            if (cPhase >= 5)
                color = Color.White * teethOpacity;
            for (int i = 0; i < teeth.Count; i++)
                teeth[i].Draw(spriteBatch, shift, color, scale);


        }

        ~BytingIntro()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (teeth != null)
            {
                for (int i = 0; i < teeth.Count; i++)
                {
                    teeth[i].tex.Dispose();
                }
                teeth = null;

                teethPast = null;
            }
        }

        class Tooth
        {
            public Vector2 pos;
            public Texture2D tex;
            public float distance;
            public bool upsideDown;
            public float scaleY;
            public float depth = 0;

            public Tooth(Texture2D tex, bool upsideDown)
            {
                this.tex = tex;
                this.upsideDown = upsideDown;
                this.pos = Vector2.Zero;
                this.scaleY = 1;
            }

            public void Update()
            {
                //pos.Y++;
            }

            public void Draw(SpriteBatch spriteBatch, Vector2 shift, Color color, float scaleX)
            {
                float scaleY = scaleX * this.scaleY;
                if (!upsideDown)
                    spriteBatch.Draw(tex, new Rectangle((int)(pos.X * scaleX + shift.X), (int)(pos.Y * scaleX + shift.Y), (int)(tex.Width * scaleX), (int)(tex.Height * scaleY)), null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
                else
                    spriteBatch.Draw(tex, new Rectangle((int)(pos.X * scaleX + shift.X), (int)((pos.Y + tex.Height) * scaleX + shift.Y), (int)(tex.Width * scaleX), (int)(-tex.Height * scaleY)), null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
            }

            public override string ToString()
            {
                return (pos.X + " " + pos.Y);
            }

            public Tooth Clone()
            {
                return (Tooth)this.MemberwiseClone();
            }
        }
    }
}
