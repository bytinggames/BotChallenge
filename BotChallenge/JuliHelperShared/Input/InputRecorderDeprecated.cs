using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class InputRecorderDeprecated
    {
        private bool _recording, _playing, mouseInput;
        public bool removeLastFrame;

        private Dictionary<int, List<Command>> keyRecord;   //0
        private Dictionary<int, Vector2> mbRecord;          //1
        private Dictionary<int, int> mbWheelRecord;         //2
        private int _frame;
        private int frames;
        public static bool changeBlock = true;
        public Vector2 mouseOffset = Vector2.Zero;

        Vector2 mbPos;

        public EventHandler FinishPlaying;

        public KeyP[] keysInput;

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

        public InputRecorderDeprecated(bool removeLastFrame, bool mouseInput)
        {
            this.removeLastFrame = removeLastFrame;
            this.mouseInput = mouseInput;
            Initialize();
        }

        public InputRecorderDeprecated(string fromPath, bool removeLastFrame = true, bool mouseInput = true)
        {
            this.mouseInput = true;
            this.removeLastFrame = true;
            Open(fromPath);
        }
        public InputRecorderDeprecated(byte[] fromData, bool removeLastFrame = true, bool mouseInput = true)
        {
            this.mouseInput = true;
            this.removeLastFrame = true;
            Open(fromData);
        }

        private void Initialize()
        {
            _recording = _playing = false;
            Input.blockInput = false;
            keyRecord = new Dictionary<int, List<Command>>();
            mbRecord = new Dictionary<int, Vector2>();
            mbWheelRecord = new Dictionary<int, int>();
            frames = _frame = 0;

            if (keysInput == null)
            {
                if (mouseInput)
                    keysInput = Input.keys;
                else
                    keysInput = Input.keys.Take(Input.keys.Length - 3).ToArray();
            }
        }

        public void Update()
        {
            if (playing)
            {
                #region Keys

                if (keyRecord.ContainsKey(_frame - 1))
                {
                    foreach (Command command in keyRecord[_frame - 1])
                    {
                        if (command.press)
                            command.key.pressed = false;
                        else
                        {
                            command.key.released = false;
                            if (changeBlock)
                                command.key.blocked = false;
                        }
                    }
                }

                if (keyRecord.ContainsKey(_frame))
                {
                    foreach (Command command in keyRecord[_frame])
                    {
                        if (command.press)
                        {
                            command.key.pressed = true;
                            command.key.down = true;
                            if (changeBlock)
                                command.key.blocked = true;
                        }
                        else
                        {
                            command.key.released = true;
                            command.key.down = false;
                        }
                    }
                }

                #endregion

                if (mouseInput)
                {
                    #region Mouse

                    //Reset
                    if (mbWheelRecord.ContainsKey(_frame - 1))
                        Input.mbWheel = 0;

                    if (mbRecord.ContainsKey(_frame))
                    {
                        Input.mbPosPast = mbPos;
                        Input.mbPos = mbPos = mbRecord[_frame] + mouseOffset;
                    }
                    else
                    {
                        Input.mbPosPast = mbPos;
                        Input.mbPos = mbPos;
                    }

                    if (mbWheelRecord.ContainsKey(_frame))
                        Input.mbWheel = mbWheelRecord[_frame];

                    #endregion
                }
                if (_frame >= frames)
                {
                    Input.ClearKeys();
                    _playing = false;
                    Input.blockInput = false;

                    if (FinishPlaying != null)
                        FinishPlaying(this, EventArgs.Empty);
                }
            }

            if (recording)
            {
                #region Keys

                List<Command> newCommands = new List<Command>();
                
                for (int i = 0; i < keysInput.Length; i++)
                {
                    if (keysInput[i].pressed)
                        newCommands.Add(new Command(keysInput[i], true));
                    if (keysInput[i].released)
                        newCommands.Add(new Command(keysInput[i], false));
                }

                #endregion

                if (mouseInput)
                {
                    #region Mouse

                    if (Input.mbPos != Input.mbPosPast)
                        mbRecord.Add(_frame, Input.mbPos - mouseOffset);

                    if (Input.mbWheel != 0)
                        mbWheelRecord.Add(_frame, Input.mbWheel);

                    #endregion
                }

                if (newCommands.Count > 0)
                    keyRecord.Add(_frame, newCommands);
            }

            if (recording || playing)
                _frame++;
        }

        public void UpdateControl()
        {
            if (Input.f5.released)
                StartStopRecording();
            if (Input.f6.released)
                StartStopPlaying(true);
            if (Input.f7.released)
                Open(Path.Combine(G.exeDir, "record.txt"));
            if (Input.f8.released)
                Save(Path.Combine(G.exeDir, "record.txt"));
        }

        public void ResetRecording()
        {
            Initialize();
        }

        public void StartRecording(bool recordCurrentFrameInputDown = false)
        {
            Initialize();
            if (!recordCurrentFrameInputDown)
                Input.ClearKeys();

            _recording = true;

            if (recordCurrentFrameInputDown)
            {
                _frame = -1;


                #region Keys

                List<Command> newCommands = new List<Command>();

                for (int i = 0; i < keysInput.Length; i++)
                {
                    if (keysInput[i].down && !keysInput[i].pressed)
                        newCommands.Add(new Command(keysInput[i], true));
                }

                #endregion

                if (newCommands.Count > 0)
                    keyRecord.Add(_frame, newCommands);

                _frame = 0;

            }
        }

        public void StopRecording()
        {
            if (_recording)
            {
                frames = _frame;
                if (removeLastFrame)
                {
                    if (keyRecord.ContainsKey(frames))
                        keyRecord.Remove(frames);
                    frames--;
                }
                _recording = false;
            }
        }

        public void StartStopRecording()
        {
            if (recording)
                StopRecording();
            else
                StartRecording();
        }

        public void StartPlaying(bool clearKeys)
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

            //Init already down keys
            if (keyRecord.ContainsKey(_frame))
            {
                foreach (Command command in keyRecord[_frame])
                {
                    if (command.press)
                    {
                        command.key.down = true;
                        if (changeBlock)
                            command.key.blocked = true;
                    }
                }
            }

            mbPos = Input.mbPos;

            _frame = 0;
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

        public void Save(string filePath)
        {
            System.IO.File.WriteAllBytes(filePath, ToData());
        }

        public void Open(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                Open(System.IO.File.ReadAllBytes(filePath));
            }
        }

        public void Open(byte[] data)
        {
            Initialize();

            int i = 0;

            frames = BitConverter.ToInt32(data, i); i += 4;

            while (i < data.Length)
            {
                byte id = data[i]; i++;
                switch (id)
                {
                    case 0:
                        int keyRecordCount = BitConverter.ToInt32(data, i); i += 4;
                        for (int j = 0; j < keyRecordCount; j++)
                        {
                            int frame = BitConverter.ToInt32(data, i); i += 4;
                            byte commandsCount = data[i]; i++;

                            List<Command> commands = new List<Command>();

                            for (int k = 0; k < commandsCount; k++)
                            {
                                short key = BitConverter.ToInt16(data, i); i += 2;
                                bool press = data[i] == 1; i++;
                                commands.Add(new Command(Input.keys[key], press));
                            }

                            keyRecord.Add(frame, commands);
                        }
                        break;

                    case 1:
                        int mbRecordCount = BitConverter.ToInt32(data, i); i += 4;
                        for (int j = 0; j < mbRecordCount; j++)
                        {
                            int frame = BitConverter.ToInt32(data, i); i += 4;
                            int x = BitConverter.ToInt32(data, i); i += 4;
                            int y = BitConverter.ToInt32(data, i); i += 4;
                            mbRecord.Add(frame, new Vector2(x, y));
                        }
                        break;

                    case 2:
                        int mbWheelCount = BitConverter.ToInt32(data, i); i += 4;
                        for (int j = 0; j < mbWheelCount; j++)
                        {
                            int frame = BitConverter.ToInt32(data, i); i += 4;
                            int wheel = BitConverter.ToInt32(data, i); i += 4;
                            mbWheelRecord.Add(frame, wheel);
                        }
                        break;
                }
            }
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

        public byte[] ToData()
        {
            //id length data ...

            List<byte> data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(frames));

            data.Add(0); //id
            data.AddRange(BitConverter.GetBytes(keyRecord.Count)); //length

            int[] keys = keyRecord.Keys.ToArray();
            List<Command>[] commands = keyRecord.Values.ToArray();
            for (int i = 0; i < keyRecord.Count; i++)
            {
                data.AddRange(BitConverter.GetBytes(keys[i])); //frame
                data.Add((byte)commands[i].Count); //commands count
                for (int j = 0; j < commands[i].Count; j++)
                {
                    data.AddRange(BitConverter.GetBytes((short)commands[i][j].key.id)); //key
                    data.Add((byte)(commands[i][j].press ? 1 : 0)); //press/release
                }
            }

            if (mouseInput)
            {
                #region Mouse

                data.Add(1); //1
                data.AddRange(BitConverter.GetBytes(mbRecord.Count));
                keys = mbRecord.Keys.ToArray();
                Vector2[] pos = mbRecord.Values.ToArray(); 
                for (int i = 0; i < mbRecord.Count; i++)
                {
                    data.AddRange(BitConverter.GetBytes(keys[i])); //frame
                    data.AddRange(BitConverter.GetBytes((int)pos[i].X)); //x
                    data.AddRange(BitConverter.GetBytes((int)pos[i].Y)); //y
                }

                data.Add(2); //2
                data.AddRange(BitConverter.GetBytes(mbWheelRecord.Count));
                keys = mbWheelRecord.Keys.ToArray();
                int[] wheels = mbWheelRecord.Values.ToArray();
                for (int i = 0; i < mbWheelRecord.Count; i++)
                {
                    data.AddRange(BitConverter.GetBytes(keys[i])); //frame
                    data.AddRange(BitConverter.GetBytes(wheels[i])); //wheel
                }

                #endregion
            }

            return data.ToArray();
        }

        public void FromData(byte[] data)
        {

        }

        private struct Command
        {
            public KeyP key;
            public bool press;

            public Command(KeyP key, bool press)
            {
                this.key = key;
                this.press = press;
            }
        }
    }
}
