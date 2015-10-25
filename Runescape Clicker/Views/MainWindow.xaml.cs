using System.Windows;
using System.Windows.Controls;

namespace Runescape_Clicker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewPatternViewSummon(object sender, RoutedEventArgs e)
        {
            NewPatternView content = new NewPatternView();
            
            content.Reset();
            content.Show();
        }

        private void LoadPatternViewSummon(object sender, RoutedEventArgs e)
        {
            LoadPatternView content = new LoadPatternView();

            content.Show();
        }
        
        private void DeletePatternViewSummon(object sender, RoutedEventArgs e)
        {

        }
    }
}
