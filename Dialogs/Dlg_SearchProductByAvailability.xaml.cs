using Shop.source;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

namespace Shop.Dialogs
{
    public partial class Dlg_SearchProductByAvailability : Window
    {
        private MainWindow _mainWindow;
        public Dlg_SearchProductByAvailability(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Get availibility from Textbox and validate
            if (double.TryParse(MinAvailabilityTextBox.Text, out double minAvailability))
            {
                // Execute the search
                List<Product> products = SearchProductsByAvailability(minAvailability);
                ResultsDataGrid.ItemsSource = products;
            }
            else
            {
                _mainWindow.AddLogMessage("Please enter a valid Number for the availibility", "Error");
            }
        }

        private List<Product> SearchProductsByAvailability(double minAvailability)
        {
            List<Product> products = new List<Product>();

            using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                connection.Open();

                // SQL-Query with Filter for availability
                string query = "SELECT ProductID, Name, Unit, Price, Quantity FROM Products WHERE Quantity >= @MinAvailability";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@MinAvailability", minAvailability);

                    using (var reader = cmd.ExecuteReader())
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

            return products;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
