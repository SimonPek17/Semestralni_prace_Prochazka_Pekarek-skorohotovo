using System.ComponentModel;
using System.Windows;
using Semestralni_prace_Prochazka_Pekarek.ViewModels;

namespace Semestralni_prace_Prochazka_Pekarek.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Runtime DataContext (Designer si s tím poradí)
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (DataContext == null)
                    DataContext = new MainWindowVM();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicializace DB až po načtení okna
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            if (DataContext is MainWindowVM vm)
            {
                vm.Initialize();
            }
        }
    }
}
