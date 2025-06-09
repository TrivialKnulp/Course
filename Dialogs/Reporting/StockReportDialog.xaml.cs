using System.Windows;

namespace Shop.Dialogs
{
    public partial class StockReportDialog : Window
    {
        private MainWindow _mainWindow;
        public StockReportDialog(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
            DataContext = new StockReportViewModel(_mainWindow);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
