using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class InputRecorder
    {
        private bool _recording, _playing, mouseInput;
        public bool removeLastFrame;

        class Future
        {
            public int frame;
            public List<KeyP> pressed = new List<KeyP>(), released = new List<KeyP>();
            public Vector2? mbPos;
            public int mbWheel;

            public void Play()
            {
                #region Keys

                for (int i = 0; i < pressed.Count; i++)
                {
                    pressed[i].pressed = true;
                    pressed[i].down = true;
                    if (changeBlock)
                        pressed[i].blocked = true;
                }
                for (int i = 0; i < released.Count; i++)
                {
                    released[i].released = true;
                    released[i].down = false;
                }
                
                #endregion

                if (mbPos.HasValue)
                {
                    Input.mbPos = mbPos.Value;
                }
                Input.mbWheel = mbWheel;
            }

            public void PlayPast()
            {
                for (int i = 0; i < pressed.Count; i++)
                {
                    pressed[i].pressed = false;
                }
                for (int i = 0; i < released.Count; i++)
                {
                    released[i].released = false;
                    if (changeBlock)
                        released[i].blocked = false;
                }
            }
        }

        Future future, past;

        private int _frame;
        //private int frames;
        public static bool changeBlock = true;

        //public Vector2 mouseOffset = Vector2.Zero;

        Vector2 mbPosPast;

        public EventHandler FinishPlaying;

        public KeyP[] keysInput;

        Stream stream;
        BinaryWriter writer;
        BinaryReader reader;

        public bool recording
        {
            get { return _recording; }
        }


        public bool playing
        {
            get { return _playing; }
        }

        public int frame
        {
            get { return _frame; }
        }

        int lastFrameWrittenToStream = 0;

        string gameVersion;
        const int inrVersion = 1;
        Action<BinaryWriter> writeSave;
        Action<BinaryReader> readSave;

        public InputRecorder(bool removeLastFrame, bool mouseInput, string gameVersion, Action<BinaryWriter> writeSave, Action<BinaryReader> readSave)
        {
            this.removeLastFrame = removeLastFrame;
            this.mouseInput = mouseInput;
            this.gameVersion = gameVersion;
            this.writeSave = writeSave;
            this.readSave = readSave;

            Initialize();
        }
        
        private void Initialize()
        {
            _recording = _playing = false;
            Input.blockInput = false;
            /*frames = */_frame = 0;
            lastFrameWrittenToStream = 0;

            if (keysInput == null)
            {
                if (mouseInput)
                    keysInput = Input.keys;
                else
                {
                    List<KeyP> keys = Input.keys.ToList();
                    keys.Remove(Input.mbLeft);
                    keys.Remove(Input.mbRight);
                    keys.Remove(Input.mbMiddle);
                    keysInput = keys.ToArray();
                }
            }
        }

        enum CodeBody : byte
        {
            Frame = 1,
            MousePos = 2,
            MouseWheel = 3,
            KeyPressed = 4,
            KeyReleased = 5,
        }

        enum CodeHead : byte
        {
            EndHeader = 1,
            Seed = 2,
            SaveState = 3,
            GameVersion = 4,
            InrVersion = 5,
        }

        public void Update()
        {
            if (playing)
            {
                Input.mbPos = mbPosPast;
                Input.mbWheel = 0;

                if (past != null && past.frame == _frame - 1)
                {
                    past.PlayPast();
                    past = null;
                }

                if (future == null)
                {
                    if (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        future = GetFuture();

                        Future GetFuture()
                        {
                            Future future = new Future();
                            while (true)
                            {
                                CodeBody code = (CodeBody)reader.ReadByte();
                                switch (code)
                                {
                                    case CodeBody.Frame:
                                        future.frame = lastFrameWrittenToStream + reader.ReadInt32();
                                        lastFrameWrittenToStream = future.frame;
                                        return future;
                                    case CodeBody.MousePos:
                                        future.mbPos = reader.ReadVector2();
                                        break;
                                    case CodeBody.MouseWheel:
                                        future.mbWheel = reader.ReadInt32();
                                        break;
                                    case CodeBody.KeyPressed:
                                        future.pressed.Add(Input.keys[reader.ReadInt16()]);
                                        break;
                                    case CodeBody.KeyReleased:
                                        future.released.Add(Input.keys[reader.ReadInt16()]);
                                        break;
                                    default:
                                        throw new Exception();
                                }
                            }
                        }
                    }
                    else
                    {
                        // stop playing
                        //if (_frame >= frames)
                        //{
                        //}
                        Input.ClearKeys();
                        _playing = false;
                        Input.blockInput = false;

                        if (FinishPlaying != null)
                            FinishPlaying(this, EventArgs.Empty);
                        return;
                    }
                }

                if (future.frame == _frame)
                {
                    future.Play();
                    past = future;
                    future = null;
                }
                else if (future.frame < _frame)
                    throw new Exception();


                mbPosPast = Input.mbPos;
            }

            if (recording)
            {
                // update format:
                // 1(byte): frame distance to last input frame
                //      frame(int)
                // 2(byte): mouse
                //      x(Vector2)
                // 3(byte): mouseWheel
                //      wheel(int)
                // 4(byte): key pressed
                //      id(short)
                // 5(byte): key released
                //      id(short)

                long prevStreamPos = writer.BaseStream.Position;

                if (mouseInput)
                {
                    #region Mouse

                    if (Input.mbPos != mbPosPast)
                    {
                        writer.Write((byte)CodeBody.MousePos);
                        writer.Write(Input.mbPos);
                        mbPosPast = Input.mbPos;
                    }

                    if (Input.mbWheel != 0)
                    {
                        writer.Write((byte)CodeBody.MouseWheel);
                        writer.Write(Input.mbWheel);
                    }

                    #endregion
                }

                #region Keys

                for (int i = 0; i < keysInput.Length; i++)
                {
                    if (keysInput[i].pressed)
                    {
                        writer.Write((byte)CodeBody.KeyPressed);
                        writer.Write(keysInput[i].id);
                    }
                    else if (keysInput[i].released)
                    {
                        writer.Write((byte)CodeBody.KeyReleased);
                        writer.Write(keysInput[i].id);
                    }
                }

                #endregion

                if (prevStreamPos != writer.BaseStream.Position)
                {
                    writer.Write((byte)CodeBody.Frame);
                    writer.Write(_frame - lastFrameWrittenToStream);
                    lastFrameWrittenToStream = _frame;

                    writer.Flush();
                }
            }

            if (recording || playing)
                _frame++;
        }

        public void UpdateControl()
        {
            //if (Input.f5.released)
            //    StartStopRecording();
            //if (Input.f6.released)
            //    StartStopPlaying(true);
            //if (Input.f7.released)
            //    Open(G.exeDir + "\\record.txt");
            //if (Input.f8.released)
            //    Save(G.exeDir + "\\record.txt");
        }

        public void ResetRecording()
        {
            Initialize();
        }

        public void StartRecording(int seed, bool recordCurrentFrameInputDown = false)
        {
            Initialize();
            if (!recordCurrentFrameInputDown)
                Input.ClearKeys();

            _recording = true;
            mbPosPast = Vector2.Zero;

            writer.Write((byte)CodeHead.Seed);
            writer.Write(seed);

            writer.Write((byte)CodeHead.GameVersion);
            writer.Write(gameVersion);

            writer.Write((byte)CodeHead.InrVersion);
            writer.Write(inrVersion);

            if (writeSave != null)
            {
                writer.Write((byte)CodeHead.SaveState);
                writeSave(writer);
            }

            writer.Write((byte)CodeHead.EndHeader);

            if (recordCurrentFrameInputDown)
            {
                _frame = -1;

                throw new NotImplementedException();

                Update();
                
                _frame = 0;
            }
        }

        public void StopRecording()
        {
            if (_recording)
            {
                //frames = _frame;
                //if (removeLastFrame)
                //{
                //    if (keyRecord.ContainsKey(frames))
                //        keyRecord.Remove(frames);
                //    frames--;
                //}
                // TODO: remove last frame
                _recording = false;
            }
        }

        //public void StartStopRecording()
        //{
        //    if (recording)
        //        StopRecording();
        //    else
        //        StartRecording();
        //}

        public int? StartPlaying(bool clearKeys)
        {
            if (recording)
                StopRecording();

            if (clearKeys)
                Input.ClearKeys();
            else
                Input.DeblockKeys();

            _playing = true;
            //Input.blockInput = true;
            _frame = -1;
            lastFrameWrittenToStream = 0;


            mbPosPast = Vector2.Zero;

            int? seed = null;

            CodeHead code;
            while ((code = (CodeHead)reader.ReadByte()) != CodeHead.EndHeader)
            {
                switch (code)
                {
                    case CodeHead.Seed:
                        seed = reader.ReadInt32();
                        break;
                    case CodeHead.SaveState:
                        if (readSave != null)
                            readSave(reader);
                        break;
                    case CodeHead.GameVersion:
                        {
                            string version = reader.ReadString();
                            if (version != gameVersion)
                            {
#if WINDOWS1
                                System.Windows.Forms.MessageBox.Show("input game version: " + version + " current game version: " + gameVersion);
#endif
                                //throw new Exception("input game version: " + version + " current game version: " + gameVersion);
                            }
                        }
                        break;
                    case CodeHead.InrVersion:
                        {
                            int version = reader.ReadInt32();
                            if (version != inrVersion)
                                throw new Exception("input inr version: " + version + " current inr version: " + inrVersion);
                        }
                        break;
                    default:
                        throw new Exception("couldn't read input header");
                }
            }

            Update();

            _frame = 0;

            ////Init already down keys
            //if (keyRecord.ContainsKey(_frame))
            //{
            //    foreach (Command command in keyRecord[_frame])
            //    {
            //        if (command.press)
            //        {
            //            command.key.down = true;
            //            if (changeBlock)
            //                command.key.blocked = true;
            //        }
            //    }
            //}

            return seed;
        }

        public void StopPlaying(bool clearKeys)
        {
            _playing = false;
            Input.blockInput = false;

            if (clearKeys)
                Input.ClearKeys();
            else
                Input.DeblockKeys();
        }

        public void StartStopPlaying(bool clearKeys)
        {
            if (playing)
                StopPlaying(clearKeys);
            else
                StartPlaying(clearKeys);
        }
        
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, bool left, bool top)
        {
            int resX = DrawM.gDevice.PresentationParameters.BackBufferWidth;
            int resY = DrawM.gDevice.PresentationParameters.BackBufferHeight;

            string text = "";
            if (recording)
                text += "[recording]";
            if (playing)
                text += "[playing]";

            if (recording || playing)
                text += " f: " + frame;

            Vector2 size = Calculate.CeilVector(font.MeasureString(text) / 16f) * 16f;
            Vector2 pos = new Vector2((left) ? 8 : resX - size.X - 8, (top) ? 8 : resY - size.Y - 8);
            spriteBatch.DrawString(font, text, pos, Color.Red);
        }
        
        public void Record(string _filePath, int seed)
        {
            Dispose();
            string dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            stream = new FileStream(_filePath, FileMode.Create);
            writer = new BinaryWriter(stream);

            StartRecording(seed);
        }

        public int? Play(string _filePath)
        {
            Dispose();
            System.Threading.Thread.Sleep(1000);
            stream = File.OpenRead(_filePath);// new FileStream(_filePath, FileMode.Open);
            reader = new BinaryReader(stream);

            return StartPlaying(true);
        }

        public void Dispose()
        {
            writer?.Close();
            reader?.Close();
            stream?.Close();
            Dispose(stream, writer, reader);
            stream = null;
            writer = null;
            reader = null;
        }

        private void Dispose(params IDisposable[] disposes)
        {
            for (int i = 0; i < disposes.Length; i++)
            {
                if (disposes[i] != null)
                    disposes[i].Dispose();
            }
        }
    }
}
