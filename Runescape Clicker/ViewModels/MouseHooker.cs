using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Runescape_Clicker.ViewModels
{
    public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
    public delegate void MouseHookFunction(MouseHooker m, MouseMessages message, HookStruct str);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.x, point.y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class HookStruct
    {
        public POINT pt;
        public int hwnd;
        public int wHitTestCode;
        public int dwExtraInfo;
    }

    public enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    public class MouseHooker
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        private const int WH_MOUSE_LL = 14;

        private LowLevelMouseProc HookHandleFct;
        private IntPtr Hook;
        private List<MouseHookFunction> Functions;
        public static MouseHooker Instance;

        public MouseHooker()
        {
            Functions = new List<MouseHookFunction>();
            Instance = this;
        }

        public static MouseHooker operator+(MouseHooker Hooker, MouseHookFunction fct)
        {
            Hooker.Functions.Add(fct);
            return (Hooker);
        }

        public static MouseHooker operator -(MouseHooker Hooker, MouseHookFunction fct)
        {
            Hooker.Functions.Remove(fct);
            return (Hooker);
        }

        public void ActivateHook()
        {
            HookHandleFct = HookHandle;
            IntPtr hInstance = LoadLibrary("User32");
            Hook = SetWindowsHookEx(WH_MOUSE_LL, HookHandleFct, hInstance, 0);
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
            HookStruct hookStruct = (HookStruct)Marshal.PtrToStructure(lParam, typeof(HookStruct));
            foreach (MouseHookFunction f in Functions)
            {
                f(this, (MouseMessages)wParam, hookStruct);
            }
            return (CallNextHookEx(Instance.Hook, code, (int)wParam, lParam));
        }
    }
}
