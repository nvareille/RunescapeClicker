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
    /// Interaction logic for LoadPattern.xaml
    /// </summary>
    public partial class LoadPatternView : Window
    {
        public MouseInterpreter Interpreter;

        public LoadPatternView()
        {
            InitializeComponent();
            Interpreter = new MouseInterpreter();
        }

        private void ExecuteActions(object sender, RoutedEventArgs e)
        {
            Interpreter.LoadActions("pattern.rsclicker");
            Interpreter.ExecuteActions();
        }
    }
}
