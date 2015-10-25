using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Runescape_Clicker.Models;

namespace Runescape_Clicker.ViewModels
{
    public class KeyboardHook
    {
        public static bool Stop = false;
        public static Mutex mutex = new Mutex();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;

        private LowLevelKeyboardProc _proc = hookProc;

        private static IntPtr hhook = IntPtr.Zero;

        public void SetHook()
        {
            Stop = false;
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }

        public static void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

        public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                Console.WriteLine("KEY");
                UnHook();
                Stop = true;
                mutex.ReleaseMutex();
                return (IntPtr)1;
            }
            mutex.ReleaseMutex();
            return CallNextHookEx(hhook, code, (int)wParam, lParam);
        }
    }

    public class MouseInterpreter
    {
        private KeyboardHook keyboardHook = new KeyboardHook();

        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public Pattern Actions;
        public Dictionary<string, MouseEventFlags> MouseCodes;

        public MouseInterpreter()
        {
            MouseCodes = new Dictionary<string, MouseEventFlags>();

            MouseCodes.Add("LEFT_CLICK_DOWN", MouseEventFlags.LeftDown);
            MouseCodes.Add("LEFT_CLICK_UP", MouseEventFlags.LeftUp);
            MouseCodes.Add("RIGHT_CLICK_DOWN", MouseEventFlags.RightDown);
            MouseCodes.Add("RIGHT_CLICK_UP", MouseEventFlags.RightUp);
        }

        public void LoadActions(string path)
        {
            Actions = new Pattern();

            Actions.Load(path);
        }

        public void ExecuteActions()
        {
            POINT pos = new POINT();

            keyboardHook.SetHook();
            Actions.GetNextAction();
            while (Actions.Actions.Count > 0)
            {
                KeyboardHook.mutex.WaitOne();
                if (KeyboardHook.Stop)
                    return;

                string str = Actions.GetNextAction();

                //Console.WriteLine(str);

                if (str.Contains("CLICK"))
                {
                    mouse_event((int)MouseCodes[str], pos.x, pos.y, 0, 0);
                }
                else if (str.Contains("WAIT"))
                {
                    str = str.Substring(5);
                    double ms = Convert.ToDouble(str);

                    DateTime a = DateTime.Now;
                    DateTime b = a;

                    while ((b - a).TotalMilliseconds < ms)
                    {
                        KeyboardHook.mutex.WaitOne();
                        b = DateTime.Now;
                        if (KeyboardHook.Stop)
                            return;
                    }
                }
                else
                {
                    string[] nbrs = str.Split(' ');

                    int x = Convert.ToInt32(nbrs[0]);
                    int y = Convert.ToInt32(nbrs[1]);

                    pos.x = x;
                    pos.y = y;
                    SetCursorPos(x, y);
                }
            }
        }
    }
}
