using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JuliHelper
{
    public static class Input
    {
        public enum KeyState { Pressed, Press, Released, None };

        //public static bool clearKeyStringOnLoop = true;
        public static bool deCatchOnLoop = true, deCatchOnRelease = false;

        #region Keys

        public static Key q = new Key(Keys.Q, "q", "Q", "@");
        public static Key w = new Key(Keys.W, "w", "W");
        public static Key e = new Key(Keys.E, "e", "E");
        public static Key r = new Key(Keys.R, "r", "R");
        public static Key t = new Key(Keys.T, "t", "T");
        public static Key z = new Key(Keys.Z, "z", "Z");
        public static Key u = new Key(Keys.U, "u", "U");
        public static Key i = new Key(Keys.I, "i", "I");
        public static Key o = new Key(Keys.O, "o", "O");
        public static Key p = new Key(Keys.P, "p", "P");
        public static Key a = new Key(Keys.A, "a", "A");
        public static Key s = new Key(Keys.S, "s", "S");
        public static Key d = new Key(Keys.D, "d", "D");
        public static Key f = new Key(Keys.F, "f", "F");
        public static Key g = new Key(Keys.G, "g", "G");
        public static Key h = new Key(Keys.H, "h", "H");
        public static Key j = new Key(Keys.J, "j", "J");
        public static Key k = new Key(Keys.K, "k", "K");
        public static Key l = new Key(Keys.L, "l", "L");
        public static Key y = new Key(Keys.Y, "y", "Y");
        public static Key x = new Key(Keys.X, "x", "X");
        public static Key c = new Key(Keys.C, "c", "C");
        public static Key v = new Key(Keys.V, "v", "V");
        public static Key b = new Key(Keys.B, "b", "B");
        public static Key n = new Key(Keys.N, "n", "N");
        public static Key m = new Key(Keys.M, "m", "M");
        public static Key space = new Key(Keys.Space, " ", " ");
        public static Key d1 = new Key(Keys.D1, "1", "!");
        public static Key d2 = new Key(Keys.D2, "2", '"'.ToString(), "²");
        public static Key d3 = new Key(Keys.D3, "3", "§", "³");
        public static Key d4 = new Key(Keys.D4, "4", "$");
        public static Key d5 = new Key(Keys.D5, "5", "%");
        public static Key d6 = new Key(Keys.D6, "6", "&");
        public static Key d7 = new Key(Keys.D7, "7", "/", "{");
        public static Key d8 = new Key(Keys.D8, "8", "(", "[");
        public static Key d9 = new Key(Keys.D9, "9", ")", "]");
        public static Key d0 = new Key(Keys.D0, "0", "=", "}");
        public static Key enter = new Key(Keys.Enter, "\n", "\n");
        public static Key left = new Key(Keys.Left);
        public static Key right = new Key(Keys.Right);
        public static Key up = new Key(Keys.Up);
        public static Key down = new Key(Keys.Down);
        public static Key leftShift = new Key(Keys.LeftShift);
        public static Key rightShift = new Key(Keys.RightShift);
        public static Key leftControl = new Key(Keys.LeftControl);
        public static Key rightControl = new Key(Keys.RightControl);
        public static Key leftAlt = new Key(Keys.LeftAlt);
        public static Key rightAlt = new Key(Keys.RightAlt);
        public static Key backSpace = new Key(Keys.Back);
        public static Key delete = new Key(Keys.Delete);
        public static Key esc = new Key(Keys.Escape);
        public static Key num1 = new Key(Keys.NumPad1, "1", "1");
        public static Key num2 = new Key(Keys.NumPad2, "2", "2");
        public static Key num3 = new Key(Keys.NumPad3, "3", "3");
        public static Key num4 = new Key(Keys.NumPad4, "4", "4");
        public static Key num5 = new Key(Keys.NumPad5, "5", "5");
        public static Key num6 = new Key(Keys.NumPad6, "6", "6");
        public static Key num7 = new Key(Keys.NumPad7, "7", "7");
        public static Key num8 = new Key(Keys.NumPad8, "8", "8");
        public static Key num9 = new Key(Keys.NumPad9, "9", "9");
        public static Key num0 = new Key(Keys.NumPad0, "0", "0");
        public static Key plusNum = new Key(Keys.Add, "+", "+");
        public static Key minusNum = new Key(Keys.Subtract, "-", "-");
        public static Key multiplyNum = new Key(Keys.Multiply, "*", "*");
        public static Key divideNum = new Key(Keys.Divide, "/", "/");
        public static Key tab = new Key(Keys.Tab, "    ", "    ");
        public static Key dot = new Key(Keys.OemPeriod, ".", ":");
        public static Key comma = new Key(Keys.OemComma, ",", ";");
        public static Key smaller = new Key(Keys.OemBackslash, "<", ">", "|");
        public static Key sharp = new Key(Keys.OemQuestion, "#", "'");
        public static Key plus = new Key(Keys.OemPlus, "+", "*", "~");
        public static Key minus = new Key(Keys.OemMinus, "-", "_");
        public static Key insert = new Key(Keys.Insert);
        public static Key end = new Key(Keys.End);
        public static Key f1 = new Key(Keys.F1);
        public static Key f2 = new Key(Keys.F2);
        public static Key f3 = new Key(Keys.F3);
        public static Key f4 = new Key(Keys.F4);
        public static Key f5 = new Key(Keys.F5);
        public static Key f6 = new Key(Keys.F6);
        public static Key f7 = new Key(Keys.F7);
        public static Key f8 = new Key(Keys.F8);
        public static Key f9 = new Key(Keys.F9);
        public static Key f10 = new Key(Keys.F10);
        public static Key f11 = new Key(Keys.F11);
        public static Key f12 = new Key(Keys.F12);
        public static Key ä = new Key(Keys.OemQuotes, "ä", "Ä");
        public static Key ö = new Key(Keys.OemTilde, "ö", "Ö");
        public static Key ü = new Key(Keys.OemSemicolon, "ü", "Ü");
        public static Key none = new Key(Keys.None);
        public static Key auto = new Key(Keys.OemAuto);
        public static Key clear = new Key(Keys.OemClear);
        public static Key akut = new Key(Keys.OemCloseBrackets, "´", "`");
        public static Key copy = new Key(Keys.OemCopy);
        public static Key enlW = new Key(Keys.OemEnlW);
        public static Key sharpS = new Key(Keys.OemOpenBrackets, "ß", "?", "\\");
        public static Key pipe = new Key(Keys.OemPipe, "^", "°");
        public static Key quotes = new Key(Keys.OemQuotes);
        public static Key semicolon = new Key(Keys.OemSemicolon);
        public static Key tilde = new Key(Keys.OemTilde);

        public static KeyCollection anyShift = new KeyCollection(leftShift, rightShift);
        public static KeyCollection anyControl = new KeyCollection(leftControl, rightControl);
        public static KeyCollection anyAlt = new KeyCollection(leftAlt, rightAlt);

        public static bool HasTyped(string text, int timeForCharacter = 30)
        {
            int cTime = 1;

            if (!GetKeyFromStringLow(text[text.Length - 1]).pressed)
                return false;

            for (int i = text.Length - 2; i >= 0; i--)
            {
                Key key;
                if ((key = GetKeyFromStringLow(text[i])) != null)
                {
                    if (key.TimeDown > cTime - timeForCharacter && key.TimeDown < cTime)
                        cTime = key.TimeDown;
                    else
                        return false;
                }
                else
                    return false;
            }
            return true;
        }

        public static Key GetKeyFromStringLow(string strLow)
        {
            return (Key)keys.First(f => f is Key && ((Key)f).keyStringLow == strLow);
        }
        public static Key GetKeyFromStringLow(char c)
        {
            return GetKeyFromStringLow(c.ToString());
        }


        #endregion

        public static KeyMouse mbLeft = new KeyMouse();
        public static KeyMouse mbRight = new KeyMouse();
        public static KeyMouse mbMiddle = new KeyMouse();

        public static Vector2 mbPos;
        public static Vector2 mbPosPast;
        /// <summary>
        /// how long the mouse hasn't been moved in frames
        /// </summary>
        public static int mouseStillFrames;
        /// <summary>
        /// time to hit the mouse button again in order to count as a double press
        /// </summary>
        public static int doublePressTimeMax = 30;

        public static int mbWheel = 0;
        private static int lastMbWheel = 0;

        public static KeyboardState ks;
        public static GamePadState[] gpStates;

        public static Gamepad[] controllers;

        public static KeyP[] keys;
        public static KeyCollection[] keyCollections;
        public static List<KeyCollection> customKeyCollections;
		public static KeyP[] digits = new KeyP[]
		{
			d0, d1, d2, d3, d4, d5, d6, d7, d8, d9
		};

        public static bool blockInput;

        public static int? numberPressed;

        public class Gamepad
        {
            int playerIndex;

            public KeyPad a;
            public KeyPad b;
            public KeyPad x;
            public KeyPad y;
            public KeyPad back;
            public KeyPad start;
            public KeyPad bigButton;
            public KeyPad dPadDown;
            public KeyPad dPadLeft;
            public KeyPad dPadRight;
            public KeyPad dPadUp;
            public KeyPad leftShoulder;
            public KeyPad rightShoulder;
            public KeyPad leftStick;
            public KeyPad rightStick;
            public KeyPad leftThumbstickDown;
            public KeyPad leftThumbstickUp;
            public KeyPad leftThumbstickLeft;
            public KeyPad leftThumbstickRight;
            public KeyPad rightThumbstickDown;
            public KeyPad rightThumbstickUp;
            public KeyPad rightThumbstickLeft;
            public KeyPad rightThumbstickRight;
            public KeyPad leftTriggerButton;
            public KeyPad rightTriggerButton;

            public Vector2 leftThumbStick;
            public Vector2 rightThumbStick;
            public float leftTrigger;
            public float rightTrigger;


            public Gamepad(int playerIndex)
            {
                this.playerIndex = playerIndex;

                a = new KeyPad(Buttons.A, playerIndex);
                b = new KeyPad(Buttons.B, playerIndex);
                x = new KeyPad(Buttons.X, playerIndex);
                y = new KeyPad(Buttons.Y, playerIndex);
                back = new KeyPad(Buttons.Back, playerIndex);
                start = new KeyPad(Buttons.Start, playerIndex);
                bigButton = new KeyPad(Buttons.BigButton, playerIndex);
                dPadDown = new KeyPad(Buttons.DPadDown, playerIndex);
                dPadLeft = new KeyPad(Buttons.DPadLeft, playerIndex);
                dPadRight = new KeyPad(Buttons.DPadRight, playerIndex);
                dPadUp = new KeyPad(Buttons.DPadUp, playerIndex);
                leftShoulder = new KeyPad(Buttons.LeftShoulder, playerIndex);
                rightShoulder = new KeyPad(Buttons.RightShoulder, playerIndex);
                leftStick = new KeyPad(Buttons.LeftStick, playerIndex);
                rightStick = new KeyPad(Buttons.RightStick, playerIndex);
                leftThumbstickDown = new KeyPad(Buttons.LeftThumbstickDown, playerIndex);
                leftThumbstickUp = new KeyPad(Buttons.LeftThumbstickUp, playerIndex);
                leftThumbstickLeft = new KeyPad(Buttons.LeftThumbstickLeft, playerIndex);
                leftThumbstickRight = new KeyPad(Buttons.LeftThumbstickRight, playerIndex);
                rightThumbstickDown = new KeyPad(Buttons.RightThumbstickDown, playerIndex);
                rightThumbstickUp = new KeyPad(Buttons.RightThumbstickUp, playerIndex);
                rightThumbstickLeft = new KeyPad(Buttons.RightThumbstickLeft, playerIndex);
                rightThumbstickRight = new KeyPad(Buttons.RightThumbstickRight, playerIndex);
                leftTriggerButton = new KeyPad(Buttons.LeftTrigger, playerIndex);
                rightTriggerButton = new KeyPad(Buttons.RightTrigger, playerIndex);

            }
        }

        public static void Initialize()
        {
            keys = new KeyP[]{ a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z, space, d0, d1, d2, d3, d4, d5, d6, d7, d8, d9, enter, left, right, up, down,
                leftShift, rightShift, leftControl, rightControl, leftAlt, rightAlt, backSpace, delete, esc, num0, num1, num2, num3, num4, num5, num6, num7, num8, num9, plusNum, minusNum,
                multiplyNum, divideNum, tab, dot, comma, smaller, sharp, plus, minus, insert, end, f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, ä, ö, ü, mbLeft, mbRight, mbMiddle,
                auto, clear, akut, copy, enlW, sharpS, pipe, quotes, semicolon, tilde};

            keyCollections = new KeyCollection[] { anyShift, anyControl, anyAlt };
            customKeyCollections = new List<KeyCollection>();

            ClearKeys();

            esc.KeyStringDown += btn => KeyString.Escape();
            a.KeyStringControlDown += btn => KeyString.SelectAll();
            v.KeyStringControlDown += btn => KeyString.Paste();
            c.KeyStringControlDown += btn => KeyString.Copy();
            x.KeyStringControlDown += btn => KeyString.Cut();

            left.KeyStringDown += btn => KeyString.CursorMove(-1);
            right.KeyStringDown += btn => KeyString.CursorMove(1);
            backSpace.KeyStringDown += btn => KeyString.Delete(true);
            delete.KeyStringDown += btn => KeyString.Delete(false);
            

            gpStates = new GamePadState[4];
            gpStates[0] = GamePad.GetState(PlayerIndex.One);
            gpStates[1] = GamePad.GetState(PlayerIndex.Two);
            gpStates[2] = GamePad.GetState(PlayerIndex.Three);
            gpStates[3] = GamePad.GetState(PlayerIndex.Four);

            controllers = new Gamepad[4];
            for (int index = 0; index < 4; index++)
                controllers[index] = new Gamepad(index);

            KeyString.Initialize();
        }

        public static void ClearKeys()
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].down = keys[i].pressed = keys[i].released = false;
                keys[i].id = (short)i;
                keys[i].blocked = false;
                keys[i].catched = false;
            }
        }
        public static void DeblockKeys()
        {
            for (int i = 0; i < keys.Length; i++)
                keys[i].blocked = false;
        }


        public static void Update()
        {
            if (!blockInput)
            {
                ks = Keyboard.GetState();

                gpStates[0] = GamePad.GetState(PlayerIndex.One);
                gpStates[1] = GamePad.GetState(PlayerIndex.Two);
                gpStates[2] = GamePad.GetState(PlayerIndex.Three);
                gpStates[3] = GamePad.GetState(PlayerIndex.Four);

                #region Mouse

                MouseState ms = Mouse.GetState();

                mbPosPast = mbPos;
                mbPos = new Vector2(ms.X, ms.Y);
                if (mbPos != mbPosPast)
                    mouseStillFrames = 0;
                else
                    mouseStillFrames++;

                mbLeft.state = ms.LeftButton;
                mbRight.state = ms.RightButton;
                mbMiddle.state = ms.MiddleButton;

                mbWheel = lastMbWheel - ms.ScrollWheelValue;
                lastMbWheel = ms.ScrollWheelValue;

                #endregion

                for (int i = 0; i < keys.Length; i++)
                    keys[i].Update();

                for (int i = 0; i < keyCollections.Length; i++)
                    keyCollections[i].Update();

                for (int i = 0; i < customKeyCollections.Count; i++)
                    customKeyCollections[i].Update();

                #region Controller

                for (int index = 0; index < gpStates.Length; index++)
                {
                    if (gpStates[index].IsConnected)
                    {
                        controllers[index].a.Update();
                        controllers[index].b.Update();
                        controllers[index].x.Update();
                        controllers[index].y.Update();
                        controllers[index].back.Update();
                        controllers[index].start.Update();
                        controllers[index].bigButton.Update();
                        controllers[index].dPadDown.Update();
                        controllers[index].dPadLeft.Update();
                        controllers[index].dPadRight.Update();
                        controllers[index].dPadUp.Update();
                        controllers[index].leftShoulder.Update();
                        controllers[index].rightShoulder.Update();
                        controllers[index].leftStick.Update();
                        controllers[index].rightStick.Update();
                        controllers[index].leftThumbstickDown.Update();
                        controllers[index].leftThumbstickUp.Update();
                        controllers[index].leftThumbstickLeft.Update();
                        controllers[index].leftThumbstickRight.Update();
                        controllers[index].rightThumbstickDown.Update();
                        controllers[index].rightThumbstickUp.Update();
                        controllers[index].rightThumbstickLeft.Update();
                        controllers[index].rightThumbstickRight.Update();
                        controllers[index].leftTriggerButton.Update();
                        controllers[index].rightTriggerButton.Update();

                        controllers[index].leftThumbStick = gpStates[index].ThumbSticks.Left;
                        controllers[index].rightThumbStick = gpStates[index].ThumbSticks.Right;
                        controllers[index].leftTrigger = gpStates[index].Triggers.Left;
                        controllers[index].rightTrigger = gpStates[index].Triggers.Right;
                    }
                }

                #endregion

                //if (leftShift.pressed == KeyState.Press || rightShift.pressed == KeyState.Press)
                //    keyString = keyString.ToUpper();

                int d0Index = Array.IndexOf(keys, d0);
                numberPressed = null;

                for (int i = 0; i < 10; i++)
                {
                    if (keys[d0Index + i].pressed)
                    {
                        numberPressed = i;
                        break;
                    }
                }

                if (numberPressed == null)
                {
                    d0Index = Array.IndexOf(keys, num0);
                    numberPressed = null;

                    for (int i = 0; i < 10; i++)
                    {
                        if (keys[d0Index + i].pressed)
                        {
                            numberPressed = i;
                            break;
                        }
                    }
                }
            }
        }
    }

    public enum MouseButton
    {
        None,
        Left,
        Right,
        Middle
    }
}
