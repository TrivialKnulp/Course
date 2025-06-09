using Shop.source;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Shop.Dialogs
{
    public partial class Dlg_Product : Window
    {
        private Regex _decimalRegex;

        private MainWindow _mainWindow;

        public Dlg_Product(MainWindow main)
        {
            InitializeComponent();
            _decimalRegex = new Regex(@"^[0-9]*(?:[\,.][0-9]*)?$");

            _mainWindow = main;
         
            CategoryComboBox.ItemsSource = _mainWindow.CategoryViewModel.Categories;
            CategoryComboBox.DisplayMemberPath = "CategoryName";
            CategoryComboBox.SelectedValuePath = "CategoryID";
        }
        //ID обраного товару. -1 означає "не вибрано"
        public int SelectedProductID { get; set; } = -1;

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string productName = NameTextBox.Text;
            string unit = UnitTextBox.Text;
            decimal price;
            var selectedCategory = CategoryComboBox.SelectedItem as Category;
            int categoryId = ((dynamic)CategoryComboBox.SelectedItem)?.CategoryID ?? 0;

            if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(unit) ||
                !decimal.TryParse(PriceTextBox.Text, out price) || categoryId == 0)
            {
                _mainWindow.AddLogMessage("Будь ласка, заповніть всі поля дійсними даними.", "Error");
                return;
            }
            DialogResult = true; 
            this.Close(); 
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
            this.Close();
        }
      
        private void DecimalOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            e.Handled = !_decimalRegex.IsMatch(fullText);
        }

        private void DecimalOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true; 
        }
    }
}
