using Shop.source;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Windows;

namespace Shop.Dialogs
{
    public partial class Dlg_SearchProductByPrice : Window
    {
        private MainWindow _mainWindow;
        public Dlg_SearchProductByPrice(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            double? minPrice = TryParseDouble(MinPriceTextBox.Text);
            double? maxPrice = TryParseDouble(MaxPriceTextBox.Text);

            List<Product> products = new List<Product>();

            using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                connection.Open();

                using (var cmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Dynamic SQL basierend on entry
                string query = "SELECT ProductID, Name, Unit, Price, Quantity FROM Products WHERE 1=1";
                var command = new SQLiteCommand();
                command.Connection = connection;

                if (minPrice.HasValue)
                {
                    query += " AND Price >= @MinPrice";
                    command.Parameters.AddWithValue("@MinPrice", minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    query += " AND Price <= @MaxPrice";
                    command.Parameters.AddWithValue("@MaxPrice", maxPrice.Value);
                }

                command.CommandText = query;

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

            ResultsDataGrid.ItemsSource = products;
        }

        private double? TryParseDouble(string text)
        {
            if (double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                return value;
            return null;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
