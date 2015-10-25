﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Runescape_Clicker.Models;

namespace Runescape_Clicker.ViewModels
{
    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
        public int x;
        public int y;

        public POINT Clone()
        {
            POINT copy = new POINT();

            copy.x = x;
            copy.y = y;
            return (copy);
        }

        public static bool operator==(POINT p1, POINT p2)
        {
            Console.WriteLine(p1.x + " " + p1.y + " <=> " + p2.x + " " + p2.y);
            return (p1.x == p2.x && p1.y == p2.y);
        }

        public static bool operator!=(POINT p1, POINT p2)
        {
            return (!(p1 == p2));
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

    public delegate void MouseTrigger(MouseCatcher m, HookStruct str);

    public class MouseCatcher
    {
        public POINT pt;
        public static MouseCatcher MouseCatcherRef;
        public static Dictionary<MouseMessages, MouseTrigger> Actions;
        public Pattern Pattern;

        public static void MouseMove(MouseCatcher m, HookStruct str)
        {
            if (m.pt != str.pt)
                m.Pattern.Actions.Add(str.pt.x + " " + str.pt.y);
        }

        public static void LeftClick(MouseCatcher m, HookStruct str)
        {
            m.Pattern.Actions.Add("LEFT_CLICK");
        }

        public static void RightClick(MouseCatcher m, HookStruct str)
        {
            m.Pattern.Actions.Add("RIGHT_CLICK");
        }

        public void Init()
        {
            if (MouseCatcherRef == null)
            {
                pt = new POINT();
                MouseCatcherRef = this;
                Actions = new Dictionary<MouseMessages, MouseTrigger>();

                Actions.Add(MouseMessages.WM_MOUSEMOVE, MouseMove);
                Actions.Add(MouseMessages.WM_LBUTTONDOWN, LeftClick);
                Actions.Add(MouseMessages.WM_RBUTTONDOWN, RightClick);
            }
        }

        public void ActivateHook(string init)
        {
            Init();
            Pattern = new Pattern();
            Pattern.Actions.Add(init);
            _hookID = SetHook(_proc);
        }

        public void DeactivateHook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                HookStruct hookStruct = (HookStruct)Marshal.PtrToStructure(lParam, typeof(HookStruct));
                if (Actions.ContainsKey((MouseMessages)wParam))
                {
                    Actions[(MouseMessages) wParam](MouseCatcherRef, hookStruct);
                }
                MouseCatcherRef.pt = hookStruct.pt.Clone();
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
          LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
          IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
