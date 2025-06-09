using System.Windows;

namespace Shop.Dialogs
{
    public partial class DepreciationReportDialog : Window
    {
        private MainWindow _mainWindow;
        public DepreciationReportDialog(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            DataContext = new DepreciationReportViewModel(_mainWindow);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
