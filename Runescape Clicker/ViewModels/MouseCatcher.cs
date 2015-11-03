using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Runescape_Clicker.Models;

namespace Runescape_Clicker.ViewModels
{
    public delegate void MouseTrigger(MouseCatcher m, HookStruct str);

    public class MouseCatcher
    {
        public static MouseCatcher MouseCatcherRef;
        public Pattern Pattern;
        private DateTime a = DateTime.Now;

        public MouseCatcher()
        {
            MouseCatcherRef = this;
        }

        public int GetElapsedTime()
        {
            TimeSpan b = DateTime.Now - a;
            a = DateTime.Now;
            return (Convert.ToInt32(b.TotalMilliseconds));
        }

        public static void MouseMove(MouseHooker hooker, MouseMessages message, HookStruct str)
        {
            MouseCatcher m = MouseCatcherRef;

            m.Pattern.Actions.Add("WAIT " + m.GetElapsedTime());
            m.Pattern.Actions.Add(str.pt.x + " " + str.pt.y);
        }

        public static void LeftClickUp(MouseHooker hook, MouseMessages message, HookStruct str)
        {
            MouseCatcher m = MouseCatcherRef;

            if (message == MouseMessages.WM_LBUTTONUP)
                m.Pattern.Actions.Add("LEFT_CLICK_UP");
        }

        public static void RightClickUp(MouseHooker hook, MouseMessages message, HookStruct str)
        {
            MouseCatcher m = MouseCatcherRef;

            if (message == MouseMessages.WM_RBUTTONUP)
                m.Pattern.Actions.Add("RIGHT_CLICK_UP");
        }
        public static void LeftClickDown(MouseHooker hook, MouseMessages message, HookStruct str)
        {
            MouseCatcher m = MouseCatcherRef;

            if (message == MouseMessages.WM_LBUTTONDOWN)
                m.Pattern.Actions.Add("LEFT_CLICK_DOWN");
        }

        public static void RightClickDown(MouseHooker hook, MouseMessages message, HookStruct str)
        {
            MouseCatcher m = MouseCatcherRef;

            if (message == MouseMessages.WM_RBUTTONDOWN)
                m.Pattern.Actions.Add("RIGHT_CLICK_DOWN");
        }

        public void Start(string mode)
        {
            Pattern = new Pattern();

            Pattern.Actions.Add(mode);
            GetElapsedTime();
            MouseHooker.Instance += MouseMove;
            MouseHooker.Instance += LeftClickDown;
            MouseHooker.Instance += LeftClickUp;
            MouseHooker.Instance += RightClickDown;
            MouseHooker.Instance += RightClickUp;
        }

        public void Stop()
        {
            MouseHooker.Instance -= MouseMove;
            MouseHooker.Instance -= LeftClickDown;
            MouseHooker.Instance -= LeftClickUp;
            MouseHooker.Instance -= RightClickDown;
            MouseHooker.Instance -= RightClickUp;
        }
    }
}
