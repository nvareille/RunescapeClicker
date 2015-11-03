using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Runescape_Clicker.ViewModels;

namespace Runescape_Clicker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public KeyboardHooker KeyboardHooker;
        public MouseHooker MouseHooker;

        public App()
        {
            KeyboardHooker = new KeyboardHooker();
            MouseHooker = new MouseHooker();

            KeyboardHooker.ActivateHook();
            MouseHooker.ActivateHook();
        }
    }
}
