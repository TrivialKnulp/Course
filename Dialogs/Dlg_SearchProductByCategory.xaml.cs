using Shop.source;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

namespace Shop.Dialogs
{
    public partial class Dlg_SearchProductByCategory : Window
    {
        private MainWindow _mainWindow;
        public Dlg_SearchProductByCategory(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = new List<Category>();

            using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                connection.Open();

                string query = "SELECT CategoryID, CategoryName FROM Categories";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            CategoryID = reader.GetInt32(0),
                            CategoryName = reader.GetString(1)
                        });
                    }
                }
            }
            CategoryComboBox.ItemsSource = categories;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryComboBox.SelectedValue is int selectedCategoryId)
            {
                List<Product> products = new List<Product>();

                using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
                {
                    connection.Open();

                    using (var cmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string query = @"SELECT ProductID, Name, Unit, Price, Quantity
                                     FROM Products
                                     WHERE CategoryID = @CategoryID";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryID", selectedCategoryId);

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

                ProductsDataGrid.ItemsSource = products;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
