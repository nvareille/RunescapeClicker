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
    /// Interaction logic for NewPatternView.xaml
    /// </summary>
    public partial class NewPatternView : Window
    {
        public static NewPatternView r;
        private bool IsRecording;
        private MouseCatcher mouseCatcher;

        public NewPatternView()
        {
            InitializeComponent();
            KeyboardHooker.Instance += CatchTouch;
            r = this;
        }

        public void Reset()
        {
            mouseCatcher = new MouseCatcher();
        }

        private void StartRecording(object sender, RoutedEventArgs e)
        {
            IsRecording = !IsRecording;

            if (IsRecording)
            {
                RecordingButton.Content = "Click to stop recording";
                mouseCatcher.Start((bool)Absolute.IsChecked ? "ABSOLUTE" : "RELATIVE");
            }
            else
            {
                RecordingButton.Content = "Start Recording";
                mouseCatcher.Stop();
                mouseCatcher.Pattern.Save("pattern.rsclicker");
            }
        }

        private static void CatchTouch(KeyboardHooker hooker, int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && (int)wParam == KeyboardHooker.WM_KEYDOWN && Marshal.ReadInt32(lParam) == 162)
            {
                r.StartRecording(null, null);
            }
        }

        public void ClosingFct(object sender, CancelEventArgs a)
        {
            KeyboardHooker.Instance -= CatchTouch;
            mouseCatcher.Stop();
        }
    }
}
