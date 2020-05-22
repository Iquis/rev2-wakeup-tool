using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GGXrdWakeupDPUtil.Library
{
    public class Keyboard
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Keyboard.Input[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        public void SendKey(
          Keyboard.DirectXKeyStrokes key,
          bool KeyUp,
          Keyboard.InputType inputType)
        {
            uint num1 = !KeyUp ? 8U : 10U;
            Keyboard.Input[] pInputs = new Keyboard.Input[1]
            {
        new Keyboard.Input()
        {
          type = (int) inputType,
          u = new Keyboard.InputUnion()
          {
            ki = new Keyboard.KeyboardInput()
            {
              wVk = (ushort) 0,
              wScan = (ushort) key,
              dwFlags = num1,
              dwExtraInfo = Keyboard.GetMessageExtraInfo()
            }
          }
        }
            };
            int num2 = (int)Keyboard.SendInput((uint)pInputs.Length, pInputs, Marshal.SizeOf(typeof(Keyboard.Input)));
        }

        public void SendKey(ushort key, bool KeyUp, Keyboard.InputType inputType)
        {
            uint num1 = !KeyUp ? 8U : 10U;
            Keyboard.Input[] pInputs = new Keyboard.Input[1]
            {
        new Keyboard.Input()
        {
          type = (int) inputType,
          u = new Keyboard.InputUnion()
          {
            ki = new Keyboard.KeyboardInput()
            {
              wVk = (ushort) 0,
              wScan = key,
              dwFlags = num1,
              dwExtraInfo = Keyboard.GetMessageExtraInfo()
            }
          }
        }
            };
            int num2 = (int)Keyboard.SendInput((uint)pInputs.Length, pInputs, Marshal.SizeOf(typeof(Keyboard.Input)));
        }

        [Flags]
        public enum InputType
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2,
        }

        [Flags]
        public enum KeyEventF
        {
            KeyDown = 0,
            ExtendedKey = 1,
            KeyUp = 2,
            Unicode = 4,
            Scancode = 8,
        }

        public enum DirectXKeyStrokes
        {
            DIK_ESCAPE = 1,
            DIK_1 = 2,
            DIK_2 = 3,
            DIK_3 = 4,
            DIK_4 = 5,
            DIK_5 = 6,
            DIK_6 = 7,
            DIK_7 = 8,
            DIK_8 = 9,
            DIK_9 = 10, // 0x0000000A
            DIK_0 = 11, // 0x0000000B
            DIK_MINUS = 12, // 0x0000000C
            DIK_EQUALS = 13, // 0x0000000D
            DIK_BACK = 14, // 0x0000000E
            DIK_BACKSPACE = 14, // 0x0000000E
            DIK_TAB = 15, // 0x0000000F
            DIK_Q = 16, // 0x00000010
            DIK_W = 17, // 0x00000011
            DIK_E = 18, // 0x00000012
            DIK_R = 19, // 0x00000013
            DIK_T = 20, // 0x00000014
            DIK_Y = 21, // 0x00000015
            DIK_U = 22, // 0x00000016
            DIK_I = 23, // 0x00000017
            DIK_O = 24, // 0x00000018
            DIK_P = 25, // 0x00000019
            DIK_LBRACKET = 26, // 0x0000001A
            DIK_RBRACKET = 27, // 0x0000001B
            DIK_RETURN = 28, // 0x0000001C
            DIK_LCONTROL = 29, // 0x0000001D
            DIK_A = 30, // 0x0000001E
            DIK_S = 31, // 0x0000001F
            DIK_D = 32, // 0x00000020
            DIK_F = 33, // 0x00000021
            DIK_G = 34, // 0x00000022
            DIK_H = 35, // 0x00000023
            DIK_J = 36, // 0x00000024
            DIK_K = 37, // 0x00000025
            DIK_L = 38, // 0x00000026
            DIK_SEMICOLON = 39, // 0x00000027
            DIK_APOSTROPHE = 40, // 0x00000028
            DIK_GRAVE = 41, // 0x00000029
            DIK_LSHIFT = 42, // 0x0000002A
            DIK_BACKSLASH = 43, // 0x0000002B
            DIK_Z = 44, // 0x0000002C
            DIK_X = 45, // 0x0000002D
            DIK_C = 46, // 0x0000002E
            DIK_V = 47, // 0x0000002F
            DIK_B = 48, // 0x00000030
            DIK_N = 49, // 0x00000031
            DIK_M = 50, // 0x00000032
            DIK_COMMA = 51, // 0x00000033
            DIK_PERIOD = 52, // 0x00000034
            DIK_SLASH = 53, // 0x00000035
            DIK_RSHIFT = 54, // 0x00000036
            DIK_MULTIPLY = 55, // 0x00000037
            DIK_NUMPADSTAR = 55, // 0x00000037
            DIK_LALT = 56, // 0x00000038
            DIK_LMENU = 56, // 0x00000038
            DIK_SPACE = 57, // 0x00000039
            DIK_CAPITAL = 58, // 0x0000003A
            DIK_CAPSLOCK = 58, // 0x0000003A
            DIK_F1 = 59, // 0x0000003B
            DIK_F2 = 60, // 0x0000003C
            DIK_F3 = 61, // 0x0000003D
            DIK_F4 = 62, // 0x0000003E
            DIK_F5 = 63, // 0x0000003F
            DIK_F6 = 64, // 0x00000040
            DIK_F7 = 65, // 0x00000041
            DIK_F8 = 66, // 0x00000042
            DIK_F9 = 67, // 0x00000043
            DIK_F10 = 68, // 0x00000044
            DIK_NUMLOCK = 69, // 0x00000045
            DIK_SCROLL = 70, // 0x00000046
            DIK_NUMPAD7 = 71, // 0x00000047
            DIK_NUMPAD8 = 72, // 0x00000048
            DIK_NUMPAD9 = 73, // 0x00000049
            DIK_NUMPADMINUS = 74, // 0x0000004A
            DIK_SUBTRACT = 74, // 0x0000004A
            DIK_NUMPAD4 = 75, // 0x0000004B
            DIK_NUMPAD5 = 76, // 0x0000004C
            DIK_NUMPAD6 = 77, // 0x0000004D
            DIK_ADD = 78, // 0x0000004E
            DIK_NUMPADPLUS = 78, // 0x0000004E
            DIK_NUMPAD1 = 79, // 0x0000004F
            DIK_NUMPAD2 = 80, // 0x00000050
            DIK_NUMPAD3 = 81, // 0x00000051
            DIK_NUMPAD0 = 82, // 0x00000052
            DIK_DECIMAL = 83, // 0x00000053
            DIK_NUMPADPERIOD = 83, // 0x00000053
            DIK_F11 = 87, // 0x00000057
            DIK_F12 = 88, // 0x00000058
            DIK_F13 = 100, // 0x00000064
            DIK_F14 = 101, // 0x00000065
            DIK_F15 = 102, // 0x00000066
            DIK_KANA = 112, // 0x00000070
            DIK_CONVERT = 121, // 0x00000079
            DIK_NOCONVERT = 123, // 0x0000007B
            DIK_YEN = 125, // 0x0000007D
            DIK_NUMPADEQUALS = 141, // 0x0000008D
            DIK_CIRCUMFLEX = 144, // 0x00000090
            DIK_AT = 145, // 0x00000091
            DIK_COLON = 146, // 0x00000092
            DIK_UNDERLINE = 147, // 0x00000093
            DIK_KANJI = 148, // 0x00000094
            DIK_STOP = 149, // 0x00000095
            DIK_AX = 150, // 0x00000096
            DIK_UNLABELED = 151, // 0x00000097
            DIK_NUMPADENTER = 156, // 0x0000009C
            DIK_RCONTROL = 157, // 0x0000009D
            DIK_NUMPADCOMMA = 179, // 0x000000B3
            DIK_DIVIDE = 181, // 0x000000B5
            DIK_NUMPADSLASH = 181, // 0x000000B5
            DIK_SYSRQ = 183, // 0x000000B7
            DIK_RALT = 184, // 0x000000B8
            DIK_RMENU = 184, // 0x000000B8
            DIK_HOME = 199, // 0x000000C7
            DIK_UP = 200, // 0x000000C8
            DIK_UPARROW = 200, // 0x000000C8
            DIK_PGUP = 201, // 0x000000C9
            DIK_PRIOR = 201, // 0x000000C9
            DIK_LEFT = 203, // 0x000000CB
            DIK_LEFTARROW = 203, // 0x000000CB
            DIK_RIGHT = 205, // 0x000000CD
            DIK_RIGHTARROW = 205, // 0x000000CD
            DIK_END = 207, // 0x000000CF
            DIK_DOWN = 208, // 0x000000D0
            DIK_DOWNARROW = 208, // 0x000000D0
            DIK_NEXT = 209, // 0x000000D1
            DIK_PGDN = 209, // 0x000000D1
            DIK_INSERT = 210, // 0x000000D2
            DIK_DELETE = 211, // 0x000000D3
            DIK_LWIN = 219, // 0x000000DB
            DIK_RWIN = 220, // 0x000000DC
            DIK_APPS = 221, // 0x000000DD
            DIK_LEFTMOUSEBUTTON = 256, // 0x00000100
            DIK_RIGHTMOUSEBUTTON = 257, // 0x00000101
            DIK_MIDDLEWHEELBUTTON = 258, // 0x00000102
            DIK_MOUSEBUTTON3 = 259, // 0x00000103
            DIK_MOUSEBUTTON4 = 260, // 0x00000104
            DIK_MOUSEBUTTON5 = 261, // 0x00000105
            DIK_MOUSEBUTTON6 = 262, // 0x00000106
            DIK_MOUSEBUTTON7 = 263, // 0x00000107
            DIK_MOUSEWHEELUP = 264, // 0x00000108
            DIK_MOUSEWHEELDOWN = 265, // 0x00000109
        }

        public struct Input
        {
            public int type;
            public Keyboard.InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public readonly Keyboard.MouseInput mi;
            [FieldOffset(0)]
            public Keyboard.KeyboardInput ki;
            [FieldOffset(0)]
            public readonly Keyboard.HardwareInput hi;
        }

        public struct MouseInput
        {
            public readonly int dx;
            public readonly int dy;
            public readonly uint mouseData;
            public readonly uint dwFlags;
            public readonly uint time;
            public readonly IntPtr dwExtraInfo;
        }

        public struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public readonly uint time;
            public IntPtr dwExtraInfo;
        }

        public struct HardwareInput
        {
            public readonly uint uMsg;
            public readonly ushort wParamL;
            public readonly ushort wParamH;
        }
    }
}
