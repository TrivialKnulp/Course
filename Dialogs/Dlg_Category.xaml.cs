using System.Windows;

namespace Shop.Dialogs
{
    public partial class Dlg_Category : Window
    {
        private MainWindow _mainWindow;

        // Construktor
        public Dlg_Category(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
        }

        public int SelectedCategoryID { get; set; } = -1; // Default value for new category

        // Event-Handler for Button "OK"
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CategoryNameTextBox.Text))
            {
                _mainWindow.AddLogMessage("Entere please a category name.", "Error");
                return;
            }
            DialogResult = true; 
            Close(); // Close the dialog
        }

        // Event-Handler for Button "Cancel"
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
            this.Close(); 
        }
    }
}
