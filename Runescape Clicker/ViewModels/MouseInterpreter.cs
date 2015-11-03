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
    public class MouseInterpreter
    {
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

        public bool Started;
        public Thread Thread;
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

        public void Start()
        {
            Started = true;
            LoadActions("pattern.rsclicker");
            Thread = new Thread(ExecuteActions);
            Thread.Start();
        }

        public void Stop()
        {
            Started = false;
            Thread.Abort();
        }

        public void LoadActions(string path)
        {
            Actions = new Pattern();

            Actions.Load(path);
        }

        public void ExecuteActions()
        {
            bool isRelative = false;
            POINT pos = new POINT();
            POINT click = new POINT();
            POINT start = new POINT();

            string first = Actions.GetNextAction();
            if (first.Contains("RELATIVE"))
            {
                isRelative = true;
                Actions.GetNextAction();
                first = Actions.GetNextAction();

                GetCursorPos(out click);

                string[] t = first.Split(' ');
                start.x = Convert.ToInt32(t[0]);
                start.y = Convert.ToInt32(t[1]);

            }
            while (Actions.Actions.Count > 0)
            {
                string str = Actions.GetNextAction();

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
                        b = DateTime.Now;
                    }
                }
                else
                {
                    string[] nbrs = str.Split(' ');

                    int x = Convert.ToInt32(nbrs[0]);
                    int y = Convert.ToInt32(nbrs[1]);

                    pos.x = x;
                    pos.y = y;

                    if (isRelative)
                    {
                        pos.x = pos.x - start.x + click.x;
                        pos.y = pos.y - start.y + click.y;
                    }

                    SetCursorPos(pos.x, pos.y);
                }
            }
            Started = false;
        }
    }
}
