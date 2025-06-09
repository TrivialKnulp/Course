using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

namespace Shop.Dialogs
{
    public partial class DeliveryDialog : Window
    {
        private readonly MainWindow _mainWindow;
        private SQLiteConnection _connection;
        public DeliveryDialog(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
            _connection = new SQLiteConnection(_mainWindow.ConnectionString);
            LoadProducts();
            DeliveryDatePicker.SelectedDate = DateTime.Now;
        }

        private void LoadProducts()
        {
            _connection.Open();
            var cmd = new SQLiteCommand("SELECT ProductID, Name FROM Products", _connection);
            var reader = cmd.ExecuteReader();
            var products = new List<dynamic>();

            while (reader.Read())
            {
                products.Add(new { ProductID = reader.GetInt32(0), Name = reader.GetString(1) });
            }

            ProductComboBox.ItemsSource = products;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedValue == null || !double.TryParse(QuantityTextBox.Text, out double quantity))
            {
                _mainWindow.AddLogMessage("Please enter valid information.", "Error");
                return;
            }

            int productId = (int)ProductComboBox.SelectedValue;
            string deliveryDate = DeliveryDatePicker.SelectedDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");

            using (var transaction = _connection.BeginTransaction())
            {
                // Insert Delivery
                var insertCmd = new SQLiteCommand("INSERT INTO Deliveries (ProductID, Quantity, DeliveryDate) VALUES (@ProductID, @Quantity, @Date)", _connection);
                insertCmd.Parameters.AddWithValue("@ProductID", productId);
                insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                insertCmd.Parameters.AddWithValue("@Date", deliveryDate);
                insertCmd.ExecuteNonQuery();

                // Update Product Quantity + LastDeliveryDate
                var updateCmd = new SQLiteCommand("UPDATE Products SET Quantity = Quantity + @Quantity, LastDeliveryDate = @Date WHERE ProductID = @ProductID", _connection);
                updateCmd.Parameters.AddWithValue("@Quantity", quantity);
                updateCmd.Parameters.AddWithValue("@Date", deliveryDate);
                updateCmd.Parameters.AddWithValue("@ProductID", productId);
                updateCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            _mainWindow.AddLogMessage("Goods receipt recorded.", "Info");
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
