using Shop.source;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

namespace Shop.Dialogs
{
    public partial class Dlg_SearchProductName : Window
    {
        private readonly MainWindow _mainWindow;
        public Dlg_SearchProductName(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm)) return;

            List<Product> products = new List<Product>();

            using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                connection.Open();

                using (var cmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    cmd.ExecuteNonQuery(); // Wichtig
                }

                string query = @"SELECT ProductID, Name, Unit, Price, Quantity
                                 FROM Products
                                 WHERE Name LIKE @search";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@search", $"%{searchTerm}%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Unit = reader.GetString(2),
                                Price = reader.GetDouble(3),
                                Quantity = reader.GetDouble(4)
                            });
                        }
                    }
                }
            }

            ResultsDataGrid.ItemsSource = products;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
