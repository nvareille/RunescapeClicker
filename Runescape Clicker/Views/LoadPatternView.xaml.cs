using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Runescape_Clicker.ViewModels;

namespace Runescape_Clicker.Views
{
    /// <summary>
    /// Interaction logic for LoadPattern.xaml
    /// </summary>
    public partial class LoadPatternView : Window
    {
        public static LoadPatternView r;
        public MouseInterpreter Interpreter;

        public LoadPatternView()
        {
            InitializeComponent();
            Interpreter = new MouseInterpreter();
            KeyboardHooker.Instance += TouchAction;
            r = this;
        }

        private void ExecuteActions(object sender, RoutedEventArgs e)
        {
            if (!Interpreter.Started)
            {
                Interpreter.Start();
            }
            else
            {
                Interpreter.Stop();
            }

        }

        private static void TouchAction(KeyboardHooker k, int code, IntPtr wParam, IntPtr lParam)
        {
            if ((int)wParam == KeyboardHooker.WM_KEYDOWN && Marshal.ReadInt32(lParam) == 162)
                r.ExecuteActions(null, null);
        }

        public void ClosingFct(object sender, CancelEventArgs a)
        {
            KeyboardHooker.Instance -= TouchAction;
        }
    }
}
