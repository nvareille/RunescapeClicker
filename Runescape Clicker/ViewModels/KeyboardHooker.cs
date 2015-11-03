using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Runescape_Clicker.ViewModels
{
    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    public delegate void KeyboardHookFunction(KeyboardHooker Hooker, int code, IntPtr wParam, IntPtr lParam);
    
    public class KeyboardHooker
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x100;

        public LowLevelKeyboardProc HookHandleFct;
        public static IntPtr Hook;
        private List<KeyboardHookFunction> Functions;
        public static KeyboardHooker Instance;

        public KeyboardHooker()
        {
            Functions = new List<KeyboardHookFunction>();
            Instance = this;
        }

        public static KeyboardHooker operator+(KeyboardHooker Hooker, KeyboardHookFunction fct)
        {
            Hooker.Functions.Add(fct);
            return (Hooker);
        }

        public static KeyboardHooker operator-(KeyboardHooker Hooker, KeyboardHookFunction fct)
        {
            Hooker.Functions.Remove(fct);
            return (Hooker);
        }

        public void ActivateHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            HookHandleFct = HookHandle;
            Hook = SetWindowsHookEx(WH_KEYBOARD_LL, HookHandleFct, hInstance, 0);
        }

        public void DeactivateHook()
        {
            UnhookWindowsHookEx(Hook);
        }

        public static IntPtr HookHandleStatic(int code, IntPtr wParam, IntPtr lParam)
        {
            return (Instance.HookHandle(code, wParam, lParam));
        }

        public IntPtr HookHandle(int code, IntPtr wParam, IntPtr lParam)
        {
            foreach (KeyboardHookFunction f in Functions)
            {
                f(this, code, wParam, lParam);
            }
            return (CallNextHookEx(Hook, code, (int)wParam, lParam));
        }
    }
}
