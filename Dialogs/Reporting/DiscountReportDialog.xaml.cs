using System.Windows;

namespace Shop.Dialogs
{
    public partial class DiscountReportDialog : Window
    {
        private MainWindow _mainWindow;
        public DiscountReportDialog(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            DataContext = new DiscountReportViewModel(_mainWindow);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
