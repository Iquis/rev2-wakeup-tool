using System.Runtime.InteropServices;

namespace GGXrdReversalTool.Library.Input;

internal static class InputManager
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
    
    [DllImport("user32.dll")]
    private static extern IntPtr GetMessageExtraInfo();
    
    public static void SendKey(DirectXKeyStrokes key, bool keyUp, InputType inputType)
    {
        Input[] inputs = new[]
        {
            new Input()
            {
                type = (int)inputType,
                union = new InputUnion()
                {
                    ki = new KeyboardInput()
                    {
                        wVk = 0,
                        wScan = (ushort)key,
                        dwFlags = keyUp ? (uint) (KeyEventF.KeyUp | KeyEventF.Scancode) : (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            }
        };

        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
    }
}