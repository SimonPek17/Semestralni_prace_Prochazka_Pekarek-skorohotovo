using DataEntity;

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Semestralni_prace_Prochazka_Pekarek.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var s = new SkladContext();
            // s.Seed();

            Globals.context = s;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}