using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool IsRecording;
        private MouseCatcher mouseCatcher;

        public NewPatternView()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            mouseCatcher = new MouseCatcher();
            mouseCatcher.window = this;
        }

        private void StartRecording(object sender, RoutedEventArgs e)
        {
            IsRecording = !IsRecording;

            if (IsRecording)
            {
                RecordingButton.Content = "Click to stop recording";
                mouseCatcher.ActivateHook((bool)Absolute.IsChecked ? "ABSOLUTE" : "RELATIVE");
            }
            else
            {
                RecordingButton.Content = "Start Recording";
                mouseCatcher.DeactivateHook();
                mouseCatcher.Pattern.Save("pattern.rsclicker");
            }
        }
    }
}
