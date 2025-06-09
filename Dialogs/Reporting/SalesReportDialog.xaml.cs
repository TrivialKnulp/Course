using System.Windows;

namespace Shop.Dialogs
{
    public partial class SalesReportDialog : Window
    {
        private MainWindow _mainWindow;
        public SalesReportDialog(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
            DataContext = new SalesReportViewModel(_mainWindow);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
