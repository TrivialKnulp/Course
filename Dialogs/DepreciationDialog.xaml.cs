using Shop.source;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

namespace Shop.Dialogs
{
    public partial class DepreciationDialog : Window
    {

        private MainWindow _mainWindow;
        public List<Product> Products { get; set; }


        public DepreciationDialog(List<Product> products, MainWindow main)
        {
            _mainWindow = main;
            InitializeComponent();
            Products = products;
            comboBoxProducts.ItemsSource = Products;
        }

        private void BookDepreciation_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxProducts.SelectedItem is Product selectedProduct &&
                double.TryParse(textBoxQuantity.Text, out double quantity) &&
                quantity > 0)
            {
                string reason = textBoxReason.Text;
                string date = DateTime.Now.ToString("yyyy-MM-dd");

                // Reduce product stock
                double newQuantity = selectedProduct.Quantity - quantity;
                if (newQuantity < 0)
                {
                    _mainWindow.AddLogMessage("The depreciation amount is bigger than the amount.", "Error");
                    return;
                }

                using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
                {
                    conn.Open();

                    // Update produkt stock
                    var updateCmd = new SQLiteCommand("UPDATE Products SET Quantity = @Quantity WHERE ProductID = @ProductID", conn);
                    updateCmd.Parameters.AddWithValue("@Quantity", newQuantity);
                    updateCmd.Parameters.AddWithValue("@ProductID", selectedProduct.ProductID);
                    updateCmd.ExecuteNonQuery();

                    // Save depreciations
                    var insertCmd = new SQLiteCommand("INSERT INTO Depreciations (ProductID, ProductName, Quantity, Reason, Date) VALUES (@ProductID, @ProductName, @Quantity, @Reason, @Date)", conn);
                    insertCmd.Parameters.AddWithValue("@ProductID", selectedProduct.ProductID);
                    insertCmd.Parameters.AddWithValue("@ProductName", selectedProduct.Name);
                    insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                    insertCmd.Parameters.AddWithValue("@Reason", reason);
                    insertCmd.Parameters.AddWithValue("@Date", date);
                    insertCmd.ExecuteNonQuery();
                }

                _mainWindow.AddLogMessage("Depreciation booked successfully.", "Info");
                DialogResult = true;
                Close();
            }
            else
            {
                _mainWindow.AddLogMessage("Please enter valid data.", "Error");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
